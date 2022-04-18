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
            

            /*Start popular Review section*/
            //Get the id of the most liked reviews
            var reviews = context.LikeReview
                                    .GroupBy(q => q.ReviewId)
                                    .OrderByDescending(gp => gp.Count())
                                    .Take(8)
                                    .Select(g => g.Key).ToList();


            //Convert the selected reviews into ReviewViewModel
            List<ReviewViewModel> PopularReviews = new List<ReviewViewModel>();
            foreach (var item in reviews)
            {
                PopularReviews.Add(repo.GetReviewLikeStatsById(item));
                //item.User = item.User;
            }
            //add movies info to each review
            foreach(var item2 in PopularReviews)
            {
                item2.ReviewdMovie = context.Movies
                    .Where(m => m.Id == item2.MovieId).FirstOrDefault();
            }
            /*End popular Review section*/

            HomePageViewModel homePageViewModel;
            if (User.Identity.IsAuthenticated)
            {
                //Id of the current user
                var id = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                //Current user
                User user = context.Users.Where(i => i.Id == id).FirstOrDefault();
                homePageViewModel = new HomePageViewModel()
                {
                    ///
                    PopularLists = new CListsViewModel() { Lists = repo.GetListsList().Take(3).ToList() },
                    User = user,
                    PopularReviews = PopularReviews,
                };
            } else
            {
                homePageViewModel = new HomePageViewModel()
                {
                    ///
                    PopularLists = new CListsViewModel() { Lists = repo.GetListsList().Take(3).ToList() },
                    //User = user,
                    PopularReviews = PopularReviews,
                };
            }


            /*Start highest rated movies section*/
            //Later create a list and manually choose highest rated movies
            homePageViewModel.HighestRatedMovies = new List<Movie>();
            List<MovieRating> high = new List<MovieRating>();
            foreach (var movie in context.MovieRatings)
            {
                if(repo.GetAverageRating(movie.Id) > 4.0)
                {
                    homePageViewModel.HighestRatedMovies.Add(context.Movies.Where(x => x.Id == movie.Id).FirstOrDefault());
                }
            }

            homePageViewModel.HighestRatedMovies = homePageViewModel.HighestRatedMovies.Take(6).ToList();

            /*End highest rated movies section*/

            homePageViewModel.UserWatchlist = repo.GetWatchlist().Take(6).ToList();

            return View(homePageViewModel);
        }
    }
}
