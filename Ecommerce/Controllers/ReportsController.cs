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
    public class ReportsController : ControllerBase
    {
        private ReportService _reportService;
        public ReportsController(ReportService reportService)
        {
            this._reportService = reportService;
        }

        [Route("highlight")]
        [HttpGet]
        public IActionResult GetHighlight(DateTime? date)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._reportService.GetHighlight(date);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("general")]
        [HttpGet]
        public IActionResult GetGeneralReport(string keySearch, int status, DateTime? fDate, DateTime? tDate)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._reportService.GetGeneralReport(keySearch, status, fDate, tDate);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("product-report")]
        [HttpGet]
        public IActionResult GetProductReport(string keySearch, DateTime? fDate, DateTime? tDate)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._reportService.GetProductReport(keySearch, fDate, tDate);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("revenue")]
        [HttpGet]
        public IActionResult GetRevenueReport(DateTime date)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._reportService.GetRevenueReport(date);
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
