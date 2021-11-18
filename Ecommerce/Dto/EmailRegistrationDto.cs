using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Dto
{
    public class EmailRegistrationDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
    }
}
