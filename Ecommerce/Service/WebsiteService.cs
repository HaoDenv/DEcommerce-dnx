using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Dto;
using Ecommerce.Model;
using Microsoft.AspNetCore.Hosting;

namespace Ecommerce.Service
{
    public class WebsiteService : IServiceBase<WebsiteDto, int>
    {
        protected readonly MyContext context;
        protected IWebHostEnvironment hostEnvironment;

        public WebsiteService(MyContext context, IWebHostEnvironment hostEnvironment)
        {
            this.context = context;
            this.hostEnvironment = hostEnvironment;
        }

        public void DeleteById(int key, string userSession = null)
        {
            throw new NotImplementedException();
        }

        public List<WebsiteDto> GetAll()
        {
            return this.context.Websites.Select(x => new WebsiteDto()
            {
                Address = x.Address,
                Copyright = x.Copyright,
                Email = x.Email,
                Facebook = x.Facebook,
                Fax = x.Fax,
                Id = x.Id,
                Location = x.Location,
                Logo = x.Logo,
                Name = x.Name,
                PhoneNumber = x.PhoneNumber,
                Youtube = x.Youtube
            }).ToList();
        }

        public WebsiteDto GetById(int key)
        {
            throw new NotImplementedException();
        }

        public WebsiteDto Insert(WebsiteDto entity)
        {
            throw new NotImplementedException();
        }

        public void Update(int key, WebsiteDto entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.Logo))
            {
                if (entity.Logo.Contains("data:image/png;base64,"))
                {
                    string path = Path.Combine(this.hostEnvironment.ContentRootPath, $"Resources/Images");
                    string imgName = Guid.NewGuid().ToString("N") + ".png";
                    var bytes = Convert.FromBase64String(entity.Logo.Replace("data:image/png;base64,", ""));
                    using (var imageFile = new FileStream(path + "/" + imgName, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                    entity.Logo = imgName;
                }

            }

            Website website = this.context.Websites.FirstOrDefault(x => x.Id == key);

            website.Address = entity.Address;
            website.Copyright = entity.Copyright;
            website.Email = entity.Email;
            website.Facebook = entity.Facebook;
            website.Fax = entity.Fax;
            website.Location = entity.Location;
            website.Logo = entity.Logo;
            website.Name = entity.Name;
            website.PhoneNumber = entity.PhoneNumber;
            website.Youtube = entity.Youtube;

            this.context.SaveChanges();
        }
    }
}
