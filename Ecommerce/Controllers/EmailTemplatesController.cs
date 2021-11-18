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
    public class EmailTemplatesController : ControllerBase
    {
        private EmailTemplateService _emailTemplateService;
        public EmailTemplatesController(EmailTemplateService emailTemplateService)
        {
            this._emailTemplateService = emailTemplateService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._emailTemplateService.GetAll();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult GetById(int id)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._emailTemplateService.GetById(id);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("{id}")]
        [HttpPut]
        public IActionResult Put(int id, EmailTemplateDto emailTemplate)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._emailTemplateService.Update(id, emailTemplate);
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
