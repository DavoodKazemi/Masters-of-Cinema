using MastersOfCinema.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MastersOfCinema.Data.Entities
{
    public class CList
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}

//This entity (with help of ListMovies entity) is for storing
//any custom list information (not logged movies list or watchlists)