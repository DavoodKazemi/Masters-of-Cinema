using MastersOfCinema.Data;
using MastersOfCinema.Data.Entities;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.Controllers
{
    public class AppController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly Context context;
        private readonly ICinemaRepository repo;

        public AppController(IHttpContextAccessor httpContextAccessor, Context context,
            ICinemaRepository repo)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
            this.repo = repo;
        }
        public IActionResult Index()
        {
            var id = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = context.Users.Where(i => i.Id == id).FirstOrDefault();

            List<Review> reviews = context.Review.OrderBy(m => m.ReviewText).Take(8).ToList();
            List<ReviewViewModel> PopularReviews = new List<ReviewViewModel>();
            foreach (var item in reviews)
            {
                item.User = item.User;
                PopularReviews.Add(repo.GetReviewsLikeStats(item));
            }
            foreach(var item2 in PopularReviews)
            {
                item2.ReviewdMovie = context.Movies
                    .Where(m => m.Id == item2.MovieId).FirstOrDefault();
            }

            HomePageViewModel homePageViewModel = new HomePageViewModel()
            {
                ///
                PopularLists = new CListsViewModel() { Lists = repo.GetListsList().Take(3).ToList() },
                User = user,
                PopularReviews = PopularReviews,
            };
            

            return View(homePageViewModel);
        }
    }
}
