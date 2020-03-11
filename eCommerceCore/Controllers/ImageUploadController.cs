using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceCore.Controllers
{
    [Route("api/Image")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        [HttpPost]
        public async Task<ImageUploadResponse> Post([FromQuery]FIleUploadAPI uploadFile)
        {
            var resp = new ImageUploadResponse { Success = false, Message = "No file to upload" };
            if (uploadFile.file.Length > 0)
            {
                try
                {
                    int index = uploadFile.file.FileName.LastIndexOf(".");
                    string extn = uploadFile.file.FileName.Substring(index);
                    var filePath = Path.Combine("wwwroot\\assesst", Path.GetRandomFileName() + extn);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadFile.file.CopyToAsync(stream);
                        
                        resp.Success = true;
                        resp.Message = filePath.ToString();
                    }
                }
                catch (Exception ex)
                {
                    resp.Success = false;
                    resp.Message = ex.Message;
                }
            }
            else
            {
                resp.Success = false;
                resp.Message = "No file to upload";
            }
            return resp;
        }
    }

    public class ImageUploadResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class FIleUploadAPI
    {
        public IFormFile file { get; set; }
    }
}

