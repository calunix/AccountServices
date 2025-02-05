using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServices.Models
{
    public class PasswordReset
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Domain Username")]
        public string? Username { get; set; }

        [Required]
        [NotMapped]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string? UserEmailAddress { get; set; }

        public string? VerificationCode { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Verification Code is required.")]
        [Display(Name = "Verification Code")]
        public string VerificationCode2 { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CodeExpiry { get; set; }

        [NotMapped]
        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [Compare("ConfirmedPassword", ErrorMessage = "Passwords do not match")]
        public string Password { get; set; }

        [Display(Name = "Confirm New Password")]
        [NotMapped]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmedPassword { get; set; }
    }
}
