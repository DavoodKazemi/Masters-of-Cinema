using MastersOfCinema.Data;
using MastersOfCinema.Data.Entities;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.Controllers
{
    public class ReviewController: Controller
    {
        private readonly Context context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICinemaRepository repository;
        private readonly UserManager<User> userManager;

        public ReviewController(Context context,
            IWebHostEnvironment webHostEnvironment,
            ICinemaRepository repository,
            UserManager<User> userManager)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
            this.repository = repository;
            this.userManager = userManager;
        }

        //Post review - save it to database - display it
        [HttpPost]
        public async Task<IActionResult> Review(MovieRateDirector movieRateDirector)
        {
            //Only need to get movieId and the review text from view
            //Need to set a movieId and User to the new record

            //Start saving the review
            //create a new review object for saving into database
            Review newReview = new Review()
            {
                //add review text
                ReviewText = movieRateDirector.UserReview.ReviewText

            };
            //add movie Id
            var movieId = movieRateDirector.UserReview.MovieId;
            newReview.MovieId = movieId;

            //add user (reviewer)
            var UserName = HttpContext.User.Identity.Name;
            newReview.User = context.Users.FirstOrDefault(u => u.UserName == UserName);

            newReview.Id = 0;

            if (ModelState.IsValid)
            {
                //Save (Create or update) rating in DB
                context.Update(newReview);
                await context.SaveChangesAsync();
            }
            //End saving the review

            //create a new model for sending the new review to the view
            MovieRateDirector movieRateDirector2 = new MovieRateDirector();

            //add like count and isLiked to the new review
            //(probably would be better to add them manually: likeCount = 0, isLiked = false)
            movieRateDirector2.UserReview = repository.GetReviewsLikeStats(newReview);

            //Display the new review
            return PartialView("Review/_AjaxReview", movieRateDirector2);
            //End create and send model
        }

        //Like / Unlike a review
        [HttpPost]
        public async Task<IActionResult> LikeReview(int ReviewId)
        {
            //If user not logged in, they can't like review
            if (User.Identity.IsAuthenticated == false)
            {
                return BadRequest("You should login first");
            }

            //Only need to get Review Id from view
            //Need to set a Review Id and User to the new record
            LikeReview like = new LikeReview();
            var reviewId = ReviewId;
            //Check to see if this review had been liked by this user before
            var UserName = HttpContext.User.Identity.Name;

            like.User = context.Users.FirstOrDefault(u => u.UserName == UserName);

            //If true it had been liked by this user before (meaning user wants to unlike it now)
            bool wasLiked = context.LikeReview.Where(u => u.User.UserName == UserName).Any(m => m.ReviewId == ReviewId);

            //If wasLiked != true, it's a like request, else it's an unlike request
            if (!wasLiked)
            {
                //Create record
                like.Id = 0;
                like.ReviewId = ReviewId;
                if (ModelState.IsValid)
                {
                    //Save (Create) like in DB
                    context.Update(like);
                    await context.SaveChangesAsync();
                }
            }
            else //it's a delete (unlike) request
            {
                //Delete like
                var logItem = context.LikeReview.Where(u => u.User.UserName == UserName)
                .FirstOrDefault(m => m.ReviewId == ReviewId);
                context.LikeReview.Remove(logItem);
                await context.SaveChangesAsync();

            }
            return Ok("Form Data received!");
        }


        //Edit a review
        [HttpPost]
        public async Task<IActionResult> BeginEditReview(string reviewId)
        {
            //create a new model for sending the new review to the view
            //MovieRateDirector movieRateDirector = new MovieRateDirector();
            int id = Int32.Parse(reviewId);
            var existingUserReview = repository.GetReviewLikeStatsById(id);

            //create a function to get it by id directly

            //add like count and isLiked to the new review
            //(probably would be better to add them manually: likeCount = 0, isLiked = false)
            /*movieRateDirector.UserReview = repository.GetMovieReviews(1).FirstOrDefault();

            return PartialView("Review/_EditReviewPartial", movieRateDirector);*/

            MovieRateDirector movieRateDirector2 = new MovieRateDirector();

            Review review = context.Review.FirstOrDefault();
            //add like count and isLiked to the new review
            //(probably would be better to add them manually: likeCount = 0, isLiked = false)
            movieRateDirector2.UserReview = existingUserReview;

            //Display the new review
            return PartialView("Review/_EditReview", movieRateDirector2);
        }

        //Update a review
        [HttpPost]
        public async Task<IActionResult> UpdateReview(MovieRateDirector newTextId)
        {
            var review = await context.Review.Include(x => x.User).FirstOrDefaultAsync(m => m.Id == newTextId.UserReview.Id);
            var newText = newTextId.UserReview.ReviewText;

            //if (ModelState.IsValid)
            //{
            //the new text has to be different and also has to not be empty
            if (review.ReviewText != newText && newTextId.UserReview.ReviewText != "")
            {
                //If image not uploaded, assign the default photo
                review.ReviewText = newText;
            }

            var local = context.Set<Review>()
            .Local
            .FirstOrDefault(entry => entry.Id.Equals(review.Id));
            // check if local is not null 
            if (local != null)
            {
                // detach
                context.Entry(local).State = EntityState.Detached;
            }

            context.Update(review);
            await context.SaveChangesAsync();

            //create a new model for sending the new review to the view

            //add like count and isLiked to the new review
            //(probably would be better to add them manually: likeCount = 0, isLiked = false)
            newTextId.UserReview = repository.GetReviewLikeStatsById(review.Id);

            //}
            return PartialView("Review/_AjaxReview", newTextId);
        }


        [HttpPost]
        public async Task<IActionResult> CancelReview(MovieRateDirector newTextId)
        {
            var review = await context.Review.Include(x => x.User).FirstOrDefaultAsync(m => m.Id == newTextId.UserReview.Id);

            //add like count and isLiked to the new review
            //(probably would be better to add them manually: likeCount = 0, isLiked = false)
            newTextId.UserReview = repository.GetReviewLikeStatsById(review.Id);

            //}
            return PartialView("Review/_AjaxReview", newTextId);
        }


        //Delete a review
        [HttpPost]
        public async Task<IActionResult> DeleteReview(MovieRateDirector movieIdReviewId)
        {
            //int id = Int32.Parse(reviewId);
            //Define a methid for get review by id
            var reviewId = movieIdReviewId.UserReview.Id;
            var review = await context.Review.Include(x => x.User).FirstOrDefaultAsync(m => m.Id == reviewId);

            //delete the likes of the review too
            IEnumerable<LikeReview> reviewLikes = context.LikeReview.Where(x => x.ReviewId == review.Id).ToList();
            foreach (var item in reviewLikes)
            {
                context.LikeReview.Remove(item);
            }

            //delete the the review itself
            context.Review.Remove(review);
            await context.SaveChangesAsync();
            //Get movie id directly if possible
            movieIdReviewId.Movie = new Movie { Id = movieIdReviewId.MovieRating.MovieId };
            //empty the review part of the model
            //scroll to the text area after del
            //add a confirm for deletion
            return PartialView("Review/_EmptyReview", movieIdReviewId);
        }
    }
}
