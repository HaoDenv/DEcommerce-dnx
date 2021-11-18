using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    [Table("EmailRegistration")]
    public class EmailRegistration
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }

    }
}
