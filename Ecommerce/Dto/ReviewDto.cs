using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dto
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int? Star { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }

        public ProductDto Product { get; set; }
    }
}
