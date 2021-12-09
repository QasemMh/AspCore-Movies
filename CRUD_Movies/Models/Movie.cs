using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD_Movies.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required, MaxLength(250)]
        public string Name { get; set; }
        public int Year { get; set; }
        public double Rate { get; set; }
        [Required, MaxLength(2500)]
        public string StoryLine { get; set; }
        public string ImagePath { get; set; }


        [NotMapped]
        public IFormFile Image { get; set; }

        public byte GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
