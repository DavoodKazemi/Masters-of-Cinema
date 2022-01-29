using MastersOfCinema.Data;
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

            return View();
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
                        return RedirectToAction("Profile");
                    }
                }
            }
                ModelState.AddModelError("", "Failed to login");

            return View();
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
        public IActionResult Watchlist()
        {
            var id = _userId.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _context.Users.Where(i => i.Id == id).FirstOrDefault();
            var watchList = new ProfileViewModel()
            {
                Movies = _repository.GetWatchlist(),
                Directors = _repository.GetAllDirectors(),
                CurrentUser = user,
                
            };
            watchList.listCount = watchList.Movies.Count();

            return View(watchList);
        }

        //Films user has watched (logged) listed
        public IActionResult Films()
        {
            var id = _userId.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _context.Users.Where(i => i.Id == id).FirstOrDefault();
            var films = new ProfileViewModel()
            {
                Movies = _repository.GetFilms(),
                Directors = _repository.GetAllDirectors(),
                CurrentUser = user
            };
            films.listCount = films.Movies.Count();

            return View(films);
        }
    }
}
