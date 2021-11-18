using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    [Table("ProductImage")]
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Image { get; set; }

        public virtual Product Product { get; set; }
    }
}
