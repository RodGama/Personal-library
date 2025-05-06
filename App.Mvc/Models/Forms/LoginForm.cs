using System.ComponentModel.DataAnnotations;

namespace App.MVC.Models.Forms
{
    public class LoginForm
    {

        [Required(ErrorMessage = "Email cannot by empty")]
        [EmailAddress(ErrorMessage = "Email address not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password cannot be empty.")]
        [StringLength(100, ErrorMessage = "Password is too weak. Minimum length is 6 characters.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
