using MastersOfCinema.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MastersOfCinema.Data.Entities
{
    public class CList
    {
        public CList()
        {
            Movies = new List<ListMovies>();
        }
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ListMovies> Movies { get; set; }
        [NotMapped]
        public List<string> Avatars { get; set; }
    }
}

//This entity (with help of ListMovies entity) is for storing
//any custom list information (not logged movies list or watchlists)