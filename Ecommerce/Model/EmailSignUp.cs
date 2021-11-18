using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    [Table("EmailSignUp")]
    public class EmailSignUp
    {
        [Key]
        public string Email { get; set; }
        public string OTP { get; set; }
    }
}
