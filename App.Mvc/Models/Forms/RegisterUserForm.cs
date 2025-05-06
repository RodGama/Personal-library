using System.ComponentModel.DataAnnotations;

namespace App.MVC.Models.Forms
{
    public class RegisterUserForm
    {
        [Required(ErrorMessage = "Name cannot be empty.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email cannot by empty")]
        [EmailAddress(ErrorMessage = "Email address not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password cannot be empty.")]
        [StringLength(100, ErrorMessage = "Password is too weak. Minimum length is 6 characters.", MinimumLength = 6)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Birth date cannot be empty.")]
        [DataType(DataType.Date, ErrorMessage = "Birth date cannot be in the future.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CustomValidation(typeof(RegisterUserForm), "ValidateAge")]
        public string BirthDate { get; set; }

        public static ValidationResult ValidateAge(string birthDate, ValidationContext context)
        {
            if (DateTime.TryParse(birthDate, out DateTime birthDateParsed))
            {
                var today = DateTime.Now;
                if (birthDateParsed > today)
                    return new ValidationResult("Birth date cannot be in the future.", new[] { context.MemberName });
            }
            return ValidationResult.Success;
        }
    }
}
