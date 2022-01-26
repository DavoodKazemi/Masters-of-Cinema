using MastersOfCinema.Data;
using MastersOfCinema.Data.Entities;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MastersOfCinema.Controllers
{
    public class MovieRateController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ICinemaRepository _repository;
        private readonly UserManager<User> _userManager;

        public MovieRateController(Context context,
            IWebHostEnvironment webHostEnvironment,
            ICinemaRepository repository,
            UserManager<User> userManager)
        {
            _context = context;
            _hostEnvironment = webHostEnvironment;
            _repository = repository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Log(MovieRateDirector movieRateDirector)
        {

            //Only need to get movieId from view
            //Need to set a movieId and User to the new record
            MovieLog log = new MovieLog();
            var movieId = movieRateDirector.MovieRating.MovieId;
            //Check to see if this movie had been added to the user's watchlist before
            var UserName = HttpContext.User.Identity.Name;
            //watchlist.MovieId = id;
            log.User = _context.Users.FirstOrDefault(u => u.UserName == UserName);
            //If true it had been in user's watchlist (meaning user wants to remove it from watchlist)
            bool wasLogged = _context.MovieLogs.Where(u => u.User.UserName == UserName).Any(m => m.MovieId == movieId);

            //If rating != 0, it's either create or update, else it's delete rate
            if (!wasLogged)
            {
                //Create
                log.Id = 0;
                log.MovieId = movieId;
                if (ModelState.IsValid)
                {
                    //Save (Create or update) rating in DB
                    _context.Update(log);
                    await _context.SaveChangesAsync();
                }
            }
            else //it's a delete request
            {
                //Delete rating - If rating = 0, it means they clicked on remove rate button
                var logItem = _context.MovieLogs.Where(u => u.User.UserName == UserName)
                .FirstOrDefault(m => m.MovieId == movieId);
                _context.MovieLogs.Remove(logItem);
                await _context.SaveChangesAsync();

            }

            return Ok("Form Data received!");
        }
    }
}
