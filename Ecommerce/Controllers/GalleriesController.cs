using Ecommerce.Dto;
using Ecommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalleriesController : ControllerBase
    {
        private GalleryService _galleryService;
        public GalleriesController(GalleryService galleryService)
        {
            this._galleryService = galleryService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._galleryService.GetAll();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpPost]
        public IActionResult Post(GalleryDto gallery)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._galleryService.Insert(gallery);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._galleryService.DeleteById(id);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
    }
}
