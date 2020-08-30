using System.ComponentModel.DataAnnotations;

namespace BookStore_UI.Models
{
    public class LoginModel
    {
        [EmailAddress]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
