using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServices.Models
{
    public class AccountStatus
    {
        [NotMapped]
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string? EmailAddress { get; set; }

        [NotMapped]
        [Required]
        public string AccountStatusResults { get; set; }
    }
}
