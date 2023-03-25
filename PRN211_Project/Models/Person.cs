using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PRN211_Project.Models
{
    public partial class Person
    {
        public Person()
        {
            Rates = new HashSet<Rate>();
        }

        public int? PersonId { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 3)]
		public string? Fullname { get; set; }

        [Required]
        public string? Gender { get; set; }

		[Required]
		[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
		public string Email { get; set; }

		[Required]
		public string? Password { get; set; }

		[Required]
		public int? Type { get; set; }

		[Required]
		public bool? IsActive { get; set; }

        public virtual ICollection<Rate>? Rates { get; set; }
    }
}
