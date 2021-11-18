using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Dto;
using Ecommerce.Util;

namespace Ecommerce.Service
{
    public class ReportService
    {
        protected readonly MyContext context;
        public ReportService(MyContext context)
        {
            this.context = context;
        }

        public ReportHighlight GetHighlight(DateTime? date)
        {
            if (!date.HasValue)
                date = DateTime.Now;

            ReportHighlight highlight = new ReportHighlight();
            highlight.TotalNewOrder = this.context.Orders
                .Where(x => x.Created.Day == date.Value.Day &&
                            x.Created.Month == date.Value.Month &&
                            x.Created.Year == date.Value.Year)
                .Count();

            highlight.DailySales = this.context.Orders
                .Where(x => x.Created.Day == date.Value.Day &&
                            x.Created.Month == date.Value.Month &&
                            x.Created.Year == date.Value.Year)
                .Sum(x => x.TotalAmount == null ? 0 : x.TotalAmount) ?? 0;

            highlight.TotalOrder = this.context.Orders
               .Where(x => x.Created.Month == date.Value.Month &&
                           x.Created.Year == date.Value.Year)
               .Count();

            highlight.SalesRevenue = this.context.Orders
               .Where(x => x.Created.Month == date.Value.Month &&
                           x.Created.Year == date.Value.Year)
                .Sum(x => x.TotalAmount == null ? 0 : x.TotalAmount) ?? 0;

            highlight.OrderQty = new List<int>();
            highlight.OrderQtyByStatus = new List<int>();
            highlight.Revenues = new List<double>();

            DateTime dateNow = DateTime.Now;

            for (int i = 1; i < 13; i++)
            {
                int totalOrder = this.context.Orders
                            .Where(x => x.Created.Month == i &&
                                        x.Created.Year == date.Value.Year)
                            .Count();

                double totalAmount = this.context.Orders
                            .Where(x => x.Created.Month == i &&
                                        x.Created.Year == date.Value.Year)
                            .Sum(x => x.TotalAmount == null ? 0 : x.TotalAmount) ?? 0;

                if ((date.Value.Year == dateNow.Year && dateNow.Month >= i) || date.Value.Year != dateNow.Year)
                {
                    highlight.OrderQty.Add(totalOrder);
                }
                highlight.Revenues.Add(totalAmount);
            }

            new List<int>()
            {
                Constants.OrderStatus.CHO_XAC_NHAN,
                Constants.OrderStatus.DA_XAC_NHAN,
                Constants.OrderStatus.DANG_VAN_CHUYEN,
                Constants.OrderStatus.DA_GIAO,
                Constants.OrderStatus.DA_HUY,
            }.ForEach(status =>
            {
                highlight.OrderQtyByStatus.Add(this.context.Orders
                        .Where(x => x.Status == status)
                        .Where(x => x.Created.Month == date.Value.Month &&
                                    x.Created.Year == date.Value.Year)
                        .Count());
            });

            return highlight;
        }

        public List<OrderDto> GetGeneralReport(string keySearch, int status, DateTime? fDate, DateTime? tDate)
        {
            var query = this.context.Orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keySearch))
                query = query.Where(x => x.Id.ToString().Contains(keySearch) || x.Customer.FullName.Contains(keySearch));

            if (status > 0)
                query = query.Where(x => status < 0 || x.Status == status);

            if (fDate.HasValue && tDate.HasValue)
            {
                DateTime fd = fDate.Value.Date;
                DateTime td = tDate.Value.AddDays(1).Date;
                query = query.Where(x => x.Created >= fd && x.Created < td);
            }

            return query
                .OrderByDescending(x => x.Created)
                .Select(x => new OrderDto()
                {
                    Id = x.Id,
                    Created = x.Created,
                    Address = x.Address,
                    PhoneNumber = x.PhoneNumber,
                    Customer = new CustomerDto()
                    {
                        FullName = x.Customer.FullName,
                    },
                    Note = x.Note,
                    Status = x.Status,
                    TotalAmount = x.TotalAmount,
                })
                .ToList();
        }

        public List<ProductDto> GetProductReport(string keySearch, DateTime? fDate, DateTime? tDate)
        {
            if (string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            List<ProductDto> productDtos = this.context.Products
                .Where(x => keySearch == null || x.Name.Contains(keySearch))
                .Select(x => new ProductDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = x.Image,
                    TotalQty = 0,
                    TotalAmount = 0
                }).ToList();

            foreach (var item in productDtos)
            {
                var query = this.context.OrderDetails
                    .Where(x => x.ProductId == item.Id);

                if (fDate.HasValue && tDate.HasValue)
                {
                    DateTime fd = fDate.Value.Date;
                    DateTime td = tDate.Value.AddDays(1).Date;
                    query = query.Where(x => x.Order.Created >= fd && x.Order.Created < td);
                }

                item.TotalQty = query.Sum(x => x.Qty);
                item.TotalAmount = query.Sum(x => x.Qty * x.ProductDiscountPrice);
            }

            return productDtos
                .OrderByDescending(x => x.TotalAmount)
                .ThenByDescending(x => x.TotalQty).ToList();
        }

        public object GetRevenueReport(DateTime? date)
        {
            if (!date.HasValue)
                date = DateTime.Now;

            List<double> revenues = new List<double>();
            List<int> orderQty = new List<int>();

            for (int i = 1; i < 13; i++)
            {
                int totalOrder = this.context.Orders
                            .Where(x => x.Created.Month == i &&
                                        x.Created.Year == date.Value.Year)
                            .Count();

                double totalAmount = this.context.Orders
                            .Where(x => x.Created.Month == i &&
                                        x.Created.Year == date.Value.Year)
                            .Sum(x => x.TotalAmount == null ? 0 : x.TotalAmount) ?? 0;

                orderQty.Add(totalOrder);
                revenues.Add(totalAmount);
            }

            return new
            {
                revenues,
                orderQty
            };
        }
    }
}
