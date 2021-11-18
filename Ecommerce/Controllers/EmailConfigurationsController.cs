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
    public class EmailConfigurationsController : ControllerBase
    {
        private EmailConfigurationService _emailConfigurationService;
        public EmailConfigurationsController(EmailConfigurationService emailConfigurationService)
        {
            this._emailConfigurationService = emailConfigurationService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._emailConfigurationService.Get();
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
        public IActionResult Update(int id, EmailConfigurationDto emailConfiguration)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._emailConfigurationService.Update(id, emailConfiguration);
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
