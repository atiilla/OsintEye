using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitRecon.Models
{
    public class EmailAuthor
    {
        public int Id { get; set; }  // Primary key (should auto-increment)
        public string Email { get; set; }
        public string Author { get; set; }
    }


}
