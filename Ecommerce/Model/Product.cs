using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    [Table("Product")]
    public class Product
    {
        public int Id { get; set; }
        public int? MenuId { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Image { get; set; }
        public int? Index { get; set; }
        public int? Status { get; set; }
        public double? Price { get; set; }
        public double? DiscountPrice { get; set; }
        public bool? Selling { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }

        public virtual Menu Menu { get; set; }
        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductRelated> ProductRelateds { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
