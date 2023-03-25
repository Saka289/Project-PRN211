using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PRN211_Project.Models
{
    public partial class Rate
    {
        public int MovieId { get; set; }
        public int PersonId { get; set; }
        public string? Comment { get; set; }
        [Required]
        [Range(0, 10, ErrorMessage = "Rating must be in range")]
        public double NumericRating { get; set; }
        public DateTime? Time { get; set; }

        public virtual Movie? Movie { get; set; }
        public virtual Person? Person { get; set; }
    }
}
