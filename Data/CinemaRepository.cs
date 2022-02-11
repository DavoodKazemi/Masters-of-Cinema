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

        //done
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


    }
}
