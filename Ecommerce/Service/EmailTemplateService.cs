using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Dto;
using Ecommerce.Model;

namespace Ecommerce.Service
{
    public class EmailTemplateService : IServiceBase<EmailTemplateDto, int>
    {
        protected readonly MyContext context;
        public EmailTemplateService(MyContext context)
        {
            this.context = context;
        }

        public virtual void DeleteById(int key, string userSession = null)
        {
            throw new NotImplementedException();
        }

        public virtual List<EmailTemplateDto> GetAll()
        {
            return this.context.EmailTemplates
                 .Select(x => new EmailTemplateDto()
                 {
                     Id = x.Id,
                     Subject = x.Subject,
                     Type = x.Type
                 })
                 .ToList();
        }

        public virtual EmailTemplateDto GetById(int key)
        {
            return this.context.EmailTemplates
                 .Where(x => x.Id == key)
                 .Select(x => new EmailTemplateDto()
                 {
                     Content = x.Content,
                     BCC = x.BCC,
                     CC = x.CC,
                     Id = x.Id,
                     KeyGuide = x.KeyGuide,
                     Subject = x.Subject,
                     Type = x.Type
                 })
                 .FirstOrDefault();
        }

        public virtual EmailTemplateDto Insert(EmailTemplateDto entity)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(int key, EmailTemplateDto entity)
        {
            EmailTemplate emailTemplate = this.context.EmailTemplates
                 .FirstOrDefault(x => x.Id == key);

            emailTemplate.Subject = entity.Subject;
            emailTemplate.CC = entity.CC;
            emailTemplate.BCC = entity.BCC;
            emailTemplate.Content = entity.Content;

            this.context.SaveChanges();
        }
    }
}