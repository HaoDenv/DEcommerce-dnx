using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    [Table("Website")]
    public class Website
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string Facebook { get; set; }
        public string Youtube { get; set; }
        public string Copyright { get; set; }

    }
}
