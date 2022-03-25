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
        IEnumerable<Movie> GetRatings();

        //custom lists
        IEnumerable<Movie> GetCustomList(int id);
        IEnumerable<CList> GetListsListForAjax(int pageNum, int itemsPerPage, IEnumerable<CList> lists);
        string GetListTitle(int id);
        string GetListDescription(int id);
        IEnumerable<CList> GetListsList();
        MovieLog IsLoggedMovieId(int id);
        Watchlist IsInWatchlistById(int id);
        //Search for create list
        IEnumerable<Movie> SearchMovie(string searchTerm);
        AddListViewModel GetSuggest(string searchTerm);
        //Infinity scroll
        IEnumerable<Movie> GetMoviesForAjax(int pageNum, int itemsPerPage);
        IEnumerable<Movie> GetMovieListForAjax(int pageNum, int itemsPerPage, IEnumerable<Movie> movies);
        //Review
        IEnumerable<ReviewViewModel> GetMovieReviews(int id);
        Review IsReviewed(int id);
        //Review GetUserReview(int id);
    }
}