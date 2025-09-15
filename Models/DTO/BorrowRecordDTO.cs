using Domain.Attributes;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class BorrowRecordDTO
    {

        [Required]
        public int ReaderId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string BookName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string ReaderName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

    }
}
