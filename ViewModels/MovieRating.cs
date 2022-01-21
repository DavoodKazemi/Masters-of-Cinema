using System.ComponentModel.DataAnnotations;

namespace MastersOfCinema.ViewModels
{
    public class MovieRating
    {
        [Key]
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int? Rating { get; set; }
    }
}
