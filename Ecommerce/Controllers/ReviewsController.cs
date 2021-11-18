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
    public class ReviewsController : ControllerBase
    {
        private ReviewService _reviewService;
        public ReviewsController(ReviewService reviewService)
        {
            this._reviewService = reviewService;
        }

        [HttpGet]
        public IActionResult Get(string keySearch = null, int status = -1)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._reviewService.Get(keySearch, status);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-status/{id}")]
        [HttpGet]
        public IActionResult UpdateStatus(int id, int status)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._reviewService.UpdateStatus(id, status);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("review")]
        [HttpGet]
        public IActionResult Review(int orderDetailId, int rate, string comment)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._reviewService.Insert(orderDetailId, rate, comment);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-by-order/{orderDetailId}")]
        [HttpGet]
        public IActionResult GetByOrder(int orderDetailId)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._reviewService.GetByOrder(orderDetailId);
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
