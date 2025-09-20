using Domain.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class BorrowRecord
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ReaderId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [ForeignKey("ReaderId")]
        public Reader Reader { get; set; } = null!;

        [Required]
        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;

        public DateTime BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; } = null;

        public DateTime ReturnDue { get; set; }

    }
}
