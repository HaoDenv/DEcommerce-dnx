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
    public class AuthsController : ControllerBase
    {
        private UserService _userService;
        public AuthsController(UserService userService)
        {
            this._userService = userService;
        }

        [HttpPost]
        public IActionResult GetAccessToken(UserDto user)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._userService.GetAccessToken(user);
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
