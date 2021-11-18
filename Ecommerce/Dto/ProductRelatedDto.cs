using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dto
{
    public class ProductRelatedDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductRelatedId { get; set; }

        public ProductDto Product { get; set; }
        public ProductDto ProductRelated { get; set; }
    }
}
