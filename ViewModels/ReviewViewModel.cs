using MastersOfCinema.Data.Entities;
using System.Collections;
using System.Collections.Generic;
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
        public bool IsLiked { get; set; }
        public int? ReviewerRate { get; set; }
        public List<LikeReview> Likers { get; set; }
        //For homepage only - to display the movie poster
        public Movie ReviewdMovie { get; set; }
    }
}

//Each review in a page is displayed by this
//including like count and also determining if this review is liked by the user or not
