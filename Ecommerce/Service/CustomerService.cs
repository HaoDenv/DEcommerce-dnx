using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Ecommerce.Dto;
using Ecommerce.Model;
using Ecommerce.Util;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Service
{
    public class CustomerService : IServiceBase<CustomerDto, string>
    {
        protected readonly MyContext context;
        public CustomerService(MyContext context)
        {
            this.context = context;
        }

         //<Summary>
        // Gửi yêu cầu OTP vào email khi đăng ký tài khoản
         //</Summary>
        //<param name="email"></param>
        public void RequestOTP(string email)
        {
            if (this.context.Customers.Any(x => x.Email == email))
                throw new ArgumentException("Email đã được sử dụng");
            EmailSignUp emailSignUp = this.context.EmailSignUps.FirstOrDefault(x => x.Email == email);

            string otp = new Random().Next(100000, 999999).ToString();
            if (emailSignUp == null)
            {
                emailSignUp = new EmailSignUp()
                {
                    Email = email,
                    OTP = otp
                };
                this.context.EmailSignUps.Add(emailSignUp);
            }
            else
            {
                emailSignUp.OTP = otp;
            }
            EmailConfiguration emailConfiguration = this.context.EmailConfigurations.FirstOrDefault();
            EmailTemplate emailTemplate = this.context.EmailTemplates.FirstOrDefault(x => x.Id == 6);

            string bodyMail = emailTemplate.Content.Replace(Constants.EmailKeyGuide.OTP, otp);
            DataHelper.SendMail(emailConfiguration, emailTemplate.Subject, bodyMail, new List<string>()
            {
                email
            }, emailTemplate.CC?.Split(';').ToList(), emailTemplate.BCC?.Split(';').ToList());
            this.context.SaveChanges();
        }

        //<summary>
        // Xác thực mã OTP khi đăng ký tài khoản
         //</summary>
        //<param name="email"></param>
        //<param name="otp"></param>
         //<returns></returns>
        public bool ConfirmOTP(string email, string otp)
        {
            return this.context.EmailSignUps.Any(x => x.Email == email && x.OTP == otp);
        }

        //<summary>
         //Gửi yêu cầu cấp lại mật khẩu
         //</summary>
        //<param name="email"></param>
        public void ForgotPassword(string email)
        {
            if (!this.context.Customers.Any(x => x.Email == email))
                throw new ArgumentException("Email chưa được đăng ký sử dụng");
            Customer customer = this.context.Customers.FirstOrDefault(x => x.Email == email);

            string newPassword = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            customer.Password = DataHelper.SHA256Hash(customer.Email + "_" + newPassword);

            EmailConfiguration emailConfiguration = this.context.EmailConfigurations.FirstOrDefault();
            EmailTemplate emailTemplate = this.context.EmailTemplates.FirstOrDefault(x => x.Id == 5);

            string bodyMail = emailTemplate.Content.Replace(Constants.EmailKeyGuide.NewPassword, newPassword);
            DataHelper.SendMail(emailConfiguration, emailTemplate.Subject, bodyMail, new List<string>()
            {
                email
            }, emailTemplate.CC?.Split(';').ToList(), emailTemplate.BCC?.Split(';').ToList());
            this.context.SaveChanges();
        }

         //<summary>
         //Đăng nhập
         //</summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public object GetAccessToken(CustomerDto entity)
        {
            Customer customer = this.context.Customers
                .FirstOrDefault(x => x.Email == entity.Email);

            if (customer == null)
                throw new ArgumentException("Tài khoản hoặc mật khẩu không đúng");

            string passwordCheck = DataHelper.SHA256Hash(entity.Email + "_" + entity.Password);

            if (customer.Password != passwordCheck)
                throw new ArgumentException("Tài khoản hoặc mật khẩu không đúng");

            customer.LastLogin = DateTime.Now;
            this.context.SaveChanges();

            DateTime expirationDate = DateTime.Now.Date.AddMinutes(Constants.JwtConfig.ExpirationInMinutes);
            long expiresAt = (long)(expirationDate - new DateTime(1970, 1, 1)).TotalSeconds;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Constants.JwtConfig.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.UserData, customer.Code),
                        new Claim(ClaimTypes.Expiration, expiresAt.ToString())
                }),
                Expires = expirationDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new
            {
                customer.Email,
                customer.FullName,
                Token = tokenHandler.WriteToken(token),
                ExpiresAt = expiresAt
            };
        }

         //<summary>
         //Lấy danh sách đơn hàng của khách hàng
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        public List<OrderDto> GetOrders(string customerCode)
        {
            return this.context.Orders
                .Where(x => x.CustomerCode == customerCode)
                .OrderByDescending(x => x.Created)
                .Select(x => new OrderDto()
                {
                    Id = x.Id,
                    Address = x.Address,
                    CustomerCode = x.CustomerCode,
                    PhoneNumber = x.PhoneNumber,
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
                        Attribute = y.Attribute,
                        Reviews = y.Reviews.Select(z => new ReviewDto()
                        {
                            Content = z.Content,
                            Star = z.Star,
                            CreatedBy = z.CreatedBy,
                            Status = z.Status,
                            Created = z.Created
                        }).ToList()
                    }).ToList(),
                    Created = x.Created
                })
                .ToList();
        }

        public virtual void DeleteById(string key, string userSession = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get danh sách khách hàng theo từ khóa
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        public List<CustomerDto> Get(string keySearch)
        {
            if (!string.IsNullOrWhiteSpace(keySearch))
                keySearch = null;

            return this.context.Customers
                 .Where(x => keySearch == null || x.FullName.Contains(keySearch))
                 .Select(x => new CustomerDto()
                 {
                     Code = x.Code,
                     Address = x.Address,
                     Avatar = x.Avatar,
                     Dob = x.Dob,
                     Email = x.Email,
                     FullName = x.FullName,
                     Gender = x.Gender,
                     PhoneNumber = x.PhoneNumber
                 })
                 .ToList();
        }

        public virtual List<CustomerDto> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get thông tin khách hàng theo id
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual CustomerDto GetById(string key)
        {
            return this.context.Customers
                  .Where(x => x.Code == key)
                  .Select(x => new CustomerDto()
                  {
                      Code = x.Code,
                      Address = x.Address,
                      Avatar = x.Avatar,
                      Dob = x.Dob,
                      Email = x.Email,
                      FullName = x.FullName,
                      Gender = x.Gender,
                      PhoneNumber = x.PhoneNumber,
                      Orders = x.Orders.OrderByDescending(y => y.Created).Select(y => new OrderDto()
                      {
                          Id = y.Id,
                          Address = y.Address,
                          Created = y.Created,
                          Note = y.Note,
                          PhoneNumber = y.PhoneNumber,
                          Status = y.Status,
                          TotalAmount = y.TotalAmount,
                          OrderDetails = y.OrderDetails.Select(z => new OrderDetailDto()
                          {
                              Attribute = z.Attribute,
                              ProductDiscountPrice = z.ProductDiscountPrice,
                              ProductImage = z.ProductImage,
                              ProductName = z.ProductName,
                              ProductPrice = z.ProductPrice,
                              Qty = z.Qty
                          }).ToList()
                      }).ToList()
                  })
                  .FirstOrDefault();
        }

        /// <summary>
        /// Thêm mới tài khoản khách hàng
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CustomerDto Insert(CustomerDto entity)
        {
            if (!this.context.EmailSignUps.Any(x => x.Email == entity.Email && x.OTP == entity.OTP))
                throw new ArgumentException("Thông tin xác thực OTP không hợp lệ");

            if (this.context.Customers.Any(x => x.Email == entity.Email))
                throw new ArgumentException("Email đã được đăng ký");

            if (this.context.Customers.Any(x => x.PhoneNumber == entity.PhoneNumber))
                throw new ArgumentException("Số điện thoại đã được đăng ký");

            Customer customer = new Customer()
            {
                Code = Guid.NewGuid().ToString("N"),
                FullName = entity.FullName,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                Password = DataHelper.SHA256Hash(entity.Email + "_" + entity.Password)
            };

            this.context.Customers.Add(customer);
            this.context.SaveChanges();

            return entity;
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        public virtual void Update(string key, CustomerDto entity)
        {
            Customer customer = this.context.Customers
                .FirstOrDefault(x => x.Code == key);

            if (customer != null)
            {
                if (this.context.Customers.Any(x => x.Code != entity.Code && x.Email == entity.Email))
                    throw new ArgumentException("Email đã được đăng ký");

                if (this.context.Customers.Any(x => x.Code != entity.Code && x.PhoneNumber == entity.PhoneNumber))
                    throw new ArgumentException("Số điện thoại đã được đăng ký");

                customer.FullName = entity.FullName;
                customer.Email = entity.Email;
                customer.PhoneNumber = entity.PhoneNumber;
                customer.Address = entity.Address;
                customer.Dob = entity.Dob;
                customer.Gender = entity.Gender;

                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// <param name="key"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public void ChangePassword(string key, string oldPassword, string newPassword)
        {
            Customer customer = this.context.Customers
                 .FirstOrDefault(x => x.Code == key);

            string passwordCheck = DataHelper.SHA256Hash(customer.Email + "_" + oldPassword);

            if (customer.Password != passwordCheck)
                throw new ArgumentException("Mật khẩu cũ không đúng");
            else
            {
                string newCheck = DataHelper.SHA256Hash(customer.Email + "_" + newPassword);
                customer.Password = newCheck;

                this.context.SaveChanges();
            }
        }
    }
}