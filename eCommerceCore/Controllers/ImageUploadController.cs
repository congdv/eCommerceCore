using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceCore.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] FileUpload fileUpload)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            //check if userId is null
            if (userId == null)
            {
                return BadRequest(new { success = false, message = "Login Failed" });
            }
            bool isSuccess = false;
            if (fileUpload.file != null && fileUpload.file.Length > 0)
            {
                try
                {
                    int index = fileUpload.file.FileName.LastIndexOf(".");
                    string extensionName = fileUpload.file.FileName.Substring(index);
                    var directoryPath = Path.Combine(@"wwwroot\assets");
                    if(!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    var filePath = Path.Combine(@"wwwroot\assets", Path.GetRandomFileName() + extensionName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.file.CopyToAsync(stream);
                        isSuccess = true;
                    }
                    if (isSuccess)
                    {
                        return Ok(new { success = true, message = "Successfully uploaded image", data = filePath.ToString() });

                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Failed to upload image" });
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, message = ex.Message});
                }
            }
            else
            {
                return BadRequest(new { success = false, message = "The image is not valid" });
            }

           
        }
    }


    public class FileUpload
    {
        public IFormFile file { get; set; }
    }
}

