using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.ViewModels
{
    public class User : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public List<MovieRating> UserMovieRatings { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
        [Display(Name = "Profile Picture")]
        public byte[] ProfilePicture { get; set; }

    }
}
