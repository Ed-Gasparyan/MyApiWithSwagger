using Domain.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class BookDTO
    {
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
        [Range(1, 100)]
        public int TotalCopies { get; set; }
    }
}
