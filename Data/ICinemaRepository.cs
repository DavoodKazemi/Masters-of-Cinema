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
        int GetMovieRatingCountAll(int id);
        double GetAverageRating(int id);
        IEnumerable<int> GetMovieRatingStats(int id);
        IEnumerable<double> MovieRatingChartStats(int id);
        IEnumerable<int> MovieRatingCount(int id);
        MovieRating GetRatingByMovieId(int id);
        string CurrnentUserName();
    }
}