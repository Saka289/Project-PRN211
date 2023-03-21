using System;
using System.Collections.Generic;

namespace PRN211_Project.Models
{
    public partial class Movie
    {
        public Movie()
        {
            Rates = new HashSet<Rate>();
        }

        public int MovieId { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int? GenreId { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }
    }
}
