using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private IWebHostEnvironment _hostEnvironment;

        public HomeController(IWebHostEnvironment hostEnvironment)
        {
            this._hostEnvironment = hostEnvironment;
        }

        [Route("home")]
        public string Get()
        {
            return "Api ready";
        }

        [HttpGet("file/{key}")]
        public IActionResult Get(string key)
        {
            string path = Path.Combine(this._hostEnvironment.ContentRootPath, "Resources/Images");
            var image = System.IO.File.OpenRead(path + "/" + key);
            return File(image, "image/*");
        }
    }
}
