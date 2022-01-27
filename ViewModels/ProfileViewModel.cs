using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.ViewModels
{
    public class ProfileViewModel
    {
        //Remove the unhelpful later

        public User CurrentUser { get; set; }
        public Movie Movie { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
        public int listCount { get; set; }
        //Temporarily saves the first rating of this movie, 
        //later saves the user's rating of this movie
        public MovieRating MovieRating { get; set; }
        public Director Director { get; set; }
        public IEnumerable<Director> Directors { get; set; }

        public int RateCountAll { get; set; }
        //4 people rated this movie by 1, 15 people rated this mocie by 2, ....
        public IEnumerable<int> RateCounts { get; set; }
        //11% rated this movie by 1, 40% people rated this mocie by 2, ....
        public IEnumerable<double> RatePercents { get; set; }
        //If most people rated this movie by 4, 
        //chart bar's height for rate 4 will be 100% and so on
        public IEnumerable<double> ProportionalRatePercents { get; set; }
        public double AverageRate { get; set; }
    }
}