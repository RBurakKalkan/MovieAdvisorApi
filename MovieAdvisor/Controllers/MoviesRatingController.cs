using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAdvisor.DataLayer;
using MovieAdvisor.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAdvisor.Controllers
{
    public class MoviesRatingController : ControllerBase
    {
        [Authorize]
        [HttpPost("AddRating/{UserId}/{MovieId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MovieRating))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddRating([Required] int UserId, [Required] int MovieId, string? Comment = null, int Rating = -1)
        {
            using (var _context = new MovieAdvisorDBContext())
            {
                MovieRating moviesRating = new MovieRating();
                if (!_context.Movies.Any(x => x.MoviesId == MovieId) || !_context.Users.Any(x => x.UsersId == UserId))
                {
                    return NotFound();
                }

                moviesRating.MovieId = MovieId;
                moviesRating.UserId = UserId;

                if (Rating <= 10 && Rating >= -1)
                {
                    moviesRating.Rating = Rating;
                }
                else
                {
                    moviesRating.Rating = -1;
                }
                moviesRating.Comment = Comment;
                moviesRating.RecordDate = System.DateTime.Now;
                await _context.MovieRating.AddAsync(moviesRating);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }
    }
}
