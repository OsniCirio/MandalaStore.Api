using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OpenCvSharp;

namespace MandalaStore.Infrastructure.Helpers
{
    public static class UploadHelper
    {
        public static async Task<string> UploadImagemAsync(
            IFormFile file,
            IWebHostEnvironment env)
        {
            var folder =
                Path.Combine(
                    env.WebRootPath ?? "wwwroot",
                    "uploads");

            Directory.CreateDirectory(folder);

            // nome original
            var originalName =
                $"{Guid.NewGuid()}_{file.FileName}";

            var originalPath =
                Path.Combine(folder, originalName);

            // salva original
            using (var stream =
                new FileStream(
                    originalPath,
                    FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // processa imagem
            var finalUrl =
                ProcessarMandala(
                    originalPath,
                    folder);

            return finalUrl;
        }

        private static string ProcessarMandala(
    string imagePath,
    string folder)
        {
            // =========================================================
            // 1. CARREGA A IMAGEM ORIGINAL
            // =========================================================
            using var image = Cv2.ImRead(imagePath);

            if (image.Empty())
                return $"/uploads/{Path.GetFileName(imagePath)}";


            // =========================================================
            // 2. CRIA UMA IMAGEM PEQUENA APENAS PARA DETECÇÃO
            // =========================================================
            const int tamanhoDeteccao = 600;

            double escala =
                tamanhoDeteccao /
                (double)Math.Max(image.Width, image.Height);

            using var small = new Mat();

            Cv2.Resize(
                image,
                small,
                new Size(),
                escala,
                escala,
                InterpolationFlags.Area);


            // =========================================================
            // 3. CONVERTE A IMAGEM PEQUENA PARA CINZA
            // =========================================================
            using var gray = new Mat();

            Cv2.CvtColor(
                small, // IMPORTANTE: small, e não image
                gray,
                ColorConversionCodes.BGR2GRAY);

            Cv2.GaussianBlur(
                gray,
                gray,
                new Size(9, 9),
                2);


            Console.WriteLine(
                $"ORIGINAL: {image.Width} x {image.Height}");

            Console.WriteLine(
                $"SMALL: {small.Width} x {small.Height}");

            Console.WriteLine(
                $"HOUGH: {gray.Width} x {gray.Height}");

            // =========================================================
            // 4. SEPARA A MANDALA DO FUNDO CLARO
            // =========================================================
            using var mascaraObjeto = new Mat();

            Cv2.Threshold(
                gray,
                mascaraObjeto,
                0,
                255,
                ThresholdTypes.BinaryInv |
                ThresholdTypes.Otsu);


            // Une pequenas falhas provocadas pelos fios
            using var kernel =
                Cv2.GetStructuringElement(
                    MorphShapes.Ellipse,
                    new Size(11, 11));

            Cv2.MorphologyEx(
                mascaraObjeto,
                mascaraObjeto,
                MorphTypes.Close,
                kernel,
                iterations: 2);


            // =========================================================
            // 5. LOCALIZA O CONTORNO EXTERNO
            // =========================================================
            Cv2.FindContours(
                mascaraObjeto,
                out Point[][] contornos,
                out HierarchyIndex[] hierarquia,
                RetrievalModes.External,
                ContourApproximationModes.ApproxSimple);

            double areaMinima =
                small.Width *
                small.Height *
                0.20;

            var contornoMandala =
                contornos
                    .Where(c => c.Length >= 5)
                    .Where(c =>
                        Cv2.ContourArea(c) >= areaMinima)
                    .OrderByDescending(c =>
                        Cv2.ContourArea(c))
                    .FirstOrDefault();

            if (contornoMandala == null)
            {
                Console.WriteLine(
                    "Não foi possível localizar o contorno externo.");

                return $"/uploads/{Path.GetFileName(imagePath)}";
            }


            // =========================================================
            // 6. AJUSTA UMA ELIPSE AO CONTORNO
            // =========================================================
            RotatedRect elipseSmall =
                Cv2.FitEllipse(contornoMandala);

            double maiorEixo =
                Math.Max(
                    elipseSmall.Size.Width,
                    elipseSmall.Size.Height);

            double menorEixo =
                Math.Min(
                    elipseSmall.Size.Width,
                    elipseSmall.Size.Height);

            Console.WriteLine(
                $"ELIPSE SMALL: " +
                $"Centro=({elipseSmall.Center.X:F0}," +
                $"{elipseSmall.Center.Y:F0}) " +
                $"Tamanho={elipseSmall.Size.Width:F0}x" +
                $"{elipseSmall.Size.Height:F0} " +
                $"Ovalização={maiorEixo / menorEixo:F3}");


            // =========================================================
            // 7. CONVERTE A ELIPSE PARA A IMAGEM ORIGINAL
            // =========================================================
            float fator =
                (float)(1.0 / escala);

            var elipseOriginal =
                new RotatedRect(
                    new Point2f(
                        elipseSmall.Center.X * fator,
                        elipseSmall.Center.Y * fator),

                    new Size2f(
                        elipseSmall.Size.Width * fator,
                        elipseSmall.Size.Height * fator),

                    elipseSmall.Angle);


            // =========================================================
            // 8. CRIA UM CROP QUADRADO COM 5% DE MARGEM
            // =========================================================
            int size =
                (int)Math.Ceiling(
                    Math.Max(
                        elipseOriginal.Size.Width,
                        elipseOriginal.Size.Height) *
                    1.05);

            size =
                Math.Min(
                    size,
                    Math.Min(
                        image.Width,
                        image.Height));

            int x =
                (int)Math.Round(
                    elipseOriginal.Center.X -
                    size / 2.0);

            int y =
                (int)Math.Round(
                    elipseOriginal.Center.Y -
                    size / 2.0);

            x =
                Math.Clamp(
                    x,
                    0,
                    image.Width - size);

            y =
                Math.Clamp(
                    y,
                    0,
                    image.Height - size);

            var roi =
                new Rect(
                    x,
                    y,
                    size,
                    size);

            using var cropped =
                new Mat(image, roi);


            // =========================================================
            // 9. CONVERTE PARA BGRA
            // =========================================================
            using var imagemTransparente =
                new Mat();

            Cv2.CvtColor(
                cropped,
                imagemTransparente,
                ColorConversionCodes.BGR2BGRA);


            // =========================================================
            // 10. CRIA A MÁSCARA EM FORMATO DE ELIPSE
            // =========================================================
            using var mascaraAlpha =
                new Mat(
                    cropped.Size(),
                    MatType.CV_8UC1,
                    Scalar.All(0));

            var elipseNoCrop =
                new RotatedRect(
                    new Point2f(
                        elipseOriginal.Center.X - x,
                        elipseOriginal.Center.Y - y),

                    new Size2f(
                        elipseOriginal.Size.Width,
                        elipseOriginal.Size.Height),

                    elipseOriginal.Angle);

            Cv2.Ellipse(
                mascaraAlpha,
                elipseNoCrop,
                Scalar.All(255),
                thickness: -1,
                lineType: LineTypes.AntiAlias);

            Cv2.InsertChannel(
                mascaraAlpha,
                imagemTransparente,
                3);


            // =========================================================
            // 11. SALVA O PNG TRANSPARENTE
            // =========================================================
            var finalName =
                $"{Guid.NewGuid()}_crop.png";

            var finalPath =
                Path.Combine(
                    folder,
                    finalName);

            Cv2.ImWrite(
                finalPath,
                imagemTransparente);

            return $"/uploads/{finalName}";
        }
    }
}
