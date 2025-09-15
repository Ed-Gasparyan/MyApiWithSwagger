using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class ReaderDTO
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^\+374\d{8}$", ErrorMessage = "Enter a valid Armenian phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
