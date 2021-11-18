using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    [Table("Menu")]
    public class Menu
    {
        public int Id { get; set; }
        public int? ParentMenu { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public int? Index { get; set; }
        public bool? ShowHomePage { get; set; }
        public string Type { get; set; }
        public bool Active { get; set; }

        [ForeignKey("ParentMenu")]
        public virtual Menu PMenu { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
