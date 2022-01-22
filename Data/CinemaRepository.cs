using MastersOfCinema.ViewModels;
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
        private readonly Context _context;
        private readonly ILogger<CinemaRepository> logger;

        //Movie rating methods
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
        //END Movie rating methods
        public CinemaRepository(Context context, ILogger<CinemaRepository> logger)
        {
            _context = context;
            this.logger = logger;
        }

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
            /*            return _context.Movie.Where(p => id == p.DirectorViewModelId).ToList();
            */
        }



        public IEnumerable<Movie> GetAllMovies()
        {
            try
            {
                logger.LogInformation("GetAllProducts was called!!");
                return _context.Movies.OrderBy(p => p.Id).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all products: {ex}");
                return null;
            }

        }

        public Movie GetMovieById(int id)
        {
            return _context.Movies
               .Where(o => o.Id == id)
               .FirstOrDefault();
            /*            return _context.Movie.Where(p => id == p.DirectorViewModelId).ToList();
            */
        }
    }
}
