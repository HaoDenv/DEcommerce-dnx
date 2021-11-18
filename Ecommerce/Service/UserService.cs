using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Dto;
using Ecommerce.Model;
using Ecommerce.Util;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Service
{
    public class UserService : IServiceBase<UserDto, string>
    {
        protected readonly MyContext context;
        public UserService(MyContext context)
        {
            this.context = context;
        }

        public void DeleteById(string key, string userSession = null)
        {
            User user = this.context.Users.FirstOrDefault(x => x.UserName == key);
            this.context.Users.Remove(user);

            this.context.SaveChanges();
        }

        public List<UserDto> GetAll()
        {
            return this.context.Users
                .Select(x => new UserDto()
                {
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Email = x.Email,
                    LastLogin = x.LastLogin,
                    Phone = x.Phone,
                    Active = x.Active
                })
                .ToList();
        }

        public List<UserDto> Get(string keySearch)
        {
            if (!string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            return this.context.Users
                .Where(x => keySearch == null || x.UserName.Contains(keySearch) || x.FullName.Contains(keySearch))
                 .Select(x => new UserDto()
                 {
                     UserName = x.UserName,
                     FullName = x.FullName,
                     Email = x.Email,
                     LastLogin = x.LastLogin,
                     Phone = x.Phone,
                     Active = x.Active
                 })
                 .ToList();
        }

        public UserDto GetById(string key)
        {
            return this.context.Users
               .Where(x => x.UserName == key)
               .Select(x => new UserDto()
               {
                   UserName = x.UserName,
                   FullName = x.FullName,
                   Email = x.Email,
                   LastLogin = x.LastLogin,
                   Phone = x.Phone,
                   Active = x.Active
               })
               .FirstOrDefault();
        }

        public UserDto Insert(UserDto entity)
        {
            if (this.context.Users.Any(x => x.UserName == entity.UserName))
                throw new ArgumentException("Tên tài khoản đã tồn tại");

            User user = new User()
            {
                UserName = entity.UserName,
                Phone = entity.Phone,
                Active = entity.Active,
                Email = entity.Email,
                FullName = entity.FullName,
                Password = DataHelper.SHA256Hash(entity.UserName + "_" + entity.Password),
            };

            this.context.Users.Add(user);

            this.context.SaveChanges();

            return entity;
        }

        public void Update(string key, UserDto entity)
        {
            User user = this.context.Users.FirstOrDefault(x => x.UserName == key);

            user.Phone = entity.Phone;
            user.Active = entity.Active;
            user.Email = entity.Email;
            user.FullName = entity.FullName;

            this.context.SaveChanges();
        }

        public void ResetPassword(string key, string newPassword)
        {
            User user = this.context.Users.FirstOrDefault(x => x.UserName == key);

            user.Password = DataHelper.SHA256Hash(user.UserName + "_" + newPassword);
            this.context.SaveChanges();
        }

        public object GetAccessToken(UserDto entity)
        {
            User user = this.context.Users
                .Where(x => x.Active)
                .FirstOrDefault(x => x.UserName == entity.UserName);

            if (user == null)
                throw new ArgumentException("Tài khoản hoặc mật khẩu không đúng");

            string passwordCheck = DataHelper.SHA256Hash(entity.UserName + "_" + entity.Password);

            if (user.Password != passwordCheck)
                throw new ArgumentException("Tài khoản hoặc mật khẩu không đúng");

            user.LastLogin = DateTime.Now;
            this.context.SaveChanges();

            DateTime expirationDate = DateTime.Now.Date.AddMinutes(Constants.JwtConfig.ExpirationInMinutes);
            long expiresAt = (long)(expirationDate - new DateTime(1970, 1, 1)).TotalSeconds;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Constants.JwtConfig.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.UserData, user.UserName),
                        new Claim(ClaimTypes.Expiration, expiresAt.ToString())
                }),
                Expires = expirationDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new
            {
                user.UserName,
                user.FullName,
                Token = tokenHandler.WriteToken(token),
                ExpiresAt = expiresAt
            };
        }
    }
}
