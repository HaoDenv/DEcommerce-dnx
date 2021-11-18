using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Dto;
using Ecommerce.Model;
using Ecommerce.Util;
using Microsoft.AspNetCore.Hosting;

namespace Ecommerce.Service
{
    public class ArticleService : IServiceBase<ArticleDto, int>
    {
        protected readonly MyContext context;
        protected IWebHostEnvironment hostEnvironment;
        public ArticleService(MyContext context, IWebHostEnvironment hostEnvironment)
        {
            this.context = context;
            this.hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Xóa bài viết
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userSession"></param>
        public void DeleteById(int key, string userSession = null)
        {
            Article article = this.context.Articles.FirstOrDefault(x => x.Id == key);

            if (article != null)
            {
                this.context.Articles.Remove(article);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Get bài viết theo từ khóa
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        public List<ArticleDto> Get(string keySearch)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            return this.context.Articles
                .Where(x => keySearch == null || x.Title.Contains(keySearch) || x.Menu.Name.Contains(keySearch))
                .OrderBy(x => x.Index)
                .Select(x => new ArticleDto()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Alias = x.Alias,
                    MenuId = x.MenuId,
                    Index = x.Index,
                    Active = x.Active,
                    Image = x.Image,
                    Menu = new MenuDto()
                    {
                        Name = x.Menu.Name
                    },
                    Created = x.Created
                })
                .ToList();
        }

        /// <summary>
        /// Get tất cả bài viết
        /// </summary>
        /// <returns></returns>
        public List<ArticleDto> GetAll()
        {
            return this.context.Articles
                .Select(x => new ArticleDto()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Alias = x.Alias
                })
                .ToList();
        }

        /// <summary>
        /// Get các bài viết nỗi bật để hiển thị trên trang chủ
        /// </summary>
        /// <returns></returns>
        public List<ArticleDto> GetHighlight()
        {
            return this.context.Articles
                .Where(x => x.Active)
                .Where(x => x.Menu.Type == "bai-viet")
                .OrderBy(x => x.Index)
                .Select(x => new ArticleDto()
                {
                    Title = x.Title,
                    Alias = x.Alias,
                    Image = x.Image
                })
                .Take(5)
                .ToList();
        }

        /// <summary>
        /// Get bài viết theo alias của menu
        /// </summary>
        /// <param name="menuAlias">Alias của menu</param>
        /// <param name="take">Số lượng bài viết</param>
        /// <returns></returns>
        public MenuDto GetByMenu(string menuAlias, int take = 30)
        {
            MenuDto menu = this.context.Menus
                   .Where(x => x.Alias == menuAlias)
                   .Select(x => new MenuDto()
                   {
                       Id = x.Id,
                       Name = x.Name,
                       Alias = x.Alias
                   }).FirstOrDefault();

            menu.Articles = this.context.Articles
                .Where(x => x.Active)
                .Where(x => x.Menu.Alias == menuAlias)
                .OrderBy(x => x.Index)
                .Select(x => new ArticleDto()
                {
                    Title = x.Title,
                    Alias = x.Alias,
                    Image = x.Image
                })
                .Take(take)
                .ToList();

            return menu;
        }

        /// <summary>
        /// Get bài viết theo id
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ArticleDto GetById(int key)
        {
            return this.context.Articles
                .Where(x => x.Id == key)
               .Select(x => new ArticleDto()
               {
                   Id = x.Id,
                   Title = x.Title,
                   Alias = x.Alias,
                   Active = x.Active,
                   ShortDescription = x.ShortDescription,
                   Description = x.Description,
                   Created = x.Created,
                   Image = x.Image,
                   Index = x.Index,
                   MenuId = x.MenuId
               })
               .FirstOrDefault();
        }

        /// <summary>
        /// Get bài viết theo alias
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public ArticleDto GetByAlias(string alias)
        {
            return this.context.Articles
               .Where(x => x.Alias == alias)
               .Select(x => new ArticleDto()
               {
                   Menu = new MenuDto()
                   {
                       Alias = x.Menu.Alias
                   },
                   Title = x.Title,
                   Alias = x.Alias,
                   ShortDescription = x.ShortDescription,
                   Description = x.Description,
                   Image = x.Image,
                   Created = x.Created
               })
               .FirstOrDefault();
        }

        /// <summary>
        /// Thêm mới bài viết
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ArticleDto Insert(ArticleDto entity)
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
            Article article = new Article()
            {
                Active = entity.Active,
                Alias = "",
                Created = DateTime.Now,
                Description = entity.Description,
                Image = entity.Image,
                Index = entity.Index,
                MenuId = entity.MenuId,
                ShortDescription = entity.ShortDescription,
                Title = entity.Title,
            };

            this.context.Articles.Add(article);
            this.context.SaveChanges();
            article.Alias = entity.Alias + "-" + article.Id;

            this.context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Cập nhật bài viết
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        public void Update(int key, ArticleDto entity)
        {
            Article article = this.context.Articles
                .FirstOrDefault(x => x.Id == key);

            if (article != null)
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

                article.Title = entity.Title;

                if (article.Alias != entity.Alias)
                    article.Alias = entity.Alias + "-" + entity.Id;

                article.Active = entity.Active;
                article.ShortDescription = entity.ShortDescription;
                article.Description = entity.Description;
                article.Image = entity.Image;
                article.Index = entity.Index;
                article.MenuId = entity.MenuId;

                this.context.SaveChanges();
            }
        }
    }
}
