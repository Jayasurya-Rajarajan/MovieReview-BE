using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http;
using DTO;
using System.Threading.Tasks;

namespace MovieReview.Controllers
{
    public class MovieReviewController : ApiController
    {
        private MovieReviewEntities _dbContext = new MovieReviewEntities();

        [HttpGet]
        [Route("GetMovies")]
        public IHttpActionResult GetMovies()
        {
            try
            {
                var movies = _dbContext.Movies.Where(p => p.IsActive == true).ToList()
                    .Select(p => new MovieDTO { 
                        Id = p.Id,
                        MovieName = p.MovieName,
                        MoviePoster = p.MoviePoster,
                        CreatedOn = p.CreatedOn,
                        DeletedOn = p.DeletedOn,
                        IsActive = p.IsActive,
                        MovieRating = p.MovieRating
                    }).ToList().OrderByDescending(p => p.CreatedOn);
                var result = new
                {
                    status = true,
                    message = "movies retrived",
                    data = new
                    {
                        movies
                    }
                };
                return Content(HttpStatusCode.OK, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
            catch(Exception ex)
            {
                var result = new
                {
                    status = false,
                    message = ex.Message,
                    data = new
                    {
                        
                    }
                };
                return Content(HttpStatusCode.InternalServerError, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
        }

        [HttpGet]
        [Route("GetReviews")]
        public IHttpActionResult GetReviews(int movieId)
        {
            try
            {
                var reviews = _dbContext.Reviews.Where(p => p.MovieId == movieId && p.IsActive == true).ToList()
                    .Select(p => new ReviewDTO { 
                        Id = p.Id,
                        Comments = p.Comments,
                        //CreatedOn = p.CreatedOn,
                        CreatedOn = TimeZoneInfo.ConvertTimeFromUtc((DateTime)p.CreatedOn, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                        Email = p.Email,
                        DeletedBy = p.DeletedBy,
                        IsActive = p.IsActive,
                        DeletedOn = p.DeletedOn,
                        MovieId = p.MovieId
                    }).ToList().OrderByDescending(p => p.CreatedOn);
                var result = new
                {
                    status = true,
                    message = "reviews retrived",
                    data = new
                    {
                        reviews
                    }
                };
                return Content(HttpStatusCode.OK, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
            catch(Exception ex)
            {
                var result = new
                {
                    status = false,
                    message = ex.Message,
                    data = new
                    {

                    }
                };
                return Content(HttpStatusCode.InternalServerError, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
        }

        [HttpPut]
        [Route("UpdateReviews")]
        public async Task<IHttpActionResult> UpdateReviews(ReviewDTO review)
        {
            try
            {
                string message = "";
                bool status = true;
                if(review != null)
                {
                    var targetReview = _dbContext.Reviews.Where(p => p.Id == review.Id && p.MovieId == review.MovieId).FirstOrDefault();
                    if(targetReview != null)
                    {
                        targetReview.ModifiedOn = DateTime.UtcNow;
                        targetReview.Comments = review.Comments;
                        await _dbContext.SaveChangesAsync();
                        message = "Review updated";
                        status = true;
                    }
                    else
                    {
                        message = "Invalid review";
                        status = false;
                    }

                }
                else
                {
                    message = "Parameter is null";
                    status = false;
                }
                var result = new
                {
                    status = status,
                    message = message,
                    data = new
                    {
                        review
                    }
                };
                return Content(HttpStatusCode.OK, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
            catch(Exception ex)
            {
                var result = new
                {
                    status = false,
                    message = ex.Message,
                    data = new
                    {

                    }
                };
                return Content(HttpStatusCode.InternalServerError, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
        }

        [HttpDelete]
        [Route("DeleteReview")]
        public async Task<IHttpActionResult> DeleteReview(int movieId, int reviewId)
        {
            try
            {
                string message = "";
                bool status = false;
                var review = new Review();
                if(movieId > 0 && reviewId > 0)
                {
                    review = _dbContext.Reviews.Where(p => p.MovieId == movieId && p.Id == reviewId).FirstOrDefault();
                    
                    // Soft delete
                    review.IsActive = false;
                    review.DeletedOn = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();

                    // Hard delete
                    // _dbContext.Reviews.Remove(review);
                    message = "Review deleted";
                    status = true;
                }
                else
                {
                    message = "Invalid review id";
                    status = false;
                }
                var result = new
                {
                    status = status,
                    message = message,
                    data = new
                    {
                        review = new ReviewDTO
                        {
                            Id = review.Id,
                            Comments = review.Comments,
                            MovieId = review.MovieId
                        }
                    }
                };
                return Content(HttpStatusCode.OK, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
            catch(Exception ex)
            {
                var result = new
                {
                    status = false,
                    message = ex.Message,
                    data = new
                    {

                    }
                };
                return Content(HttpStatusCode.InternalServerError, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
        }

        [HttpPost]
        [Route("CreateReviews")]
        public async Task<IHttpActionResult> CreateReviews(ReviewDTO review)
        {
            try
            {
                string message = "";
                bool status = false;
                var reviewParam = new Review();
                if (review != null)
                {
                    reviewParam = new Review
                    {
                        Comments = review.Comments,
                        Email = review.Email,
                        CreatedOn = DateTime.UtcNow,
                        IsActive = true,
                        MovieId = review.MovieId
                    };
                    _dbContext.Reviews.Add(reviewParam);
                    await _dbContext.SaveChangesAsync();
                    message = "Review added";
                    status = true;
                }
                else
                {
                    message = "Invalid parameter";
                    status = false;
                }
                var result = new
                {
                    status = status,
                    message = message,
                    data = new
                    {
                        reviewParam
                    }
                };
                return Content(HttpStatusCode.OK, JToken.Parse(JsonConvert.SerializeObject(result)));

            }
            catch(Exception ex)
            {
                var result = new
                {
                    status = false,
                    message = ex.Message,
                    data = new
                    {

                    }
                };
                return Content(HttpStatusCode.InternalServerError, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
        }

        [HttpPost]
        [Route("AddMovie")]
        public async Task<IHttpActionResult> AddMovie(MovieDTO movie)
        {
            try
            {
                string message = "";
                bool status = false;
                var movieparam = new Movie();
                if (movie != null)
                {
                    movieparam = new Movie
                    {
                        MovieName = movie.MovieName,
                        MoviePoster = movie.MoviePoster,
                        MovieRating = movie.MovieRating,
                        CreatedOn = DateTime.UtcNow,
                        IsActive = true
                    };
                    movieparam = _dbContext.Movies.Add(movieparam);
                    await _dbContext.SaveChangesAsync();
                    message = "Movie added";
                    status = true;
                }
                else
                {
                    message = "Invalid parameter";
                    status = false;
                }
                var result = new
                {
                    status = status,
                    message = message,
                    data = new
                    {
                        movieparam
                    }
                };
                return Content(HttpStatusCode.OK, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
            catch(Exception ex)
            {
                var result = new
                {
                    status = false,
                    message = ex.Message,
                    data = new
                    {

                    }
                };
                return Content(HttpStatusCode.InternalServerError, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
        }

        [HttpDelete]
        [Route("DeleteMovie")]
        public async Task<IHttpActionResult> DeleteMovie(int id)
        {
            try
            {
                bool status = false;
                string message = "";
                var movie = new Movie();
                if (id > 0)
                {
                    movie = _dbContext.Movies.Where(p => p.Id == id).FirstOrDefault();
                    if(movie != null)
                    {
                        movie.IsActive = false;
                        movie.DeletedOn = DateTime.UtcNow;
                        await _dbContext.SaveChangesAsync();
                        message = "Deleted successfully";
                        status = true;
                    }
                    else
                    {
                        message = "Invalid id";
                        status = false;
                    }
                }
                else
                {
                    message = "Invalid parameter";
                    status = false;
                }
                var result = new
                {
                    status = status,
                    message = message,
                    data = new
                    {
                        movie
                    }
                };
                return Content(HttpStatusCode.OK, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
            catch(Exception ex)
            {
                var result = new
                {
                    status = false,
                    message = ex.Message,
                    data = new
                    {

                    }
                };
                return Content(HttpStatusCode.InternalServerError, JToken.Parse(JsonConvert.SerializeObject(result)));
            }
        } 

    }
}
