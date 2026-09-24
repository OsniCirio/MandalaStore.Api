using SixLabors.ImageSharp;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using static System.Net.Mime.MediaTypeNames;
using Image = SixLabors.ImageSharp.Image;

namespace MandalaStore.API.Helpers;

public static class ImageHashHelper
{
    public static string GenerateHash(string imagePath)
    {
        using var image =
       Image.Load<Rgba32>(imagePath);

        // reduz imagem
        image.Mutate(x =>
            x.Resize(9, 8)
             .Grayscale());

        ulong hash = 0;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var left = image[x, y];
                var right = image[x + 1, y];

                if (left.R > right.R)
                {
                    hash |= 1UL << (y * 8 + x);
                }
            }
        }

        return hash.ToString("X");
    }

    public static int HammingDistance(
        string hash1,
        string hash2)
    {
        ulong h1 = Convert.ToUInt64(hash1, 16);
        ulong h2 = Convert.ToUInt64(hash2, 16);

        ulong xor = h1 ^ h2;

        int distance = 0;

        while (xor != 0)
        {
            distance++;
            xor &= xor - 1;
        }

        return distance;
    }
}
