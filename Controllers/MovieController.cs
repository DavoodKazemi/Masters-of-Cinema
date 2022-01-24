using MastersOfCinema.Data;
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
    public class MovieController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ICinemaRepository _repository;
        private readonly UserManager<User> _userManager;

        public MovieController(Context context, 
            IWebHostEnvironment webHostEnvironment, 
            ICinemaRepository repository,
            UserManager<User> userManager)
        {
            _context = context;
            _hostEnvironment = webHostEnvironment;
            _repository = repository;
            _userManager = userManager;
        }

        // GET: Movies
        public ActionResult Index()
        {
            MovieRateDirector movieRateDirector = new MovieRateDirector()
            {
                Movies = _repository.GetAllMovies(),
                Directors = _repository.GetAllDirectors(),
            };
            return View(movieRateDirector);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Bind director thing in order to display the IDs and Nemes of directors in the view
            List<Director> cl = new List<Director>();
            cl = (from c in _context.Directors select c).ToList();
            ViewBag.message = cl;
            //END Bind director thing

            var movieViewModel = await _context.Movies.FindAsync(id);
            if (movieViewModel == null)
            {
                return NotFound();
            }
            return View(movieViewModel);
        }

        // POST: Directors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,Description,ImageFile,ImageName,DirectorId")] Movie movieViewModel)
        {
            /*var OriginalImage = Movie.ImageName;*/

            var Movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            var OriginalImage = Movie.ImageName;
            //movieViewModel.DirectorId = 1;


            if (id != movieViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (movieViewModel.ImageFile == null)
                    {
                        //If image not uploaded, assign the default photo
                        movieViewModel.ImageName = Movie.ImageName;
                    }
                    else
                    {
                        //If image uploaded, Save image to wwwroot/image
                        string fileName;
                        //If image uploaded, Save image to wwwroot/image
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        fileName = Path.GetFileNameWithoutExtension(movieViewModel.ImageFile.FileName);
                        string extension = Path.GetExtension(movieViewModel.ImageFile.FileName);
                        movieViewModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        var path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await movieViewModel.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    var local = _context.Set<Movie>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(id));
                    // check if local is not null 
                    if (local != null)
                    {
                        // detach
                        _context.Entry(local).State = EntityState.Detached;
                    }

                    _context.Update(movieViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieViewModelExists(movieViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movieViewModel);
        }
        private bool MovieViewModelExists(int id)
        {
            return _context.Directors.Any(e => e.Id == id);
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
/*            var tupleModel = new Tuple<IEnumerable<MovieViewModel>, IEnumerable<DirectorViewModel
 *            
 *            >>(GetMovies(), GetDirectors());
*/

            //Bind director thing in order to display the IDs and Nemes of directors in the view
            List<Director> cl = new List<Director>();
            cl = (from c in _context.Directors select c).ToList();
            ViewBag.message = cl;
            //END Bind director thing
            return View();
        }

        // POST: Image/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Year,Description,ImageFile,ImageName,DirectorId")] Movie movieViewModel)
        {
            if (ModelState.IsValid)
            {
                string fileName;

                if (movieViewModel.ImageFile == null)
                {
                    //If image not uploaded, assign the default photo
                    fileName = "DirectorsDefaultImage.jpg";
                    movieViewModel.ImageName = fileName;
                }
                else
                {
                    //If image uploaded, Save image to wwwroot/image
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    fileName = Path.GetFileNameWithoutExtension(movieViewModel.ImageFile.FileName);
                    string extension = Path.GetExtension(movieViewModel.ImageFile.FileName);
                    movieViewModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    var path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await movieViewModel.ImageFile.CopyToAsync(fileStream);
                    }
                }

                //Insert record
                _context.Add(movieViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movieViewModel);
        }


        // GET: Movie/Details/5
        [Authorize]
        public IActionResult Details(int id)
        {
            //Current User
            var UserName = HttpContext.User.Identity.Name;

            //Add data to view model
            MovieRateDirector movieRateDirector = new MovieRateDirector()
            {
                Movie = _repository.GetMovieById(id),
                MovieRating = _repository.GetRatingByMovieId(id),
                AverageRate = _repository.GetAverageRating(id),
                RatePercents = _repository.MovieRatingChartStats(id),
                RateCounts = _repository.MovieRatingCount(id),
            };
            //If movie was not rated, make a new Rating obj to prevent error
            if (movieRateDirector.MovieRating == null)
            { movieRateDirector.MovieRating = new MovieRating
                {Id = 0, MovieId = id, User = _context.Users.FirstOrDefault(u => u.UserName == UserName)};
            }

            //Start Rate stats
            //Rate all count + You need to fix this later when add users, you need to define something like isRated (userId)
            movieRateDirector.RateCountAll = movieRateDirector.Movie.MovieRatings.Count();

            //Saving Old rate
            ViewBag.oldRate = movieRateDirector.MovieRating.Rating ?? 0;
            
            //END Rate stats


            movieRateDirector.Director = _context.Directors
                .FirstOrDefault(m => m.Id == movieRateDirector.Movie.DirectorId);
            //END Add data to view model

            if (movieRateDirector.Movie == null)
            {
                //return view(404);?
                return NotFound();
            }

            return View(movieRateDirector);
        }

        [HttpPost]
        public async Task<IActionResult> Details(MovieRateDirector movieRateDirector)
        {
            //Current user
            var UserName = HttpContext.User.Identity.Name;

            movieRateDirector.Movie = _repository.GetMovieById(movieRateDirector.MovieRating.MovieId);
            movieRateDirector.Director = _context.Directors
                .FirstOrDefault(m => m.Id == movieRateDirector.Movie.DirectorId);

            movieRateDirector.MovieRating.User = _context.Users.FirstOrDefault(u => u.UserName == UserName);

            //If rating != 0, it's either create or update, else it's delete rate
            if (movieRateDirector.MovieRating.Rating != 0) {
                //Update or Create?
                //Check - Movie has any rating
                int movieId = movieRateDirector.MovieRating.MovieId;
                //Check if this user had rated this movie before!
                bool hadRated = _context.MovieRatings.Where(u => u.User.UserName == UserName).Any(m => m.MovieId == movieId);
                if (hadRated)
                {
                    //Update - Make the Id of view = Id of the existing rate in db
                    movieRateDirector.MovieRating.Id = _context.MovieRatings.Where(u => u.User.UserName == UserName)
                                    .FirstOrDefault(m => m.MovieId == movieRateDirector.MovieRating.MovieId).Id;

                    //To avoid error in update
                    var local = _context.Set<MovieRating>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(movieRateDirector.MovieRating.Id));

                    // check if local is not null 
                    if (local != null)
                    {
                        // detach
                        _context.Entry(local).State = EntityState.Detached;
                    }
                }
                else
                {
                    //Create
                    movieRateDirector.MovieRating.Id = 0;
                }
                //END - Update or Create?

                if (ModelState.IsValid)
                {
                    //Save (Create or update) rating in DB
                    _context.Update(movieRateDirector.MovieRating);
                    await _context.SaveChangesAsync();
                }
            }
            else //it's a delete request
            {
                //Delete rating - If rating = 0, it means they clicked on remove rate button
                var directorViewModel = _context.MovieRatings.Where(u => u.User.UserName == UserName)
                .FirstOrDefault(m => m.MovieId == movieRateDirector.MovieRating.MovieId);
                _context.MovieRatings.Remove(directorViewModel);
                await _context.SaveChangesAsync();
            }

            return Ok("Form Data received!");
        }
    }
}
