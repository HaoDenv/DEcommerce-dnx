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
    public class GalleryService : IServiceBase<GalleryDto, int>
    {
        protected readonly MyContext context;
        protected IWebHostEnvironment hostEnvironment;
        public GalleryService(MyContext context, IWebHostEnvironment hostEnvironment)
        {
            this.context = context;
            this.hostEnvironment = hostEnvironment;
        }

        public void DeleteById(int key, string userSession = null)
        {
            this.context.Galleries.Remove(context.Galleries.FirstOrDefault(x => x.Id == key));
            this.context.SaveChanges();
        }

        public List<GalleryDto> GetAll()
        {
            return this.context.Galleries.Select(x => new GalleryDto()
            {
                Id = x.Id,
                Image = x.Image,
                Type = x.Type
            }).ToList();
        }

        public GalleryDto GetById(int key)
        {
            return this.context.Galleries
                .Where(x => x.Id == key)
                .Select(x => new GalleryDto()
                {
                    Id = x.Id,
                    Image = x.Image,
                    Type = x.Type
                }).FirstOrDefault();
        }

        public GalleryDto Insert(GalleryDto entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.Image))
            {
                if (entity.Image.Contains("data:image/png;base64,"))
                {
                    string path = Path.Combine(this.hostEnvironment.ContentRootPath, $"Resources/Images");
                    string imgName = Guid.NewGuid().ToString("N") + ".png";
                    var bytes = Convert.FromBase64String(entity.Image.Replace("data:image/png;base64,", ""));
                    using (var imageFile = new FileStream(path + "/" + imgName, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                    entity.Image = imgName;
                }

            }

            Gallery gallery = new Gallery()
            {
                Image = entity.Image,
                Type = entity.Type
            };

            this.context.Galleries.Add(gallery);
            this.context.SaveChanges();

            return entity;

        }

        public void Update(int key, GalleryDto entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.Image))
            {
                if (entity.Image.Contains("data:image/png;base64,"))
                {
                    string path = Path.Combine(this.hostEnvironment.ContentRootPath, $"Resources/Images");
                    string imgName = Guid.NewGuid().ToString("N") + ".png";
                    var bytes = Convert.FromBase64String(entity.Image.Replace("data:image/png;base64,", ""));
                    using (var imageFile = new FileStream(path + "/" + imgName, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                    entity.Image = imgName;
                }

            }

            Gallery gallery = this.context.Galleries.FirstOrDefault(x => x.Id == key);
            gallery.Image = entity.Image;
            gallery.Type = entity.Type;

            this.context.SaveChanges();
        }
    }
}