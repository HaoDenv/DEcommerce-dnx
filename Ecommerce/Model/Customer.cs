using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public string Code { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? Dob { get; set; }
        public string Gender { get; set; }
        public DateTime? LastLogin { get; set; }
        public string OTP { get; set; }


        public virtual ICollection<Order> Orders { get; set; }
    }
}
