using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Dto;

namespace Ecommerce.Service
{
    public class AttributeService : IServiceBase<AttributeDto, int>
    {
        protected readonly MyContext context;
        public AttributeService(MyContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Xóa thuộc tính
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userSession"></param>
        public void DeleteById(int key, string userSession = null)
        {
            if (this.context.ProductAttributes.Any(x => x.AttributeId == key))
                throw new ArgumentException("Dữ liệu đang được sử dụng");

            Model.Attribute attribute = this.context.Attributes.FirstOrDefault(x => x.Id == key);

            if (attribute != null)
            {
                this.context.Attributes.Remove(attribute);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Get thuộc tính theo từ khóa
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        public List<AttributeDto> Get(string keySearch)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;
            return this.context.Attributes
                .Where(x => keySearch == null || x.Name.Contains(keySearch))
                .Select(x => new AttributeDto()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
        }

        /// <summary>
        /// Get tất cả thuộc tính
        /// </summary>
        /// <returns></returns>
        public List<AttributeDto> GetAll()
        {
            return this.context.Attributes.Select(x => new AttributeDto()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }

        /// <summary>
        /// Get thuộc tính theo id
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AttributeDto GetById(int key)
        {
            return this.context.Attributes
                 .Where(x => x.Id == key)
                 .Select(x => new AttributeDto()
                 {
                     Id = x.Id,
                     Name = x.Name,
                 }).FirstOrDefault();
        }

        /// <summary>
        /// Thêm mới thuộc tính
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public AttributeDto Insert(AttributeDto entity)
        {
            Model.Attribute attribute = new Model.Attribute()
            {
                Name = entity.Name
            };
            this.context.Attributes.Add(attribute);
            this.context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Cập nhật thuộc tính
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        public void Update(int key, AttributeDto entity)
        {
            Model.Attribute attribute = this.context.Attributes.FirstOrDefault(x => x.Id == key);

            if (attribute != null)
            {
                attribute.Name = entity.Name;

                this.context.SaveChanges();
            }
        }
    }
}