using System.ComponentModel.DataAnnotations;


namespace MastersOfCinema.ViewModels
{
    public class ReviewViewModel
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public int MovieId { get; set; }
        public string ReviewText { get; set; }
        public int LikeCount { get; set; }
    }
}
