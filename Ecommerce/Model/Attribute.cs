using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    [Table("Attribute")]
    public class Attribute
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }
    }
}
