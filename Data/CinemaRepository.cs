using MastersOfCinema.Data.Entities;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.Data
{
    public class CinemaRepository : ICinemaRepository
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly Context _context;
        private readonly ILogger<CinemaRepository> logger;

        public CinemaRepository(IHttpContextAccessor accessor, Context context, ILogger<CinemaRepository> logger)
        {
            _accessor = accessor;
            _context = context;
            this.logger = logger;
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


        //Start Movie rating methods
        //Movie Id - **Later should return the user rating of a movie**
        public MovieRating GetRatingByMovieId(int id)
        {
            try
            {
                var UserName = _accessor.HttpContext.User.Identity.Name;

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


        //20 people rated this movie
        public int GetMovieRatingCountAll(int id)
        {
            int Count = _context.MovieRatings.Where(m => m.MovieId == id).Count();
            return Count;
        }
        //Get all ratings of a specific movie
        private IEnumerable<MovieRating> GetAllMovieRatingsById(int id)
        {
            try
            {
                logger.LogInformation("GetMovieRatings was called!");
                return _context.MovieRatings.Where(m => m.MovieId == id);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all movie ratings: {ex}");
                return null;
            }

        }
        //Average score of a movie!
        public double GetAverageRating(int id)
        {
            List<MovieRating> all = GetAllMovieRatingsById(id).ToList();

            if (all.Count == 0)
            {
                return 0;
            }
            else
            {
                double sum = 0;

                foreach (var rate in all)
                {
                    sum += rate.Rating ?? 0;
                }

                double average = sum / all.Count;

                return average;
            }
        }
        public IEnumerable<int> GetMovieRatingStats(int id)
        {
            return null;
        }
        //For example: 15% of the ratings of a movie is 1, 40% rated 2, ...
        //Ex. 20 people rated a movie. 5 people rated it by 4. percent(3) = (4 / 20) * 100 = 20%
        public IEnumerable<double> MovieRatingChartStats(int id)
        {
            List<MovieRating> coll;
            double count1, count2, count3;
            List<double> percent = new List<double>();

            for (int i = 0; i < 5; i++)
            {
                //Get all ratings of i+1 of the movie
                coll = GetAllMovieRatingsById(id).Where(m => m.Rating == i + 1).ToList();
                //Ex. 4 people rated the movie by i+1
                count1 = coll.Count();
                //Ex. ALL of the people rated the movie = 20 people
                count2 = GetAllMovieRatingsById(id).Count();
                //Ex. 4 / 20 = 0.2
                if(count1!=0 && count2 != 0)
                {
                    count3 = count1 / count2;
                } else
                {
                    count3 = 0.0;
                }
                //Ex. 0.2 * 100 = 20 (%)
                percent.Add(count3 * 100);
            }

            return percent;
        }

        //For example: 
        //12 people rated a movie by 4
        //8 people rated it by 1
        //And so on
        public IEnumerable<int> MovieRatingCount(int id)
        {
            List<int> count = new List<int>();

            for (int i = 0; i < 5; i++)
            {
                count.Add(GetAllMovieRatingsById(id).Where(m => m.Rating == i + 1).Count());
            }
            return count;
        }

        //END Movie rating methods
        

        public IEnumerable<Director> GetAllDirectors()
        {
            try
            {
                logger.LogInformation("GetAllProducts was called!!");
                return _context.Directors
                .Include(o => o.Movies)
                .ToList();
                //return _context.Director.OrderBy(p => p.Id).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all products: {ex}");
                return null;
            }

        }
        public Director GetDirectorById(int id)
        {
            return _context.Directors
               .Where(o => o.Id == id)
               .Include(o => o.Movies)
               .FirstOrDefault();
        }

        public IEnumerable<Movie> GetAllMovies()
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

        //User related methods

        //ajax
        //Number of grids for each page in infinite scroll
        //public const int ItemsPerPage = 15;
        public IEnumerable<Movie> GetMoviesForAjax(int pageNum, int itemsPerPage)
        {
            //List all items
            List<Movie> movieList = _context.Movies.ToList();

            //loaded items number
            int from = (pageNum * itemsPerPage);

            //skip the loaded ones, and load the next page
            var page = movieList.Skip(from).Take(itemsPerPage);
            return page;
        }

        public IEnumerable<Movie> GetMovieListForAjax(int pageNum, int itemsPerPage, IEnumerable<Movie> movies)
        {
            //List all items
            List<Movie> movieList = movies.ToList();

            //loaded items number
            int from = (pageNum * itemsPerPage);

            //skip the loaded ones, and load the next page
            var page = movieList.Skip(from).Take(itemsPerPage);
            return page;
        }
        //end ajax
        public string CurrnentUserName()
        {
            var UserName = _accessor.HttpContext.User.Identity.Name;
            return UserName;
        }
        public IEnumerable<Movie> GetRatings()
        {
            var User = _accessor.HttpContext.User.Identity.Name;
            //List of all rates by the user - later added to watchlist
            var rateList = _context.MovieRatings.Where(r => r.User.UserName == User);

            var movies = new List<Movie>();
            //IEnumerable<Movie> movies2 = _context.Movies.Include(x => x.MovieRatings).Where(m => m.MovieRatings == rateList);
            foreach (var item in rateList)
            {
                movies.Add(_context.Movies.FirstOrDefault(m => m.Id == item.MovieId));
            }

            return movies;
        }

        //fixed
        public IEnumerable<Movie> GetWatchlist()
        {
            var User = _accessor.HttpContext.User.Identity.Name;
            //List of all rates by the user - later added to watchlist
            var watchList = _context.Watchlists.Where(r => r.User.UserName == User);

            var movies = new List<Movie>();
            //IEnumerable<Movie> movies = _context.Movies.Include(x => x.MovieRatings).Where(m => m.MovieRatings == watchList);
            foreach (var item in watchList)
            {
                movies.Add(_context.Movies.FirstOrDefault(m => m.Id == item.MovieId));
            }

            return movies;
        }

        //done - get all of the movies one user logged
        public IEnumerable<Movie> GetFilms()
        {
            var User = _accessor.HttpContext.User.Identity.Name;
            //List of all rates by the user - later added to watchlist
            var loggedList = _context.MovieLogs.Where(r => r.User.UserName == User);

            var films = new List<Movie>();
            //IEnumerable<Movie> movies = _context.Movies.Include(x => x.MovieRatings).Where(m => m.MovieRatings == watchList);
            foreach (var item in loggedList)
            {
                films.Add(_context.Movies.FirstOrDefault(m => m.Id == item.MovieId));
            }

            return films;
        }

        public MovieLog IsLoggedMovieId(int id)
        {
            var User = _accessor.HttpContext.User.Identity.Name;
            //List of all rates by the user - later added to watchlist
            var movieLog = _context.MovieLogs.Where(r => r.User.UserName == User)
                .FirstOrDefault(i => i.MovieId == id);

            return movieLog;
        }

        public Watchlist IsInWatchlistById(int id)
        {
            var User = _accessor.HttpContext.User.Identity.Name;
            //List of all rates by the user - later added to watchlist
            var watchList = _context.Watchlists.Where(r => r.User.UserName == User)
                .FirstOrDefault(i => i.MovieId == id);

            return watchList;
        }


        //Begin Custom lists

        //Get one custom list - id refers to the property of id in CList table (id = id of the custom list)
        public IEnumerable<Movie> GetCustomList(int id)
        {
            //fixed it to display the list user wants

            //Get all movie ids of the list
            var customList = _context.ListMovies.Where(r => r.CListId == id);

            var films = new List<Movie>();

            //Add movie objects to the list, using the movie ids we have
            foreach (var item in customList)
            {
                films.Add(_context.Movies.FirstOrDefault(m => m.Id == item.MovieId));
            }

            return films;
        }

        public string GetListTitle(int id)
        {
            //fixed it to get the title of the list user wants
            var title = _context.Lists.Where(r => r.Id == id).FirstOrDefault().Title;

            return title;
        }

        public string GetListDescription(int id)
        {
            //fixed it to get the Description of the list user wants
            var description = _context.Lists.Where(r => r.Id == id).FirstOrDefault().Description;

            return description;
        }

        //Get lists of the user
        public IEnumerable<CList> GetUserCLists()
        {
            var User = _accessor.HttpContext.User.Identity.Name;
            //List of all custom lists by the user!
            var customLists = _context.Lists.Include(x => x.Movies).Where(r => r.User.UserName == User);


            var lists = new List<CList>();

            //add data to the lists
            foreach (var item in customLists /*_context.Lists.Include(x => x.Movies).ToArray()*/)
            {
                if (item.User.UserName == User)
                {
                //If the list is created by this user, add it to lists
                lists.Add(item);

                item.Avatars = new List<string>();

                //Add the images of the first few movies in the list to the avatar property!
                //Join two lists in order to find the images of the movies!
                // (By matching movieId in item with id in movie table)
                var extractAvatars = item.Movies.Join(_context.Movies, prod => prod.MovieId,
                  sale => sale.Id,
                  (prod, sale) => new
                  {
                      sale.ImageName
                  }).Take(5).Reverse();

                //if there are less than 5 movies in the list
                for (int i = extractAvatars.ToList().Count(); i < 5; i++)
                {
                    item.Avatars.Add("defaultImage");
                }

                item.Avatars.AddRange(extractAvatars.Select(prods => prods.ImageName));
                //End Add images to the avatar property
                }
            }

            return lists;
        }

        //Gets list id - returns the id of the user created the list
        public string GetUserCList(int id)
        {
            string userId = _context.Lists.Where(y => y.Id == id).Include(m => m.User).FirstOrDefault().User.Id;
            
            return userId;
        }

            //Get all the custom lists one user created
            public IEnumerable<CList> GetListsList()
        {
            var User = _accessor.HttpContext.User.Identity.Name;
            //List of all custom lists by the user!
            //var customLists2 = _context.Lists.Where(r => r.User.UserName == User).Include(x => x.Movies);
            //var customLists = _context.Lists.Include(x => x.Movies);
            //var allLists = customLists.Except(customLists2).ToList();
            var allLists = _context.Lists.Include(x => x.Movies).Include(x => x.User);
            var lists = new List<CList>();

            //lists.AddRange(GetUserCLists());

            //add data to the lists
            foreach (var item in allLists /*_context.Lists.Include(x => x.Movies).ToArray()*/)
            {
                //if (item.User.UserName != User)
                //{
                   
                    //If the list is created by this user, add it to lists
                    lists.Add(item);
                    
                    item.Avatars = new List<string>();

                    //Add the images of the first few movies in the list to the avatar property!
                    //Join two lists in order to find the images of the movies!
                    // (By matching movieId in item with id in movie table)
                    var extractAvatars = item.Movies.Join(_context.Movies, prod => prod.MovieId,
                      sale => sale.Id,
                      (prod, sale) => new
                      {
                          sale.ImageName
                      }).Take(5).Reverse();
                    
                    //if there are less than 5 movies in the list
                    for (int i = extractAvatars.ToList().Count(); i < 5; i++) {
                    item.Avatars.Add("defaultImage");
                    }

                    item.Avatars.AddRange(extractAvatars.Select(prods => prods.ImageName));
                    //End Add images to the avatar property
                //}
            }

            return lists;
        }

        public IEnumerable<CList> GetListsListForAjax(int pageNum, int itemsPerPage, IEnumerable<CList> lists)
        {
            //List all items
            var listsList = lists.ToList();

            //loaded items number
            int from = (pageNum * itemsPerPage);

            

            //skip the loaded ones, and load the next page
            var page = listsList.Skip(from).Take(itemsPerPage);
            return page;
        }

        //Gets a search term and sends a list of movies
        public IEnumerable<Movie> SearchMovie(string searchTerm)
        {
            var movies = new List<Movie>();
            movies = _context.Movies.Where(m => m.Title.Contains(searchTerm)).Take(10).ToList();

            return movies;
        }

        //Gets a search term and sends a list of pure Movie objects and also ResultMovie (id, title + year + director)
        public AddListViewModel GetSuggest(string searchTerm)
        {
            AddListViewModel model = new AddListViewModel()
            {
                suggestMovies = new List<ResultMovie>(),
                resultMovies = new List<Movie>()
            };

            model.resultMovies = SearchMovie(searchTerm).ToList();

            List<ResultMovie> result = new List<ResultMovie>();

            string id;
            string info1;
            string info2; 
            foreach (var movie in model.resultMovies)
            {
                id = movie.Id.ToString();
                info1 = GetDirectorById(movie.DirectorId).Name;
                info2 = movie.Title + " (" + movie.Year + ")";
                result.Add(new ResultMovie { Id = id, MovieDirector = info1, MovieTitleYear = info2 });
            }

            model.suggestMovies = result;
            model.SearchTerm = searchTerm;
            return model;
        }
        //END Custom lists

        //Start Review

        //Gets a review id and gives the equivalent ReviewViewModel

        //return a collection of LikeReview (including users) who liked a review
        public List<LikeReview> GetReviewLikers(int reviewId)
        {
            Review review = _context.Review.Include(u => u.User).Where(m => m.Id == reviewId).FirstOrDefault();
            //string currentUser = _accessor.HttpContext.User.Identity.Name;

            //List of all of likers - Exclude the logged in user from list (if he liked the review)
            List<LikeReview> reviewLikers = _context.LikeReview.Where(g => g.ReviewId == reviewId)
                /*.Where(u => u.User.UserName != currentUser)*/.Include(u => u.User).ToList();

            /*if (_context.LikeReview.Where(x => x.ReviewId == review.Id).Any(m => m.User.UserName == currentUser))
            {
                reviewLikersExceptMe.Insert(0, new LikeReview { User = new User { UserName = "Lara" } });
            }*/

            
            return reviewLikers;
        }
        public ReviewViewModel GetReviewLikeStatsById(int reviewId)
        {
            Review review = _context.Review.Include(u => u.User).Where(m => m.Id == reviewId).FirstOrDefault();

            string currentUser = _accessor.HttpContext.User.Identity.Name;
            ReviewViewModel reviewViewModel = new ReviewViewModel()
            {
                Id = review.Id,
                MovieId = review.MovieId,
                ReviewText = review.ReviewText,
                User = review.User,
                LikeCount = _context.LikeReview.Where(x => x.ReviewId == review.Id).Count(),
                IsLiked = _context.LikeReview.Where(x => x.ReviewId == review.Id).Any(m => m.User.UserName == currentUser),
                Likers = GetReviewLikers(reviewId)
            };
            var IsRatedByReviewer = _context.MovieRatings.Where(x => x.User == review.User).Where(z => z.MovieId == review.MovieId).FirstOrDefault();
            if (IsRatedByReviewer != null)
            {
                reviewViewModel.ReviewerRate = IsRatedByReviewer.Rating;
            }

            //The like of the current user must be moved to the first of the list
            if (reviewViewModel.IsLiked && reviewViewModel.LikeCount > 1)
            {
                //save the like
                var myLike = reviewViewModel.Likers.Where(u => u.User.UserName == currentUser).FirstOrDefault();
                //remove it from the list
                reviewViewModel.Likers = reviewViewModel.Likers.Where(u => u.User.UserName != currentUser).ToList();
                //add it to the brginning
                reviewViewModel.Likers.Insert(0, myLike);
            }


            return reviewViewModel;
        }

        //Gets a review and gives the equivalent ReviewViewModel
        public ReviewViewModel GetReviewsLikeStats(Review review)
        {
            string currentUser = _accessor.HttpContext.User.Identity.Name;
            var IsRatedByReviewer = _context.MovieRatings.Where(x => x.User == review.User).Where(z => z.MovieId == review.MovieId).FirstOrDefault();
            ReviewViewModel reviewViewModel = new ReviewViewModel()
            {
                Id = review.Id,
                MovieId = review.MovieId,
                ReviewText = review.ReviewText,
                User = review.User,
                LikeCount = _context.LikeReview.Where(x => x.ReviewId == review.Id).Count(),
                IsLiked = _context.LikeReview.Where(x => x.ReviewId == review.Id).Any(m => m.User.UserName == currentUser),
                Likers = GetReviewLikers(review.Id)
            };
            if (IsRatedByReviewer != null)
            {
                reviewViewModel.ReviewerRate = IsRatedByReviewer.Rating;
            }


            //The like of the current user must be moved to the first of the list
            if (reviewViewModel.IsLiked && reviewViewModel.LikeCount > 1)
            {
                //save the like
                var myLike = reviewViewModel.Likers.Where(u => u.User.UserName == currentUser).FirstOrDefault();
                //remove it from the list
                reviewViewModel.Likers = reviewViewModel.Likers.Where(u => u.User.UserName != currentUser).ToList();
                //add it to the brginning
                reviewViewModel.Likers.Insert(0, myLike);
            }
            reviewViewModel.User = review.User;
            return reviewViewModel;
        }



        //Get id of a movie and sends it reviews, in form of
        //ReviewViewModel (review entityt + like count and is liked)
        public IEnumerable<ReviewViewModel> GetMovieReviews(int id)
        {
            //string currentUser = _accessor.HttpContext.User.Identity.Name;
            Movie movie = GetMovieById(id);


            IEnumerable<Review> reviewsRaw = _context.Review.Include(x => x.User).Where(m => m.MovieId == id);
            List<ReviewViewModel> revs = new List<ReviewViewModel>();
            foreach(var item in reviewsRaw)
            {
                //Review + the number of its liks
                revs.Add(GetReviewsLikeStats(item));
            }

            //We will have a collection of reviews and in each review, the like count is saved
            return revs;
        }



        //find out is the movie is reviewd by user
        // + get the current user's review of the movie
        public Review IsReviewed(int id)
        {

            var User = _accessor.HttpContext.User.Identity.Name;
            //List of all rates by the user - later added to watchlist
            var review = _context.Review.Include(m => m.User)
                .Where(i => i.MovieId == id).Where(u => u.User.UserName == User).FirstOrDefault();
                
            return review;
        }

        //get the current user's review of the movie
        /*public Review GetUserReview(int id)
        {
            var User = _accessor.HttpContext.User.Identity.Name;
            Movie movie = GetMovieById(id);
            Review review2 = new Review();
            Review review = _context.Review.Include(m => m.User)
                .Where(i => i.MovieId == id).Where(u => u.User.UserName == User).FirstOrDefault() ?? review2;
            return review;
        }*/
        //End Review
    }
}
