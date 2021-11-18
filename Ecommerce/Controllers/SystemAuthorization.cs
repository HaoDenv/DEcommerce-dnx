using Ecommerce.Dto;
using Ecommerce.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    public class SystemAuthorization : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string token = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(token))
            {
                ResponseAPI responseAPI = new ResponseAPI();
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                responseAPI.Message = "Lỗi xác thực";
                context.Result = new JsonResult(responseAPI);
            }
            else
            {
                try
                {
                    string tokenValue = token.Replace("Bearer", string.Empty).Trim();
                    ClaimsPrincipal claimsPrincipal = DecodeJWTToken(tokenValue, Constants.JwtConfig.SecretKey);
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                catch
                {
                    ResponseAPI responseAPI = new ResponseAPI();
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    responseAPI.Message = "Lỗi xác thực";
                    context.Result = new JsonResult(responseAPI);
                }
            }
        }

        public static string GetCurrentUser(IHttpContextAccessor context)
        {
            try
            {
                string token = context.HttpContext.Request.Headers["Authorization"].ToString();
                string tokenValue = token.Replace("Bearer", string.Empty).Trim();
                ClaimsPrincipal claimsPrincipal = DecodeJWTToken(tokenValue, Constants.JwtConfig.SecretKey);
                return claimsPrincipal.FindFirstValue(ClaimTypes.UserData);
            }
            catch
            {
                throw new ArgumentException("Lỗi xác thực");
            }
        }

        public static ClaimsPrincipal DecodeJWTToken(string token, string secretAuthKey)
        {
            var key = Encoding.ASCII.GetBytes(secretAuthKey);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(token, validations, out var tokenSecure);
            return claims;
        }
    }
    public class SystemAuthorizeAttribute : TypeFilterAttribute
    {
        public SystemAuthorizeAttribute() : base(typeof(SystemAuthorization))
        {
        }
    }
}
