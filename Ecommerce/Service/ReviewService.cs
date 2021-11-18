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
    public class ReviewService : IServiceBase<ReviewDto, int>
    {
        protected readonly MyContext context;
        public ReviewService(MyContext context)
        {
            this.context = context;
        }

        public void DeleteById(int key, string userSession = null)
        {
            throw new NotImplementedException();
        }

        public List<ReviewDto> Get(string keySearch, int status)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            return this.context.Reviews
                .Where(x => keySearch == null || x.Content.Contains(keySearch) ||
                            x.CreatedBy.Contains(keySearch) || x.Product.Name.Contains(keySearch))
                .Where(x => status < 0 || x.Status == status)
                .OrderByDescending(x => x.Created)
                .Select(x => new ReviewDto()
                {
                    Id = x.Id,
                    CreatedBy = x.CreatedBy,
                    Content = x.Content,
                    Created = x.Created,
                    Star = x.Star,
                    Status = x.Status,
                    Product = new ProductDto()
                    {
                        Image = x.Product.Image,
                        Name = x.Product.Name,
                        Alias = x.Product.Alias
                    }
                }).ToList();
        }

        public void UpdateStatus(int id, int status)
        {
            Review review = this.context.Reviews.FirstOrDefault(x => x.Id == id);

            if (review != null)
            {
                review.Status = status;

                this.context.SaveChanges();
            }
        }

        public List<ReviewDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public ReviewDto GetById(int key)
        {
            throw new NotImplementedException();
        }

        public ReviewDto GetByOrder(int orderDetailId)
        {
            return this.context.Reviews
                .Where(x => x.OrderDetailId == orderDetailId)
                .Select(x => new ReviewDto()
                {
                    Id = x.Id,
                    Content = x.Content,
                    Created = x.Created,
                    CreatedBy = x.CreatedBy,
                    Star = x.Star,
                    Status = x.Status
                }).FirstOrDefault();
        }

        public ReviewDto Insert(ReviewDto entity)
        {
            throw new NotImplementedException();
        }

        public void Insert(int orderDetailId, int rate, string comment)
        {
            if (this.context.OrderDetails
                .Any(x => x.Id == orderDetailId && x.Order.Status == Constants.OrderStatus.DA_GIAO))
            {
                Review review = this.context.Reviews.FirstOrDefault(x => x.OrderDetailId == orderDetailId);

                if (review == null)
                {
                    OrderDetail orderDetail = this.context.OrderDetails.FirstOrDefault(x => x.Id == orderDetailId);
                    review = new Review()
                    {
                        Status = Constants.ReviewStatus.CHO_DUYET,
                        Content = comment,
                        Created = DateTime.Now,
                        CreatedBy = orderDetail.Order.Customer.FullName,
                        OrderDetailId = orderDetailId,
                        ProductId = orderDetail.ProductId,
                        Star = rate,
                    };

                    this.context.Reviews.Add(review);
                }
                else
                {
                    if (review.Star != rate || review.Content != comment)
                    {
                        review.Star = rate;
                        review.Content = comment;
                        if (review.Status == Constants.ReviewStatus.DA_DUYET)
                            review.Status = Constants.ReviewStatus.CAP_NHAT_LAI_CHO_DUYET;
                    }
                }
                this.context.SaveChanges();
            }
        }

        public void Update(int key, ReviewDto entity)
        {
            throw new NotImplementedException();
        }
    }
}
