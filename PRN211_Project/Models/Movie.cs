using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
      [Required(ErrorMessage="Title not null")]
        public int Year { get; set; }
      [Required(ErrorMessage = "year not null")]
        public string Image { get; set; }
      //[Required(ErrorMessage = "Image not null")]
      public string Description { get; set; }
      [Required(ErrorMessage = "Decription nost null")]
        public int GenreId { get; set; }
      [NotMapped]
      [DisplayName("Upload File")]
      [Required(ErrorMessage = "image file not null")]
      public IFormFile imageFile { get; set; }
        public virtual Genre? Genre { get; set; }
        public virtual ICollection<Rate>? Rates { get; set; }
    }
}
