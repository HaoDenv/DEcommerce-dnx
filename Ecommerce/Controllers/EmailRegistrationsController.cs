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
    public class EmailRegistrationsController : ControllerBase
    {
        private EmailRegistrationService _emailRegistrationService;

        public EmailRegistrationsController(EmailRegistrationService emailRegistrationService)
        {
            this._emailRegistrationService = emailRegistrationService;
        }

        [HttpGet]
        public IActionResult Get(string keySearch)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._emailRegistrationService.Get(keySearch);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpPost]
        public IActionResult Post(EmailRegistrationDto email)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._emailRegistrationService.Insert(email);
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
