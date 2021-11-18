using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    [Table("EmailConfiguration")]
    public class EmailConfiguration
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
