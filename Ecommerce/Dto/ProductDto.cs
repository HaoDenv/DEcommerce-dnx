using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dto
{
    public class ProductDto
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

        public MenuDto Menu { get; set; }
        public List<ProductAttributeDto> ProductAttributes { get; set; }
        public List<ProductImageDto> ProductImages { get; set; }
        public List<ProductRelatedDto> ProductRelateds { get; set; }
        public List<ReviewDto> Reviews { get; set; }

        public List<AttributeDto> Attributes { get; set; }

        public double? TotalQty { get; set; }
        public double? TotalAmount { get; set; }
        public double? RateAvg { get; set; }
    }
}
