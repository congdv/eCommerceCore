using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static IHostingEnvironment _environment;
        public ImageUploadController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<ImageUploadResponse> Post(FIleUploadAPI files)
        {
            var resp = new ImageUploadResponse{ Success = false };
            if (files.files.Length > 0)
            {
                try
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\assessts\\images\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\assessts\\images\\");
                    }
                    using (FileStream filestream = System.IO.File.Create(_environment.WebRootPath + "\\assessts\\images\\" + files.files.FileName))
                    {
                        files.files.CopyTo(filestream);
                        filestream.Flush();
                        resp.Success = true;
                        resp.Message = "\\assessts\\images\\" + files.files.FileName;
                    }
                }
                catch (Exception ex)
                {
                    resp.Message = ex.Message;
                    resp.Success = false;
                }
            }
            else
            {
                resp.Success = false;
                resp.Message = "No Image to upload";
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
        public IFormFile files { get; set; }
    }
}