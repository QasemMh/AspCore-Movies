﻿using CRUD_Movies.Models;
using CRUD_Movies.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD_Movies.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
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
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            return View();
        }
    }
}
