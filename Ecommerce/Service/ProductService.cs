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
    public class ProductService : IServiceBase<ProductDto, int>
    {
        protected readonly MyContext context;
        protected IWebHostEnvironment hostEnvironment;
        public ProductService(MyContext context, IWebHostEnvironment hostEnvironment)
        {
            this.context = context;
            this.hostEnvironment = hostEnvironment;
        }

        public void DeleteById(int key, string userSession = null)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                Product product = this.context.Products.FirstOrDefault(x => x.Id == key);

                this.context.ProductAttributes.RemoveRange(product.ProductAttributes);
                this.context.ProductImages.RemoveRange(product.ProductImages);
                this.context.ProductRelateds.RemoveRange(product.ProductRelateds);
                this.context.Reviews.RemoveRange(product.Reviews);
                this.context.Products.Remove(product);

                this.context.SaveChanges();
                transaction.Commit();
            }
        }

        public List<ProductDto> Get(string keySearch, int? menuId)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            return this.context.Products
                .Where(x => keySearch == null || x.Name.Contains(keySearch))
                .Where(x => menuId == null || x.MenuId == menuId)
                .OrderBy(x => x.Menu.Index)
                .ThenBy(x => x.Index)
                .Select(x => new ProductDto()
                {
                    Id = x.Id,
                    Alias = x.Alias,
                    Description = x.Description,
                    DiscountPrice = x.DiscountPrice,
                    Selling = x.Selling,
                    Image = x.Image,
                    Index = x.Index,
                    MenuId = x.MenuId,
                    Price = x.Price,
                    Name = x.Name,
                    ShortDescription = x.ShortDescription,
                    Status = x.Status,
                    Menu = x.Menu == null ? null : new MenuDto()
                    {
                        Name = x.Menu.Name
                    }
                })
                .ToList();
        }

        public List<ProductDto> GetAll()
        {
            return this.context.Products
                .Select(x => new ProductDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = x.Image
                })
                .ToList();
        }

        /// <summary>
        /// Get danh sách sản phẩm bán chạy hiển thị trên trang homepage
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<ProductDto> GetProductSelling(int number = 10)
        {
            List<ProductDto> products = this.context.Products
                .Where(x => x.Status == 10)
                .Where(x => x.Selling == true)
                .OrderBy(x => x.Index)
                .Select(x => new ProductDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = x.Image,
                    Price = x.Price,
                    DiscountPrice = x.DiscountPrice,
                    Alias = x.Alias,
                    ProductAttributes = x.ProductAttributes.Select(y => new ProductAttributeDto()
                    {
                        Attribute = new AttributeDto()
                        {
                            Id = y.Attribute.Id,
                            Name = y.Attribute.Name
                        },
                        AttributeId = y.AttributeId,
                        Value = y.Value
                    }).ToList(),
                })
                .Take(number)
                .ToList();

            this.RestructureAttribute(products);
            return products;
        }

        public List<ProductDto> Search(string keySearch, int take, string orderBy = "", string price = "")
        {
            var query = this.context.Products
                .Where(x => x.Status == 10);

            if (!string.IsNullOrWhiteSpace(keySearch))
            {
                query = query.Where(x => x.Name.Contains(keySearch));
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case "highlight":
                        break;
                    case "price-asc":
                        query = query.OrderBy(x => x.DiscountPrice);
                        break;
                    case "price-desc":
                        query = query.OrderByDescending(x => x.DiscountPrice);
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(price))
            {
                switch (price)
                {
                    case "m30":
                        query = query.Where(x => x.DiscountPrice >= 30000000);
                        break;
                    case "f20t30":
                        query = query.Where(x => x.DiscountPrice >= 20000000 && x.DiscountPrice < 30000000);
                        break;
                    case "f10t20":
                        query = query.Where(x => x.DiscountPrice >= 10000000 && x.DiscountPrice < 20000000);
                        break;
                    case "f5t10":
                        query = query.Where(x => x.DiscountPrice >= 5000000 && x.DiscountPrice < 10000000);
                        break;
                    case "l5":
                        query = query.Where(x => x.DiscountPrice < 5000000);
                        break;
                }
            }

            return query
                .Select(x => new ProductDto()
                {
                    Alias = x.Alias,
                    DiscountPrice = x.DiscountPrice,
                    Image = x.Image,
                    Price = x.Price,
                    Name = x.Name
                })
                .Take(take)
                .ToList();
        }

        /// <summary>
        /// Get danh sách sản phẩm theo alias của danh mục menu
        /// </summary>
        /// <param name="menuAlias"></param>
        /// <param name="orderBy"></param>
        /// <param name="price"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public MenuDto GetByMenu(string menuAlias, string orderBy = "", string price = "", int take = 30)
        {
            MenuDto menu = this.context.Menus
                .Where(x => x.Alias == menuAlias)
                .Select(x => new MenuDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Alias = x.Alias
                }).FirstOrDefault();

            var query = this.context.Products
                .Where(x => x.Status == 10 && x.MenuId == menu.Id);

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case "highlight":
                        break;
                    case "price-asc":
                        query = query.OrderBy(x => x.DiscountPrice);
                        break;
                    case "price-desc":
                        query = query.OrderByDescending(x => x.DiscountPrice);
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(price))
            {
                switch (price)
                {
                    case "m30":
                        query = query.Where(x => x.DiscountPrice >= 30000000);
                        break;
                    case "f20t30":
                        query = query.Where(x => x.DiscountPrice >= 20000000 && x.DiscountPrice < 30000000);
                        break;
                    case "f10t20":
                        query = query.Where(x => x.DiscountPrice >= 10000000 && x.DiscountPrice < 20000000);
                        break;
                    case "f5t10":
                        query = query.Where(x => x.DiscountPrice >= 5000000 && x.DiscountPrice < 10000000);
                        break;
                    case "l5":
                        query = query.Where(x => x.DiscountPrice < 5000000);
                        break;
                }
            }

            List<ProductDto> products = query
                .Select(x => new ProductDto()
                {
                    Id = x.Id,
                    Alias = x.Alias,
                    DiscountPrice = x.DiscountPrice,
                    Image = x.Image,
                    Price = x.Price,
                    Name = x.Name,
                    ProductAttributes = x.ProductAttributes.Select(y => new ProductAttributeDto()
                    {
                        Attribute = new AttributeDto()
                        {
                            Id = y.Attribute.Id,
                            Name = y.Attribute.Name
                        },
                        AttributeId = y.AttributeId,
                        Value = y.Value
                    }).ToList(),
                })
                .Take(take)
                .ToList();

            this.RestructureAttribute(products);
            menu.Products = products;
            return menu;
        }

        public ProductDto GetById(int key)
        {
            return this.context.Products
                 .Where(x => x.Id == key)
                 .Select(x => new ProductDto()
                 {
                     Id = x.Id,
                     Alias = x.Alias,
                     Description = x.Description,
                     DiscountPrice = x.DiscountPrice,
                     Selling = x.Selling,
                     Image = x.Image,
                     Index = x.Index,
                     MenuId = x.MenuId,
                     Price = x.Price,
                     Name = x.Name,
                     ShortDescription = x.ShortDescription,
                     Status = x.Status,
                     ProductAttributes = x.ProductAttributes.Select(y => new ProductAttributeDto()
                     {
                         AttributeId = y.AttributeId,
                         Value = y.Value
                     }).ToList(),
                     ProductRelateds = x.ProductRelateds.Select(y => new ProductRelatedDto()
                     {
                         ProductRelatedId = y.ProductRelatedId
                     }).ToList(),
                     ProductImages = x.ProductImages.Select(y => new ProductImageDto()
                     {
                         Image = y.Image
                     }).ToList()
                 })
                 .FirstOrDefault();
        }

        public ProductDto GetByAlias(string alias)
        {
            ProductDto product = this.context.Products
                .Where(x => x.Alias == alias)
                .Select(x => new ProductDto()
                {
                    Id = x.Id,
                    Alias = x.Alias,
                    Description = x.Description,
                    DiscountPrice = x.DiscountPrice,
                    Selling = x.Selling,
                    Image = x.Image,
                    Index = x.Index,
                    MenuId = x.MenuId,
                    Price = x.Price,
                    Name = x.Name,
                    ShortDescription = x.ShortDescription,
                    Status = x.Status,
                    Menu = new MenuDto()
                    {
                        Name = x.Name
                    },
                    ProductAttributes = x.ProductAttributes.Select(y => new ProductAttributeDto()
                    {
                        Attribute = new AttributeDto()
                        {
                            Id = y.Attribute.Id,
                            Name = y.Attribute.Name
                        },
                        AttributeId = y.AttributeId,
                        Value = y.Value
                    }).ToList(),
                    ProductRelateds = x.ProductRelateds.Select(y => new ProductRelatedDto()
                    {
                        ProductRelatedId = y.ProductRelatedId
                    })
                    .Take(5)
                    .ToList(),
                    ProductImages = x.ProductImages.Select(y => new ProductImageDto()
                    {
                        Image = y.Image
                    }).ToList(),
                    Reviews = x.Reviews.OrderByDescending(y => y.Created)
                        .Where(y => y.Status == Constants.ReviewStatus.DA_DUYET).Select(y => new ReviewDto()
                        {
                            Content = y.Content,
                            Created = y.Created,
                            CreatedBy = y.CreatedBy,
                            Status = y.Status,
                            Star = y.Star,
                        }).ToList()
                })
                .FirstOrDefault();

            this.RestructureAttribute(new List<ProductDto>() { product });

            product.ProductRelateds.ForEach(x =>
            {
                x.Product = this.context.Products.Where(y => y.Id == x.ProductRelatedId)
                    .Select(y => new ProductDto()
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Alias = y.Alias,
                        Image = y.Image,
                        Price = y.Price,
                        DiscountPrice = y.DiscountPrice
                    }).FirstOrDefault();
            });
            if (product.Reviews.Count > 0)
            {
                product.RateAvg = Math.Round((double)product.Reviews.Sum(x => x.Star) / product.Reviews.Count, 1);
            }
            return product;
        }

        public ProductDto Insert(ProductDto entity)
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

            Product product = new Product()
            {
                Price = entity.Price,
                Name = entity.Name,
                MenuId = entity.MenuId,
                Index = entity.Index,
                Image = entity.Image,
                DiscountPrice = entity.DiscountPrice,
                Selling = entity.Selling,
                Description = entity.Description,
                Alias = "",
                ShortDescription = entity.ShortDescription,
                Status = entity.Status
            };

            if (entity.ProductImages != null && entity.ProductImages.Count > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (var item in entity.ProductImages)
                {
                    if (!string.IsNullOrWhiteSpace(item.Image))
                    {
                        if (item.Image.Contains("data:image/png;base64,"))
                        {
                            string path = Path.Combine(this.hostEnvironment.ContentRootPath, $"Resources/Images");
                            string imgName = Guid.NewGuid().ToString("N") + ".png";
                            var bytes = Convert.FromBase64String(item.Image.Replace("data:image/png;base64,", ""));
                            using (var imageFile = new FileStream(path + "/" + imgName, FileMode.Create))
                            {
                                imageFile.Write(bytes, 0, bytes.Length);
                                imageFile.Flush();
                            }

                            item.Image = imgName;
                        }
                        productImages.Add(new ProductImage()
                        {
                            Image = item.Image
                        });
                    }
                }

                product.ProductImages = productImages;
            }
            if (entity.ProductAttributes != null && entity.ProductAttributes.Count > 0)
            {
                product.ProductAttributes = entity.ProductAttributes.Select(x => new ProductAttribute()
                {
                    AttributeId = x.AttributeId,
                    Value = x.Value
                }).ToList();
            }
            if (entity.ProductRelateds != null && entity.ProductRelateds.Count > 0)
            {
                product.ProductRelateds = entity.ProductRelateds.Select(x => new ProductRelated()
                {
                    ProductRelatedId = x.ProductRelatedId,
                }).ToList();
            }

            this.context.Products.Add(product);
            this.context.SaveChanges();
            product.Alias = entity.Alias + "-" + product.Id;

            this.context.SaveChanges();

            return entity;
        }

        public void Update(int key, ProductDto entity)
        {
            using (var transaction = this.context.Database.BeginTransaction())
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

                Product product = this.context.Products.FirstOrDefault(x => x.Id == key);

                product.MenuId = entity.MenuId;
                product.Name = entity.Name;

                if (product.Alias != entity.Alias)
                    product.Alias = entity.Alias + "-" + key;
                product.Image = entity.Image;
                product.Index = entity.Index;
                product.Status = entity.Status;
                product.Price = entity.Price;
                product.DiscountPrice = entity.DiscountPrice;
                product.Selling = entity.Selling;
                product.ShortDescription = entity.ShortDescription;
                product.Description = entity.Description;

                this.context.ProductAttributes.RemoveRange(product.ProductAttributes);
                this.context.ProductImages.RemoveRange(product.ProductImages);
                this.context.ProductRelateds.RemoveRange(product.ProductRelateds);

                if (entity.ProductAttributes != null && entity.ProductAttributes.Count > 0)
                {
                    this.context.ProductAttributes.AddRange(entity.ProductAttributes.Select(x => new ProductAttribute()
                    {
                        AttributeId = x.AttributeId,
                        ProductId = key,
                        Value = x.Value
                    }));
                }

                if (entity.ProductImages != null && entity.ProductImages.Count > 0)
                {
                    List<ProductImage> productImages = new List<ProductImage>();

                    foreach (var item in entity.ProductImages)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Image))
                        {
                            if (item.Image.Contains("data:image/png;base64,"))
                            {
                                string path = Path.Combine(this.hostEnvironment.ContentRootPath, $"Resources/Images");
                                string imgName = Guid.NewGuid().ToString("N") + ".png";
                                var bytes = Convert.FromBase64String(item.Image.Replace("data:image/png;base64,", ""));
                                using (var imageFile = new FileStream(path + "/" + imgName, FileMode.Create))
                                {
                                    imageFile.Write(bytes, 0, bytes.Length);
                                    imageFile.Flush();
                                }

                                item.Image = imgName;
                            }
                            productImages.Add(new ProductImage()
                            {
                                ProductId = key,
                                Image = item.Image
                            });
                        }
                    }

                    this.context.ProductImages.AddRange(productImages);
                }

                if (entity.ProductRelateds != null && entity.ProductRelateds.Count > 0)
                {
                    this.context.ProductRelateds.AddRange(entity.ProductRelateds.Select(x => new ProductRelated()
                    {
                        ProductId = key,
                        ProductRelatedId = x.ProductRelatedId
                    }));
                }

                this.context.SaveChanges();
                transaction.Commit();
            }
        }

        /// <summary>
        /// Cấu trúc lại thông tin thuộc tính của model sản phẩm
        /// </summary>
        /// <param name="products"></param>
        private void RestructureAttribute(List<ProductDto> products)
        {
            foreach (var product in products)
            {
                product.Attributes = product.ProductAttributes.Select(x => new AttributeDto()
                {
                    Name = x.Attribute.Name,
                    Id = x.Attribute.Id
                }).Distinct().ToList();

                product.Attributes.ForEach(x =>
                {
                    x.ProductAttributes = product.ProductAttributes
                        .Where(y => y.AttributeId == x.Id)
                        .Select(y => y.Value)
                        .FirstOrDefault()?.Split(',')
                        .Select(y => new ProductAttributeDto()
                        {
                            Value = y,
                        }).ToList() ?? new List<ProductAttributeDto>();
                });
                product.ProductAttributes = null;
            };
        }
    }
}
