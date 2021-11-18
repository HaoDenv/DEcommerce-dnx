using Ecommerce.Dto;
using Ecommerce.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Service
{
    public class EmailConfigurationService
    {
        protected readonly MyContext context;
        public EmailConfigurationService(MyContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get cấu hình tài khoản gửi mail
        /// </summary>
        /// <returns></returns>
        public EmailConfigurationDto Get()
        {
            return this.context.EmailConfigurations
                 .Select(x => new EmailConfigurationDto()
                 {
                     Id = x.Id,
                     Email = x.Email,
                     Password = x.Password
                 })
                 .FirstOrDefault();
        }

        /// <summary>
        /// Cập nhật tài khoản gửi mail
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        public void Update(int key, EmailConfigurationDto entity)
        {
            EmailConfiguration emailConfiguration = this.context.EmailConfigurations
                 .FirstOrDefault(x => x.Id == key);

            emailConfiguration.Email = entity.Email;
            emailConfiguration.Password = entity.Password;

            this.context.SaveChanges();
        }
    }
}