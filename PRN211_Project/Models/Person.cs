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

        public string? Fullname { get; set; }
        [Required]

        public string? Gender { get; set; }
        [Required]

        public string? Email { get; set; }
        [Required]

        public string? Password { get; set; }
        public int? Type { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Rate>? Rates { get; set; }
    }
}
