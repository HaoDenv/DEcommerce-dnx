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
    public class UsersController : ControllerBase
    {
        private UserService _userService;
        public UsersController(UserService userService)
        {
            this._userService = userService;
        }

        [HttpGet]
        public IActionResult Get(string keySearch)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._userService.Get(keySearch);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("{username}")]
        [HttpGet]
        public IActionResult GetById(string username)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._userService.GetById(username);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpPost]
        public IActionResult Post(UserDto user)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._userService.Insert(user);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("{username}")]
        [HttpPut]
        public IActionResult Put(string username, UserDto user)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._userService.Update(username, user);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("{username}")]
        [HttpDelete]
        public IActionResult Delete(string username)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._userService.DeleteById(username);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("reset-passsword")]
        [HttpGet]
        public IActionResult ResetPassword(string username, string newPassword)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._userService.ResetPassword(username, newPassword);
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
