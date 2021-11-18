using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    [Table("Review")]
    public class Review
    {
        public int Id { get; set; }
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int? Star { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }

        public virtual Product Product { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
    }
}
