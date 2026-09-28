using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MandalaStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment env;

        public UploadController(IWebHostEnvironment _env)
        {
            env = _env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            var folder = Path.Combine(
                env.ContentRootPath,
                "wwwroot",
                "uploads");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);

            await file.CopyToAsync(stream);

            var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

            return Ok(new { url });
        }
    }
}
