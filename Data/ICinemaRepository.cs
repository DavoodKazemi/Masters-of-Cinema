using MastersOfCinema.ViewModels;
using System.Collections.Generic;

namespace MastersOfCinema.Data
{
    public interface ICinemaRepository
    {
        IEnumerable<Director> GetAllDirectors();
        Director GetDirectorById(int id);
        IEnumerable<Movie> GetAllMovies();
        Movie GetMovieById(int id);
    }
}