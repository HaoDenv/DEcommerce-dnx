using Ecommerce.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Service
{
    public class EmailRegistrationService
    {
        protected readonly MyContext context;
        public EmailRegistrationService(MyContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get danh sách email đăng ký nhận tin
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        public List<EmailRegistrationDto> Get(string keySearch)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            return this.context.EmailRegistrations
                .Where(x => keySearch == null || x.Email.Contains(keySearch))
                 .Select(x => new EmailRegistrationDto()
                 {
                     Id = x.Id,
                     Email = x.Email,
                     Created = x.Created
                 })
                 .ToList();
        }

        /// <summary>
        /// Thêm mới email nhận tin
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public EmailRegistrationDto Insert(EmailRegistrationDto entity)
        {
            if (!this.context.EmailRegistrations.Any(x => x.Email == entity.Email))
            {
                this.context.EmailRegistrations.Add(new Model.EmailRegistration()
                {
                    Email = entity.Email,
                    Created = DateTime.Now
                });

                this.context.SaveChanges();
            }
            return entity;
        }
    }
}