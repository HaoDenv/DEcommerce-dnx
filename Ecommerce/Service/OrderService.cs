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
    public class OrderService : IServiceBase<OrderDto, int>
    {
        protected readonly MyContext context;
        public OrderService(MyContext context)
        {
            this.context = context;
        }

        public void DeleteById(int key, string userSession = null)
        {
            throw new NotImplementedException();
        }

        public List<OrderDto> Get(string keySearch, int status, DateTime? fDate, DateTime? tDate)
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

        /// <summary>
        /// Get danh sách đơn hàng chờ xử lý
        /// </summary>
        /// <param name="keySearch"></param>
        /// <param name="status"></param>
        /// <param name="fDate"></param>
        /// <param name="tDate"></param>
        /// <returns></returns>
        public List<OrderDto> GetWIP(string keySearch, int status, DateTime? fDate, DateTime? tDate)
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
                .Where(x => x.Status != Constants.OrderStatus.DA_HUY && x.Status != Constants.OrderStatus.DA_GIAO)
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

        public List<OrderDto> GetAll()
        {
            return this.context.Orders
                .OrderByDescending(x => x.Created)
                .Select(x => new OrderDto()
                {
                    Id = x.Id,
                    Created = x.Created,
                    Customer = new CustomerDto()
                    {
                        Address = x.Customer.Address,
                        Email = x.Customer.Email,
                        FullName = x.Customer.FullName,
                        PhoneNumber = x.Customer.PhoneNumber,
                    },
                    Note = x.Note,
                    Status = x.Status,
                    TotalAmount = x.TotalAmount,
                })
                .ToList();
        }

        public OrderDto GetById(int key)
        {
            return this.context.Orders
                .Where(x => x.Id == key)
                .Select(x => new OrderDto()
                {
                    Id = x.Id,
                    Created = x.Created,
                    Customer = new CustomerDto()
                    {
                        Address = x.Customer.Address,
                        Email = x.Customer.Email,
                        FullName = x.Customer.FullName,
                        PhoneNumber = x.Customer.PhoneNumber,
                    },
                    Note = x.Note,
                    Status = x.Status,
                    TotalAmount = x.TotalAmount,
                    OrderDetails = x.OrderDetails.Select(y => new OrderDetailDto()
                    {
                        Id = y.Id,
                        OrderId = y.OrderId,
                        ProductDiscountPrice = y.ProductDiscountPrice,
                        ProductImage = y.ProductImage,
                        ProductName = y.ProductName,
                        ProductPrice = y.ProductPrice,
                        Qty = y.Qty,
                        Attribute = y.Attribute
                    }).ToList()
                })
                .FirstOrDefault();
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng
        /// </summary>
        /// <param name="key"></param>
        /// <param name="status"></param>
        public void ChangeStatus(int key, int status)
        {
            Order order = this.context.Orders.FirstOrDefault(x => x.Id == key);

            order.Status = status;
            this.context.SaveChanges();
        }

        public OrderDto Insert(OrderDto entity)
        {
            DateTime dateNow = DateTime.Now;
            Order order = new Order()
            {
                CustomerCode = entity.CustomerCode,
                PhoneNumber = entity.Customer.PhoneNumber,
                Address = entity.Customer.Address,
                Note = entity.Note,
                Status = Constants.OrderStatus.CHO_XAC_NHAN,
                Created = dateNow,
            };

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            entity.OrderDetails.ForEach(x =>
            {
                Product product = context.Products.FirstOrDefault(y => y.Id == x.ProductId);

                orderDetails.Add(new OrderDetail()
                {
                    ProductId = product.Id,
                    ProductDiscountPrice = product.DiscountPrice,
                    ProductPrice = product.Price,
                    ProductImage = product.Image,
                    ProductName = product.Name,
                    Qty = x.Qty,
                    Attribute = x.Attribute?.Trim()
                });
            });

            order.OrderDetails = orderDetails;
            order.TotalAmount = orderDetails.Sum(x => x.Qty * x.ProductDiscountPrice);

            this.context.Orders.Add(order);
            this.context.SaveChanges();

            // gửi mail đặt hàng thành công
            try
            {
                string orderDetail = @"<table>
                                                <thead>
                                                    <tr style='background: #5189B9;color: white;'>
                                                        <th>Tên sản phẩm</th>
                                                        <th>Số lượng</th>
                                                        <th>Đơn giá</th>
                                                        <th>Thành tiền</th>
                                                    </tr>
                                                </thead>
                                                <tbody>{0}<tr><td colSpan='3'>Tổng tiền:</td><td>{1}</td></tr></tbody>
                                        </table>";

                StringBuilder sbOrderDetail = new StringBuilder();
                orderDetails.ForEach(x =>
                {
                    sbOrderDetail.Append($"<tr><td>{x.ProductName}</td><td>{x.Qty}</td><td>{DataHelper.ToCurrency(x.ProductDiscountPrice)}</td><td>{DataHelper.ToCurrency(x.ProductDiscountPrice * x.Qty)}</td></tr>");
                });

                string htmlBody = @$"
                        <html lang=""en"">
                            <head>    
                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
                                <title>
                                    DEcommerce
                                </title>
                                <style type=""text/css"">
                                   table th,
                                    table td {{
                                        border: 1px solid #dcdcdc;
                                        padding: 2px 10px;
                                    }}

                                    table {{
                                        border-collapse: collapse;
                                    }}
                                </style>
                            </head>
                            <body>
                                 {string.Format(orderDetail, sbOrderDetail.ToString(), DataHelper.ToCurrency(order.TotalAmount))}
                            </body>
                        </html>
                        ";
                // Gửi mail cho khách hàng
                EmailConfiguration emailConfiguration = this.context.EmailConfigurations.FirstOrDefault();
                Customer customer = this.context.Customers.FirstOrDefault(x => x.Code == entity.CustomerCode);
                if (customer != null && !string.IsNullOrWhiteSpace(customer.Email))
                {
                    EmailTemplate emailTemplateOrderSuccessful = this.context.EmailTemplates.FirstOrDefault(x => x.Id == 3);

                    string bodyMail = emailTemplateOrderSuccessful.Content
                        .Replace(Constants.EmailKeyGuide.OrderCode, order.Id.ToString())
                        .Replace(Constants.EmailKeyGuide.OrderDate, dateNow.ToString("HH:mm dd-MM-yyyy"))
                        .Replace(Constants.EmailKeyGuide.Customer, customer.FullName)
                        .Replace(Constants.EmailKeyGuide.Address, entity.Customer.Address)
                        .Replace(Constants.EmailKeyGuide.OrderDetail, htmlBody);

                    DataHelper.SendMail(emailConfiguration, emailTemplateOrderSuccessful.Subject, bodyMail, new List<string>()
                    {
                        customer.Email
                    }, emailTemplateOrderSuccessful.CC?.Split(';').ToList(), emailTemplateOrderSuccessful.BCC?.Split(';').ToList());
                }

                // gửi mail cho quản trị

                {
                    EmailTemplate emailTemplateNotifyNewOrder = this.context.EmailTemplates.FirstOrDefault(x => x.Id == 4);
                    string bodyMail = emailTemplateNotifyNewOrder.Content
                            .Replace(Constants.EmailKeyGuide.OrderCode, order.Id.ToString())
                            .Replace(Constants.EmailKeyGuide.OrderDate, dateNow.ToString("HH:mm dd-MM-yyyy"))
                            .Replace(Constants.EmailKeyGuide.Customer, customer.FullName)
                            .Replace(Constants.EmailKeyGuide.Address, entity.Customer.Address)
                            .Replace(Constants.EmailKeyGuide.OrderDetail, htmlBody);

                    DataHelper.SendMail(emailConfiguration, emailTemplateNotifyNewOrder.Subject, bodyMail,
                        emailTemplateNotifyNewOrder.CC?.Split(';').ToList(),
                        emailTemplateNotifyNewOrder.CC?.Split(';').ToList(),
                        emailTemplateNotifyNewOrder.BCC?.Split(';').ToList());
                }

            }
            catch (Exception ex)
            {
            }

            return entity;
        }

        public void Update(int key, OrderDto entity)
        {
            Order order = context.Orders
               .FirstOrDefault(x => x.Id == key);

            order.Status = entity.Status;
            order.Note = entity.Note;

            this.context.SaveChanges();
        }

    }
}
