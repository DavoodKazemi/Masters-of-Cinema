using MastersOfCinema.Data.Entities;
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
        
        
        //User profile
        string CurrnentUserName();
        IEnumerable<Movie> GetFilms();
        IEnumerable<Movie> GetWatchlist();
        MovieLog IsLoggedMovieId(int id);
        Watchlist IsInWatchlistById(int id);

    }
}