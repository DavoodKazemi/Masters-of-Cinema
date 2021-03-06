using MastersOfCinema.Data;
using MastersOfCinema.Data.Entities;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.Controllers
{
    //[Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ICinemaRepository _repository;
        private readonly Context _context;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _userId;

        public AccountController(IHttpContextAccessor httpContextAccessor,
            ICinemaRepository repository, Context context, ILogger<AccountController> logger, SignInManager<User> signInManager)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
            _signInManager = signInManager;
            _userId = httpContextAccessor;
        }
        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Director");
            }
            LoginViewModel model2 = new LoginViewModel()
            {
                Username = "LaraCroft@gmail.com",
                Password = "aVjw3ST!3cS@M26@qK$Y",
                RememberMe = true
            };
            return View(model2);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var result = await _signInManager.PasswordSignInAsync(model.Username,
                  model.Password,
                  model.RememberMe,
                  false);

                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        return RedirectToAction("Index", "App");
                    }
                }
            }
                ModelState.AddModelError("", "Failed to login");

            LoginViewModel model2 = new LoginViewModel()
            {
                Username = "LaraCroft@gmail.com",
                Password = "aVjw3ST!3cS@M26@qK$Y",
                RememberMe = true
            };

            return View(model2);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "App");
        }

        //to be fixed
        public IActionResult Profile()
        {
            var id = _userId.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _context.Users.Where(i => i.Id == id).FirstOrDefault();
            var watchList = new ProfileViewModel()
            {
                Movies = _repository.GetWatchlist(),
                Directors = _repository.GetAllDirectors(),
                CurrentUser = user
            };

            return View(watchList);
        }
        public ActionResult Watchlist(int? pageNum)
        {
            var id = _userId.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _context.Users.Where(i => i.Id == id).FirstOrDefault();
            MovieListViewModel watchList = new MovieListViewModel()
            {
                Movies = _repository.GetWatchlist(),
                //Directors = _repository.GetAllDirectors(),
                CurrentUser = user,
                IsFirstPage = false
            };

            watchList.listCount = watchList.Movies.Count();

            int itemsPerPage = 15;
            //page number (starts from 0)
            pageNum = pageNum ?? 0;

            //first time it's not ajax, next times it is
            bool isAjaxCall = HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxCall)
            {
                var newItems = _repository.GetMovieListForAjax(pageNum.Value, itemsPerPage, watchList.Movies);
                watchList.Movies = newItems;

                return PartialView("Lists/_AjaxMovieListPartial", watchList);
            }
            else
            {
                watchList.IsFirstPage = true;
                int pageCount = (watchList.Movies.ToList().Count() - 1) / itemsPerPage + 1;
                ViewBag.pageCount = pageCount;
                var movies = _repository.GetMovieListForAjax(pageNum.Value, itemsPerPage, watchList.Movies);
                watchList.Movies = movies;
                return View("Watchlist", watchList);
            }

        }

        public ActionResult Films(int? pageNum)
        {
            int itemsPerPage = 15;
            //page number (starts from 0)
            pageNum = pageNum ?? 0;

            var id = _userId.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _context.Users.Where(i => i.Id == id).FirstOrDefault();
            MovieListViewModel films = new MovieListViewModel()
            {
                Movies = _repository.GetFilms(),
                //Directors = _repository.GetAllDirectors(),
                CurrentUser = user,
                IsFirstPage = false
            };
            films.listCount = films.Movies.Count();

            

            //first time it's not ajax, next times it is
            bool isAjaxCall = HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxCall)
            {
                
                var movies = _repository.GetMovieListForAjax(pageNum.Value, itemsPerPage, films.Movies);
                films.Movies = movies;
                return PartialView("Lists/_AjaxMovieListPartial", films);
            }
            else
            {
                films.IsFirstPage = true;

                int pageCount = (films.Movies.ToList().Count() - 1) / itemsPerPage + 1;
                ViewBag.pageCount = pageCount;
                var movies = _repository.GetMovieListForAjax(pageNum.Value, itemsPerPage, films.Movies);
                films.Movies = movies;
                return View("Films", films);
            }
        }


        //Films user has rated listed
        public IActionResult Ratings(int? pageNum)
        {
            var id = _userId.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _context.Users.Where(i => i.Id == id).FirstOrDefault();
            MovieListViewModel ratedMovies = new MovieListViewModel()
            {
                Movies = _repository.GetRatings(),
                //Directors = _repository.GetAllDirectors(),
                CurrentUser = user,
                IsFirstPage = false
            };

            ratedMovies.listCount = ratedMovies.Movies.Count();

            int itemsPerPage = 15;
            //page number (starts from 0)
            pageNum = pageNum ?? 0;

            //first time it's not ajax, next times it is
            bool isAjaxCall = HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxCall)
            {
                var newItems = _repository.GetMovieListForAjax(pageNum.Value, itemsPerPage, ratedMovies.Movies);
                ratedMovies.Movies = newItems;

                return PartialView("Lists/_AjaxMovieListPartial", ratedMovies);
            }
            else
            {
                ratedMovies.IsFirstPage = true;
                int pageCount = (ratedMovies.Movies.ToList().Count() - 1) / itemsPerPage + 1;
                ViewBag.pageCount = pageCount;
                var movies = _repository.GetMovieListForAjax(pageNum.Value, itemsPerPage, ratedMovies.Movies);
                ratedMovies.Movies = movies;
                return View("Ratings", ratedMovies);
            }
        }

        //Start custom lists
        //Displays all of the custom lists of the user!
        public ActionResult UserLists(int? pageNum)
        {
            var id = _userId.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _context.Users.Where(i => i.Id == id).FirstOrDefault();
            CListsViewModel customList = new CListsViewModel()
            {

                Lists = _repository.GetUserCLists(),
                User = user,
                IsFirstPage = false
            };
            int itemsPerPage = 3;
            pageNum = pageNum ?? 0;

            customList.listCount = customList.Lists.Count();
            
            //first time it's not ajax, next times it is
            bool isAjaxCall = HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxCall)
            {
                var newItems = _repository.GetListsListForAjax(pageNum.Value, itemsPerPage, customList.Lists);
                customList.Lists = newItems;
                return PartialView("Lists/_MyCListPartial", customList);
            }
            else
            {
                customList.IsFirstPage = true;

                int pageCount = (customList.Lists.ToList().Count() - 1) / itemsPerPage + 1;

                ViewBag.pageCount = pageCount;

                var newItems = _repository.GetListsListForAjax(pageNum.Value, itemsPerPage, customList.Lists);
                customList.Lists = newItems;
                return View("UserLists", customList);
            }
        }

    }
}
