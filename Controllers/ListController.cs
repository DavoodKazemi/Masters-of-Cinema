using MastersOfCinema.Data;
using MastersOfCinema.Data.Entities;
using MastersOfCinema.ViewModels;
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
    public class ListController : Controller
    {
        private readonly ICinemaRepository _repository;
        private readonly Context _context;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _userId;

        public ListController(IHttpContextAccessor httpContextAccessor,
            ICinemaRepository repository, Context context, ILogger<AccountController> logger, SignInManager<User> signInManager)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
            _signInManager = signInManager;
            _userId = httpContextAccessor;
        }


        //Displays all of the custom lists of the user!
        public ActionResult Index(int? pageNum)
        {
            var id = _userId.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _context.Users.Where(i => i.Id == id).FirstOrDefault();
            CListsViewModel customList = new CListsViewModel()
            {

                Lists = _repository.GetListsList(),
                User = user,
                IsFirstPage = false
            };
            int itemsPerPage = 9;
            pageNum = pageNum ?? 0;

            customList.listCount = customList.Lists.Count();

            //first time it's not ajax, next times it is
            bool isAjaxCall = HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxCall)
            {
                var newItems = _repository.GetListsListForAjax(pageNum.Value, itemsPerPage, customList.Lists);
                customList.Lists = newItems;
                return PartialView("Lists/_CListPartial", customList);
            }
            else
            {
                customList.IsFirstPage = true;

                int pageCount = (customList.Lists.ToList().Count() - 1) / itemsPerPage + 1;

                ViewBag.pageCount = pageCount;

                var newItems = _repository.GetListsListForAjax(pageNum.Value, itemsPerPage, customList.Lists);
                customList.Lists = newItems;
                return View(customList);
            }
        }

        //Displays a custom list!
        public ActionResult Details(int? id, int? pageNum)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userId.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = _context.Users.Where(i => i.Id == userId).FirstOrDefault();

            MovieListViewModel customList = new MovieListViewModel()
            {
                Movies = _repository.GetCustomList(id.Value),
                Title = _repository.GetListTitle(id.Value),
                Description = _repository.GetListDescription(id.Value),
                //Directors = _repository.GetAllDirectors(),
                CurrentUser = user,
                IsFirstPage = false
            };

            customList.listCount = customList.Movies.Count();

            int itemsPerPage = 15;
            //page number (starts from 0)
            pageNum = pageNum ?? 0;

            //first time it's not ajax, next times it is
            bool isAjaxCall = HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxCall)
            {
                var newItems = _repository.GetMovieListForAjax(pageNum.Value, itemsPerPage, customList.Movies);
                customList.Movies = newItems;

                return PartialView("Lists/_AjaxMovieListPartial", customList);
            }
            else
            {
                customList.IsFirstPage = true;
                int pageCount = (customList.Movies.ToList().Count() - 1) / itemsPerPage + 1;
                ViewBag.pageCount = pageCount;
                ViewBag.listId = id.ToString();
                var movies = _repository.GetMovieListForAjax(pageNum.Value, itemsPerPage, customList.Movies);
                customList.Movies = movies;
                return View(customList);
            }

        }

        //End Displays a custom list!

        //Begin Create Custom list 
        [HttpGet]
        public ActionResult AddList()
        {
            return View();
        }

        //create list
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddList(AddListViewModel newList)
        {

            if (ModelState.IsValid)
            {
                //if user added no movie to the list
                if (newList.MovieId == null)
                {
                    newList.Message = "A list must include at least one film.";
                }
                //if user added one or more movies to the list
                else
                {
                    var UserName = HttpContext.User.Identity.Name;

                    //First we create a record for the list in CList table
                    //Title, Description and user id will be saved there
                    var viewModelObject = new CList
                    {
                        Title = newList.Title,
                        Description = newList.Description,
                        User = _context.Users.FirstOrDefault(u => u.UserName == UserName),
                    };
                    //Insert record
                    _context.Add(viewModelObject);
                    await _context.SaveChangesAsync();

                    //Then we create one record for each movies of the list in ListMovies table
                    //MovieId and List id will be saved there
                    int i = 0;
                    List<ListMovies> viewModelMovie = new List<ListMovies>
                    {

                    };

                    foreach (var item in newList.MovieId)
                    {
                        viewModelMovie.Add(new ListMovies { MovieId = item, CListId = viewModelObject.Id });
                        _context.Add(viewModelMovie[i]);
                        i++;
                    }

                    //Insert record
                    //_context.Add(viewModelMovie);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = viewModelObject.Id });
                }
            }
            return View(newList);
        }

        //Gets a search term - Display suggestions when user type in input of the live search
        public async Task<IActionResult> Search(string? searchTerm)
        {
            AddListViewModel model = new AddListViewModel()
            {
            };
            model = _repository.GetSuggest(searchTerm);
            return PartialView("Lists/_AjaxAddClistSuggest", model);
        }

        //Add a movie when user clicks on one of the items in the live search suggestions
        public async Task<IActionResult> AddMovieToCList(int? movieIdToAdd)
        {
            AddListViewModel model = new AddListViewModel()
            {
            };
            model.MovieToAdd = _repository.GetMovieById(movieIdToAdd.Value);
            return PartialView("Lists/_AddMoviePartial", model);

        }
        //End custom lists
    }
}
