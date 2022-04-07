using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.ViewModels
{
    public class HomePageViewModel
    {
        public CListsViewModel PopularLists { get; set; }
        //Later check if you really need user!
        public User User { get; set; }

        public List<ReviewViewModel> PopularReviews { get; set; }
        //Highly rated movies - ex. 36 movies
        public List<Movie> HighestRatedMovies { get; set; }
        //user watchlist
        public List<Movie> UserWatchlist { get; set; }
    }
}
