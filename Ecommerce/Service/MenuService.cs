using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Dto;
using Ecommerce.Model;
using Ecommerce.Util;

namespace Ecommerce.Service
{
    public class MenuService : IServiceBase<MenuDto, int>
    {
        protected readonly MyContext context;
        public MenuService(MyContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Xóa danh mục menu
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userSession"></param>
        public void DeleteById(int key, string userSession = null)
        {
            if (this.context.Articles.Any(x => x.MenuId == key) ||
                this.context.Products.Any(x => x.MenuId == key) ||
                this.context.Menus.Any(x => x.ParentMenu == key))
                throw new ArgumentException("Dữ liệu đang được sử dụng");

            Menu menu = this.context.Menus.FirstOrDefault(x => x.Id == key);

            this.context.Menus.Remove(menu);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Get tất cả menu
        /// </summary>
        /// <returns></returns>
        public List<MenuDto> GetAll()
        {
            List<MenuDto> menuDtos = this.context.Menus
                .OrderBy(x => x.Index)
                .ToList()
                .Select(x => new MenuDto()
                {
                    Active = x.Active,
                    Alias = x.Alias,
                    Id = x.Id,
                    Index = x.Index,
                    Name = x.Name,
                    ParentMenu = x.ParentMenu,
                    PMenu = x.PMenu == null ? null : new MenuDto()
                    {
                        Id = x.PMenu.Id,
                        Name = x.PMenu?.Name
                    },
                    ShowHomePage = x.ShowHomePage,
                    Type = x.Type
                }).ToList();

            return menuDtos;
        }

        /// <summary>
        /// Get danh sách menu chính theo từ khóa
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        public List<MenuDto> GetMainMenu(string keySearch)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            List<MenuDto> menuDtos = this.context.Menus
                .OrderBy(x => x.Index)
                .Where(x => keySearch == null || x.Name.Contains(keySearch))
                .Where(x => x.Group == "main")
                .ToList()
                .Select(x => new MenuDto()
                {
                    Active = x.Active,
                    Alias = x.Alias,
                    Id = x.Id,
                    Index = x.Index,
                    Name = x.Name,
                    ParentMenu = x.ParentMenu,
                    PMenu = x.PMenu == null ? null : new MenuDto()
                    {
                        Id = x.PMenu.Id,
                        Name = x.PMenu?.Name
                    },
                    ShowHomePage = x.ShowHomePage,
                    Type = x.Type
                }).ToList();

            return menuDtos;
        }

        /// <summary>
        /// Get danh sách menu phụ theo từ khóa
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        public List<MenuDto> GetSubMenu(string keySearch)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            List<MenuDto> menuDtos = this.context.Menus
                .OrderBy(x => x.Index)
                .Where(x => keySearch == null || x.Name.Contains(keySearch))
                .Where(x => x.Group == "sub")
                .ToList()
                .Select(x => new MenuDto()
                {
                    Active = x.Active,
                    Alias = x.Alias,
                    Id = x.Id,
                    Index = x.Index,
                    Name = x.Name,
                    ParentMenu = x.ParentMenu,
                    PMenu = x.PMenu == null ? null : new MenuDto()
                    {
                        Id = x.PMenu.Id,
                        Name = x.PMenu?.Name
                    },
                    ShowHomePage = x.ShowHomePage,
                    Type = x.Type
                }).ToList();

            return menuDtos;
        }

        /// <summary>
        /// Get tất cả danh sách menu chính được active
        /// </summary>
        /// <returns></returns>
        public List<MenuDto> GetMainMenuActive()
        {
            List<MenuDto> menuDtos = this.context.Menus
                .Where(x => x.Active)
                .Where(x => x.Group == "main")
                .OrderBy(x => x.Index)
                .Select(x => new MenuDto()
                {
                    Alias = x.Alias,
                    Id = x.Id,
                    Index = x.Index,
                    Name = x.Name,
                    ParentMenu = x.ParentMenu,
                    ShowHomePage = x.ShowHomePage,
                    Type = x.Type,
                }).ToList();

            List<MenuDto> parentMenu = menuDtos.FindAll(x => x.ParentMenu == null);
            parentMenu.ForEach(x =>
            {
                x.SubMenus = menuDtos.FindAll(y => y.ParentMenu == x.Id);
            });
            return parentMenu;
        }

        /// <summary>
        /// Get tất cả danh sách menu phụ được active
        /// </summary>
        /// <returns></returns>
        public List<MenuDto> GetSubMenuActive()
        {
            List<MenuDto> menuDtos = this.context.Menus
                .Where(x => x.Active)
                .Where(x => x.Group == "sub")
                .OrderBy(x => x.Index)
                .Select(x => new MenuDto()
                {
                    Alias = x.Alias,
                    Id = x.Id,
                    Index = x.Index,
                    Name = x.Name,
                    ParentMenu = x.ParentMenu,
                    ShowHomePage = x.ShowHomePage,
                    Type = x.Type,
                    Articles = x.Articles.Select(y => new ArticleDto()
                    {
                        Alias = y.Alias
                    }).ToList(),
                }).ToList();

            List<MenuDto> parentMenu = menuDtos.FindAll(x => x.ParentMenu == null);
            parentMenu.ForEach(x =>
            {
                x.SubMenus = menuDtos.FindAll(y => y.ParentMenu == x.Id);
            });
            return parentMenu;
        }

        /// <summary>
        /// Get tất cả danh sách menu cha chính
        /// </summary>
        /// <returns></returns>
        public List<MenuDto> GetParentMainMenu()
        {
            return this.context.Menus
                 .Where(x => x.ParentMenu == null)
                .Where(x => x.Group == "main")
                 .OrderBy(x => x.Index)
                 .Select(x => new MenuDto()
                 {
                     Id = x.Id,
                     Name = x.Name,
                 }).ToList();
        }

        /// <summary>
        /// Get tất cả danh sách menu cha phụ
        /// </summary>
        /// <returns></returns>
        public List<MenuDto> GetParentSubMenu()
        {
            return this.context.Menus
                 .Where(x => x.ParentMenu == null)
                .Where(x => x.Group == "sub")
                 .OrderBy(x => x.Index)
                 .Select(x => new MenuDto()
                 {
                     Id = x.Id,
                     Name = x.Name,
                 }).ToList();
        }

        /// <summary>
        /// Get danh sách menu theo loại
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public List<MenuDto> GetByType(List<string> types)
        {
            return this.context.Menus
                .Where(x => types.Any(y => y == x.Type))
                 .OrderBy(x => x.Index)
                 .Select(x => new MenuDto()
                 {
                     Id = x.Id,
                     Name = x.Name,
                     ParentMenu = x.ParentMenu
                 }).ToList();
        }

        /// <summary>
        /// Get tất cả menu hiển thị trang chủ
        /// </summary>
        /// <returns></returns>
        public List<MenuDto> GetAllShowHomePage()
        {
            List<MenuDto> menuDtos = this.context.Menus
                .Where(x => x.Active && x.ShowHomePage == true)
                .OrderBy(x => x.Index)
                .Select(x => new MenuDto()
                {
                    Alias = x.Alias,
                    Name = x.Name,
                    Products = x.Products
                        .Where(y => y.Status == 10)
                        .OrderBy(y => y.Index)
                        .Select(y => new ProductDto()
                        {
                            Id = y.Id,
                            Alias = y.Alias,
                            Name = y.Name,
                            Price = y.Price,
                            DiscountPrice = y.DiscountPrice,
                            Image = y.Image
                        })
                        .Take(10)
                        .ToList()
                }).ToList();

            return menuDtos;
        }

        public MenuDto GetById(int key)
        {
            return this.context.Menus
                .Where(x => x.Id == key)
                .Select(x => new MenuDto()
                {
                    Active = x.Active,
                    Alias = x.Alias,
                    Id = x.Id,
                    Index = x.Index,
                    Name = x.Name,
                    ParentMenu = x.ParentMenu,
                    ShowHomePage = x.ShowHomePage,
                    Type = x.Type,
                }).FirstOrDefault();
        }

        /// <summary>
        /// Get menu theo alias
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public MenuDto GetByAlias(string alias)
        {
            return this.context.Menus
                .Where(x => x.Alias == alias)
                .Select(x => new MenuDto()
                {
                    Alias = x.Alias,
                    Id = x.Id,
                    Name = x.Name,
                }).FirstOrDefault();
        }

        public MenuDto Insert(MenuDto entity)
        {
            Menu menu = new Menu()
            {
                Active = entity.Active,
                Alias = "",
                Index = entity.Index,
                Group = entity.Group,
                Name = entity.Name,
                ParentMenu = entity.ParentMenu,
                ShowHomePage = entity.ShowHomePage,
                Type = entity.Type,
            };

            this.context.Menus.Add(menu);
            this.context.SaveChanges();
            menu.Alias = entity.Alias + "-" + menu.Id;
            this.context.SaveChanges();

            return entity;
        }

        public void Update(int key, MenuDto entity)
        {
            Menu menu = this.context.Menus.FirstOrDefault(x => x.Id == key);

            menu.ParentMenu = entity.ParentMenu;
            menu.Name = entity.Name;
            if (menu.Alias != entity.Alias)
                menu.Alias = entity.Alias + "-" + key;
            menu.Index = entity.Index;
            menu.ShowHomePage = entity.ShowHomePage;
            menu.Type = entity.Type;
            menu.Active = entity.Active;

            this.context.SaveChanges();
        }
    }
}