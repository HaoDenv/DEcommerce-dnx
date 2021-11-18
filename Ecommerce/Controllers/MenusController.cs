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
    public class MenusController : ControllerBase
    {
        private MenuService _menuService;
        public MenusController(MenuService menuService)
        {
            this._menuService = menuService;
        }

        [Route("get-main-menu")]
        [HttpGet]
        public IActionResult GetMainMenu(string keySearch = null)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._menuService.GetMainMenu(keySearch);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-sub-menu")]
        [HttpGet]
        public IActionResult GetSubMenu(string keySearch = null)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._menuService.GetSubMenu(keySearch);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-parent-main-menu")]
        [HttpGet]
        public IActionResult GetParentMainMenu()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._menuService.GetParentMainMenu();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-parent-sub-menu")]
        [HttpGet]
        public IActionResult GetParentSubMenu()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._menuService.GetParentSubMenu();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-by-type")]
        [HttpGet]
        public IActionResult GetByType([FromQuery] List<string> types)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._menuService.GetByType(types);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-main-menu-active")]
        [HttpGet]
        public IActionResult GetMainMenuActive()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._menuService.GetMainMenuActive();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-sub-menu-active")]
        [HttpGet]
        public IActionResult GetSubMenuActive()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._menuService.GetSubMenuActive();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-all-menu-homepage")]
        [HttpGet]
        public IActionResult GetAllShowHomePage()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = this._menuService.GetAllShowHomePage();
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
                responseAPI.Data = this._menuService.GetById(id);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpPost]
        public IActionResult Post(MenuDto menu)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._menuService.Insert(menu);
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
        public IActionResult Put(int id, MenuDto menu)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                this._menuService.Update(id, menu);
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
                this._menuService.DeleteById(id);
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
