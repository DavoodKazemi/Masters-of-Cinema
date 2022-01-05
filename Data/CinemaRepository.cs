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
        private readonly Context ctx;
        private readonly ILogger<CinemaRepository> logger;

        public CinemaRepository()
        {
        }

        public CinemaRepository(Context context, ILogger<CinemaRepository> logger)
        {
            ctx = context;
            this.logger = logger;
        }

        public IEnumerable<Director> GetAllDirectors()
        {
            try
            {
                logger.LogInformation("GetAllProducts was called!!");
                return ctx.Directors
                .Include(o => o.Movies)
                .ToList();
                //return ctx.Director.OrderBy(p => p.Id).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all products: {ex}");
                return null;
            }

        }
        public Director GetDirectorById(int id)
        {
            return ctx.Directors
               .Where(o => o.Id == id)
               .Include(o => o.Movies)
               .FirstOrDefault();
            /*            return ctx.Movie.Where(p => id == p.DirectorViewModelId).ToList();
            */
        }



        public IEnumerable<Movie> GetAllMovies()
        {
            try
            {
                logger.LogInformation("GetAllProducts was called!!");
                return ctx.Movies.OrderBy(p => p.Id).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all products: {ex}");
                return null;
            }

        }

        public Movie GetMovieById(int id)
        {
            return ctx.Movies
               .Where(o => o.Id == id)
               .FirstOrDefault();
            /*            return ctx.Movie.Where(p => id == p.DirectorViewModelId).ToList();
            */
        }
    }
}
