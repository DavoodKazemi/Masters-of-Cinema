using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.ViewModels
{
    public class AddListViewModel
    {
        
        public User User { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        //Movies
        public int CListId { get; set; }
        
        public List<int> MovieId { get; set; }
        //search
        public string SearchTerm { get; set; }

        //Used for suggestions in live search
        public List<Movie> resultMovies { get; set; }
        public List<ResultMovie> suggestMovies { get; set; }

        //Click on the live search suggestions to add movies to this
        //They will be saved in the list
        public Movie MovieToAdd { get; set; }
        //Show message when user didn't add any movie
        [TempData]
        public string Message { get; set; }
    }
}
