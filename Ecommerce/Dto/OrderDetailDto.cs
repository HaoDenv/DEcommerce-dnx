using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dto
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public double? ProductPrice { get; set; }
        public double? ProductDiscountPrice { get; set; }
        public int? Qty { get; set; }
        public string Attribute { get; set; }

        public List<ReviewDto> Reviews { get; set; }
    }
}
