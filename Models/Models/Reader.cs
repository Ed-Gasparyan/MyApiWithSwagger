using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Reader
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
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

        public List<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    }
}
