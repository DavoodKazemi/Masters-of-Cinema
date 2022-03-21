using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MastersOfCinema.Data.Entities
{
    public class ListMovies
    {
        [Key]
        public int Id { get; set; }
        public int CListId { get; set; }
        public int MovieId { get; set; }
    }
}


//This entity is for storing movies of any custom list (not logged movies list or watchlists)