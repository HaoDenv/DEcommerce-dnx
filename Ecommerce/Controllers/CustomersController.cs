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
    public class CustomersController : ControllerBase
    {
        private CustomerService _customerService;
        private IHttpContextAccessor _contextAccessor;
        public CustomersController(CustomerService customerService, IHttpContextAccessor contextAccessor)
        {
            this._customerService = customerService;
            this._contextAccessor = contextAccessor;
        }

        [Route("login")]
        [HttpPost]
        public IActionResult GetAccessToken(CustomerDto customer)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._customerService.GetAccessToken(customer);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("request-otp")]
        [HttpGet]
        public IActionResult RequestOTP(string email)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._customerService.RequestOTP(email);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("confirm-otp")]
        [HttpGet]
        public IActionResult RequestOTP(string email, string otp)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._customerService.ConfirmOTP(email, otp);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("forgot-password")]
        [HttpGet]
        public IActionResult ForgotPassword(string email)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._customerService.ForgotPassword(email);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-profile")]
        [SystemAuthorize]
        [HttpGet]
        public IActionResult GetProfile()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(this._contextAccessor);
                responseAPI.Data = this._customerService.GetById(userSession);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-orders")]
        [SystemAuthorize]
        [HttpGet]
        public IActionResult GetOrders()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(this._contextAccessor);
                responseAPI.Data = this._customerService.GetOrders(userSession);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpGet]
        public IActionResult Get(string keySearch)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._customerService.Get(keySearch);
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
        public IActionResult GetById(string id)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._customerService.GetById(id);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpPost]
        public IActionResult Post(CustomerDto customer)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._customerService.Insert(customer);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-profile")]
        [SystemAuthorize]
        [HttpPut]
        public IActionResult Put(CustomerDto customer)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(this._contextAccessor);
                this._customerService.Update(userSession, customer);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("change-password")]
        [SystemAuthorize]
        [HttpGet]
        public IActionResult ChangePassword(string oldPassword, string newPassword)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(this._contextAccessor);
                this._customerService.ChangePassword(userSession, oldPassword, newPassword);
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
