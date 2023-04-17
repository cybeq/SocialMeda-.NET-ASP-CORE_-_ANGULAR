using Microsoft.AspNetCore.Mvc;
using ImageMagick;
using System.Transactions;

namespace newapi.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
      
        [HttpPost("image")]
        public IActionResult UploadImage(IFormFile formFile)
        {
            if (!IsImage(formFile))
            {
                return BadRequest("[ not an image ]");
            }
            if (formFile.Length > 1024 * 1024 * 3) // 1MB = 1024 * 1024 bytes
            {
                return BadRequest("[ file size exceeds 3MB ]");
            }

            string fileName =
                  "uploaded_photo_"
                + (DateTime.UtcNow.Ticks / (TimeSpan.TicksPerMillisecond / 1000)).ToString()
                + Path.GetExtension(formFile.FileName);

            string uploadsFolder = Path.Combine("uploads", "images");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string filePath = Path.Combine(uploadsFolder, fileName);   

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }

            ConvertImageToCustomProperties(filePath, fileName);

            string url = $"{Request.Scheme}://{Request.Host}/api/upload/images/{Path.GetFileNameWithoutExtension(fileName) + ".jpg"}";
            return Ok("UPLOADED: " + new { url });
        

        }


        [HttpGet("images/{name}")]
        public IActionResult GetUploadedPhoto(string name)
        {
            string filePath = "uploads/images/" + name;
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            return File(fileStream, "image/*");

        }



        private bool IsImage(IFormFile formFile)
        {
            if (formFile == null)
            {
                return false;
            }
            string ext = Path.GetExtension(formFile.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif")
            {
                return false;
            };
            try
            {
                var image = new MagickImage(formFile.OpenReadStream());
            }
            catch (MagickException)
            {
                return false;
            }
            return true;
        }
        private void ConvertImageToCustomProperties(string filePath, string fileName)
        {
            var uFileName = Path.GetFileNameWithoutExtension(filePath) + ".jpg";
            var outputFile = Path.Combine("uploads", "images", uFileName);

            using (var image = new MagickImage(filePath))
            {
                if (image.Width > image.Height)
                {
                    while (image.Height > 400)
                        image.Resize(image.Width, image.Height - Convert.ToInt32(image.Height * 0.1));
                    while (image.Width > 600)
                        image.Resize(image.Width - Convert.ToInt32(image.Width * 0.1), image.Height);
                }
                else
                {
                    while (image.Height > 600)
                        image.Resize(image.Width, image.Height - Convert.ToInt32(image.Height * 0.1));
                    while (image.Width > 400)
                        image.Resize(image.Width - Convert.ToInt32(image.Width * 0.1), image.Height);
                }

                image.Format = MagickFormat.Jpg;
                image.Write(outputFile);

            }
            if (fileName != uFileName)
                System.IO.File.Delete(filePath);
        }
    }
}