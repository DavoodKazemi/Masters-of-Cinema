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

        public IList<Director> DirectorList { get; set; }
        private readonly ILogger<MovieController> logger;
        private readonly ICinemaRepository _repository;
        private readonly UserManager<User> _userManager;

        public MovieController(Context context, 
            IWebHostEnvironment webHostEnvironment, 
            ILogger<MovieController> logger,
            ICinemaRepository repository,
            UserManager<User> userManager)
        {
            _context = context;
            _hostEnvironment = webHostEnvironment;
            this.logger = logger;
            _repository = repository;
            _userManager = userManager;
        }

        private IEnumerable<Movie> GetMovies()
        {
            try
            {
                logger.LogInformation("GetMovies was called!");
                return _context.Movies.OrderBy(p => p.Id).ToList();

            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all movies: {ex}");
                return null;
            }

        }

        public IEnumerable<Director> GetDirectors()
        {
            try
            {
                logger.LogInformation("GetDirectors was called!");
                return _context.Directors.OrderBy(p => p.Id).ToList();

            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all directors: {ex}");
                return null;
            }

        }

        // GET: Movies
        public ActionResult Index()
        {
            var tupleModel = new Tuple<IEnumerable<Movie>, IEnumerable<Director>>(GetMovies(), GetDirectors());
            return View(tupleModel);

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

            var UserName = HttpContext.User.Identity.Name;

            //Add data to view model
            MovieRateDirector movieRateDirector = new MovieRateDirector()
            {
                Movie = GetMovieById(id),
                MovieRating = GetRatingByMovieId(id),
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
            var UserName = HttpContext.User.Identity.Name;

            movieRateDirector.Movie = GetMovieById(movieRateDirector.MovieRating.MovieId);
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
                    /*var local = _context.Set<MovieRating>()
                    .Local.Where(u => u.User.UserName == UserName)
                    .FirstOrDefault(entry => entry.MovieId.Equals(movieRateDirector.MovieRating.MovieId));
                    */
                    
                    var local = _context.Set<MovieRating>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(movieRateDirector.MovieRating.Id));
                    /*                    .Where(entry => entry.Id.Equals(movieRateDirector.MovieRating.Id));
                    */
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

        public Movie GetMovieById(int id)
        {
            try
            {
                logger.LogInformation("GetMovieById was called!");
                return _context.Movies.Include(x => x.MovieRatings).FirstOrDefault(p => p.Id == id);
                
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all movies: {ex}");
                return null;
            }
        }

        //Movie Id - **Later should return the user rating of a movie**
        private MovieRating GetRatingByMovieId(int id)
        {
            try
            {
                var UserName = HttpContext.User.Identity.Name;

                logger.LogInformation("GetRatingById was called!");
                return _context.MovieRatings.Include(x => x.User).Where(u => u.User.UserName == UserName)
                    .FirstOrDefault(m => m.MovieId == id);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all movie ratings: {ex}");
                return null;
            }
        }
    }
}
