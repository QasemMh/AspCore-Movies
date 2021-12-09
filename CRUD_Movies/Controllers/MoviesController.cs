using CRUD_Movies.Models;
using CRUD_Movies.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD_Movies.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _env;
        private List<string> _allowExe = new List<string> { ".png", ".jpeg", ".jpg" };
        private const int MOVIE_SIZE = 2000000;

        public MoviesController(ApplicationDbContext context, IToastNotification toastNotification, IWebHostEnvironment env)
        {
            _toastNotification = toastNotification;
            _env = env;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.OrderByDescending(m => m.Rate).ToListAsync();
            return View(movies);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var genres = new MovieFormViewModel
            {
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };
            return View(genres);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View(model);
            }

            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("ImagePath", "Please Select Image");
                return View(model);
            }

            var poster = files.FirstOrDefault();


            if (!_allowExe.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("ImagePath", "Image extension not allowed");
                return View(model);
            }
            if (poster.Length > MOVIE_SIZE)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("ImagePath", "Image Length Must be less than 2MB");
                return View(model);
            }


            string wwwRootPath = _env.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + "_" + poster.FileName;
            model.ImagePath = fileName;
            var path = Path.Combine(wwwRootPath + "/image/" + fileName);
            using (var sf = new FileStream(path, FileMode.Create))
            {
                await poster.CopyToAsync(sf);
            }

            var movie = new Movie
            {
                Name = model.Name,
                Rate = model.Rate,
                StoryLine = model.StoryLine,
                Year = model.Year,
                GenreId = model.GenreId,
                ImagePath = model.ImagePath
            };
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("Movie Added Successfully");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            var viewModel = new MovieFormViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                Rate = movie.Rate,
                StoryLine = movie.StoryLine,
                Year = movie.Year,
                GenreId = movie.GenreId,
                ImagePath = movie.ImagePath,
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            //Edit
            return View("Create", viewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, MovieFormViewModel model,
            string imagePath2)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                //Edit
                return View("Create", model);
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            var files = Request.Form.Files;
            if (!files.Any())
            {
                movie.ImagePath = imagePath2;
            }
            else
            {
                var poster = files.FirstOrDefault();

                if (!_allowExe.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("ImagePath", "Image extension not allowed");
                    model.ImagePath = imagePath2;
                    //Edit
                    return View("Create", model);
                }
                if (poster.Length > MOVIE_SIZE)
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("ImagePath", "Image Length Must be less than 2MB");
                    model.ImagePath = imagePath2;

                    //Edit
                    return View("Create", model);
                }
                string wwwRootPath = _env.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" + poster.FileName;
                movie.ImagePath = fileName;
                var path = Path.Combine(wwwRootPath + "/image/" + fileName);
                using (var sf = new FileStream(path, FileMode.Create))
                {
                    await poster.CopyToAsync(sf);
                }

                //todo remove prev image
            }

            movie.Name = model.Name;
            movie.Rate = model.Rate;
            movie.StoryLine = model.StoryLine;
            movie.GenreId = model.GenreId;
            movie.Year = model.Year;

            await _context.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("Movie Updated Successfully");


            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> View(int? id)
        {
            if (id == null) return NotFound();
            var movie = await _context.Movies
                .Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            return View(movie);

            //todo add similar movie to view
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
