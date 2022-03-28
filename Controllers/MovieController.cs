using MastersOfCinema.Data;
using MastersOfCinema.Data.Entities;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Movies with inifinite scroll 
        public ActionResult Index(int? pageNum)
        {

            MovieListViewModel films = new MovieListViewModel()
            {
                Movies = _repository.GetAllMovies(),
                IsFirstPage = false,
                //Directors = _repository.GetAllDirectors(),
                //CurrentUser = user
            };
            films.listCount = films.Movies.Count();

            int itemsPerPage = 15;
            //page number - starts from 0
            pageNum = pageNum ?? 0;

            //first time it's not ajax, next times it is
            bool isAjaxCall = HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxCall)
            {
                var customers = _repository.GetMoviesForAjax(pageNum.Value, itemsPerPage);
                films.Movies = customers;
                return PartialView("Lists/_AjaxMovieListPartial", films);
            }
            else
            {
                films.IsFirstPage = true;
                int pageCount = (_context.Movies.ToList().Count() - 1) / 15 + 1;
                ViewBag.listCount = _context.Movies.ToList().Count();
                ViewBag.pageCount = pageCount;
                films.Movies = _repository.GetMoviesForAjax(pageNum.Value, itemsPerPage);
                return View("Index", films);
            }
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
                MovieLog = _repository.IsLoggedMovieId(id),
                Watchlist = _repository.IsInWatchlistById(id),
                Review = _repository.GetMovieReviews(id),
                
                //UserReview ASSIGNED BELOW
            };
            //If movie was not logged, make a new log obj to prevent error
            if (movieRateDirector.MovieLog == null)
            {
                movieRateDirector.MovieLog = new MovieLog
                {
                    Id = 0,
                    MovieId = id,
                    User = _context.Users.FirstOrDefault(u => u.UserName == UserName)
                };
            }
            //If movie was not in Watchlist, make a new Watchlist obj to prevent error
            if (movieRateDirector.Watchlist == null)
            {
                movieRateDirector.Watchlist = new Watchlist
                {
                    Id = 0,
                    MovieId = id,
                    User = _context.Users.FirstOrDefault(u => u.UserName == UserName)
                };
            }


            //If movie is reviewed by this user or not
            var UserReviewRaw = _repository.IsReviewed(id);
            //If so, add the review + like stats to UserReview!
            if(UserReviewRaw != null)
            {
                movieRateDirector.UserReview = _repository.GetReviewsLikeStats(UserReviewRaw);
            }
            

            //List count
            ViewBag.watchlistCount = _context.Watchlists.Where(m => m.MovieId == id).Count();

            //Log count
            ViewBag.logCount = _context.MovieLogs.Where(m => m.MovieId == id).Count();

            //If movie was not rated, make a new Rating obj to prevent error
            if (movieRateDirector.MovieRating == null)
            { movieRateDirector.MovieRating = new MovieRating
                {Id = 0, MovieId = id, User = _context.Users.FirstOrDefault(u => u.UserName == UserName),
                Rating = 0};
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

        //Post review - save it to database - display it
        [HttpPost]
        public async Task<IActionResult> Review(MovieRateDirector movieRateDirector)
        {
            //Only need to get movieId and the review text from view
            //Need to set a movieId and User to the new record

            //Start saving the review
            //create a new review object for saving into database
            Review newReview = new Review() {
                //add review text
                ReviewText = movieRateDirector.UserReview.ReviewText
                
            };
            //add movie Id
            var movieId = movieRateDirector.UserReview.MovieId;
            newReview.MovieId = movieId;

            //add user (reviewer)
            var UserName = HttpContext.User.Identity.Name;
            newReview.User = _context.Users.FirstOrDefault(u => u.UserName == UserName);

            newReview.Id = 0;

            if (ModelState.IsValid)
            {
                //Save (Create or update) rating in DB
                _context.Update(newReview);
                await _context.SaveChangesAsync();
            }
            //End saving the review

            //create a new model for sending the new review to the view
            MovieRateDirector movieRateDirector2 = new MovieRateDirector();

            //add like count and isLiked to the new review
            //(probably would be better to add them manually: likeCount = 0, isLiked = false)
            movieRateDirector2.UserReview = _repository.GetReviewsLikeStats(newReview);
            
            //Display the new review
            return PartialView("Review/_AjaxReview", movieRateDirector2);
            //End create and send model
        }

        //Like / Unlike a review
        [HttpPost]
        public async Task<IActionResult> LikeReview(int ReviewId)
        {
            //If user not logged in, they can't like review
            if (User.Identity.IsAuthenticated == false)
            {
                return BadRequest("You should login first");
            }

            //Only need to get Review Id from view
            //Need to set a Review Id and User to the new record
            LikeReview like = new LikeReview();
            var reviewId = ReviewId;
            //Check to see if this review had been liked by this user before
            var UserName = HttpContext.User.Identity.Name;
            
            like.User = _context.Users.FirstOrDefault(u => u.UserName == UserName);

            //If true it had been liked by this user before (meaning user wants to unlike it now)
            bool wasLiked = _context.LikeReview.Where(u => u.User.UserName == UserName).Any(m => m.ReviewId == ReviewId);

            //If wasLiked != true, it's a like request, else it's an unlike request
            if (!wasLiked)
            {
                //Create record
                like.Id = 0;
                like.ReviewId = ReviewId;
                if (ModelState.IsValid)
                {
                    //Save (Create) like in DB
                    _context.Update(like);
                    await _context.SaveChangesAsync();
                }
            }
            else //it's a delete (unlike) request
            {
                //Delete like
                var logItem = _context.LikeReview.Where(u => u.User.UserName == UserName)
                .FirstOrDefault(m => m.ReviewId == ReviewId);
                _context.LikeReview.Remove(logItem);
                await _context.SaveChangesAsync();

            }
            return Ok("Form Data received!");
        }

        //Edit a review
        [HttpPost]
        public async Task<IActionResult> Ferrari3(string reviewId)
        {
            //create a new model for sending the new review to the view
            //MovieRateDirector movieRateDirector = new MovieRateDirector();
            int id = Int32.Parse(reviewId);
            var existingUserReview = _context.Review.Include(u => u.User).Where(x => x.Id == id).FirstOrDefault();

            //create a function to get it by id directly
            var existingUserReviewStats = _repository.GetReviewsLikeStats(existingUserReview);
            //add like count and isLiked to the new review
            //(probably would be better to add them manually: likeCount = 0, isLiked = false)
            /*movieRateDirector.UserReview = _repository.GetMovieReviews(1).FirstOrDefault();

            return PartialView("Review/_EditReviewPartial", movieRateDirector);*/

            MovieRateDirector movieRateDirector2 = new MovieRateDirector();

            Review review = _context.Review.FirstOrDefault();
            //add like count and isLiked to the new review
            //(probably would be better to add them manually: likeCount = 0, isLiked = false)
            movieRateDirector2.UserReview = existingUserReviewStats;

            //Display the new review
            return PartialView("Review/_EditReview", movieRateDirector2);
        }

        //Update a review
        [HttpPost]
        public async Task<IActionResult> UpdateReview(MovieRateDirector newTextId)
        {
            var review = await _context.Review.Include(x => x.User).FirstOrDefaultAsync(m => m.Id == newTextId.UserReview.Id);
            var newText = newTextId.UserReview.ReviewText;

            //if (ModelState.IsValid)
            //{
                //the new text has to be different and also has to not be empty
                if (review.ReviewText != newText && newTextId.UserReview.ReviewText != "")
                {
                    //If image not uploaded, assign the default photo
                    review.ReviewText = newText;
                }

                var local = _context.Set<Review>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(review.Id));
                // check if local is not null 
                if (local != null)
                {
                    // detach
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Update(review);
                await _context.SaveChangesAsync();

                //create a new model for sending the new review to the view
                MovieRateDirector movieRateDirector2 = new MovieRateDirector();

                //add like count and isLiked to the new review
                //(probably would be better to add them manually: likeCount = 0, isLiked = false)
                movieRateDirector2.UserReview = _repository.GetReviewsLikeStats(review);

            //}
            return PartialView("Review/_AjaxReview", movieRateDirector2);
        }


    }
}
