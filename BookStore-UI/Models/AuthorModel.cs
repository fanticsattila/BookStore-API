using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Models
{
    public class AuthorModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [DisplayName("Biography")]
        [StringLength(250)]
        public string Bio { get; set; }

        public virtual IList<BookModel> Books { get; set; }
    }
}
