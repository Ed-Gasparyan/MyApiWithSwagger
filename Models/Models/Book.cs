using Domain.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Book
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [MaxLength(17)]
        [Isbn13]
        public string ISBN { get; set; } = string.Empty;

        [Required]
        [YearRange(1800)]
        public int PublishedYear { get; set; }

        [Required]
        [Range(1,100)]
        public int TotalCopies { get; set; }

        [Required]
        [Range(1, 100)]
        public int AvailableCopies { get; set; }
        public List<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();

    }
}
