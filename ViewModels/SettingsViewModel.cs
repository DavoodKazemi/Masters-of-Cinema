using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.ViewModels
{
    public class SettingsViewModel
    {
        //Change Password

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        //Change Password
        public string NewEmail { get; set; }
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
