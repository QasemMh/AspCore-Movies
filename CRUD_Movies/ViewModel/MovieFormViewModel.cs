using CRUD_Movies.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD_Movies.ViewModel
{
    public class MovieFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }

        public int Year { get; set; }
        [Range(1, 10)]
        public double Rate { get; set; }
        [Required, StringLength(2500)]
        public string StoryLine { get; set; }
        public string ImagePath { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage ="Category is required")]
        public byte GenreId { get; set; }

        public IEnumerable<Genre> Genres { get; set; }

        [NotMapped]
        [Display(Name = "Movie Poster")]
        public IFormFile Image { get; set; }


    }
}
