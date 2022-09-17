using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAdvisor.DataLayer;
using MovieAdvisor.Dummies;
using MovieAdvisor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAdvisor.Controllers
{
    public class MoviesGetController : ControllerBase
    {

        MovieAdvisorDBContext ctx = new MovieAdvisorDBContext();
        [Authorize]
        [HttpGet("GetMoviesByPageSize")]
        public async Task<IEnumerable<GetMoviesByPageSize>> GetMoviesByPageSize(int RecordCountPerPage)
        {
            var GetPagedMovies = new List<GetMoviesByPageSize>();
            var result = await (from c in ctx.Movies select c).ToListAsync();
            double totalPage = Convert.ToDouble(result.Count) / Convert.ToDouble(RecordCountPerPage);
            int intTotalPage = Convert.ToInt32(Math.Ceiling(totalPage)), counter = 0;

            for (int j = 1; j <= intTotalPage; j++)                                 // variable j is the page number
            {
                for (int i = 0; i < RecordCountPerPage; i++)                        // this section is to fill our dummy class.
                {
                    if (counter < result.Count)                                     // Last page is more likely to be less than given number this controls it.
                    {
                        GetMoviesByPageSize GetPagedMoviesRepo = new GetMoviesByPageSize();
                        Movies Movies = new Movies();
                        GetPagedMoviesRepo.Page = j;
                        Movies.Description = result[counter].Description;
                        Movies.Name = result[counter].Name;
                        Movies.TMDBId = result[counter].TMDBId;
                        Movies.MoviesId = result[counter].MoviesId;
                        GetPagedMoviesRepo.Movies = Movies;
                        GetPagedMovies.Add(GetPagedMoviesRepo);
                        counter++;
                    }
                }
            }
            counter = 0;
            return GetPagedMovies;
        }
        [Authorize]
        [HttpGet("GetMovieDetail")]
        public async Task<IEnumerable<GetMovieDetails>> GetMovieDetails(int MovieId)
        {
            int[] LastRatingsPerUser = ctx.MovieRating.AsQueryable().Where(x => x.MovieId == MovieId && x.Rating != -1)
                                                                   .GroupBy(u => u.UserId)
                                                                   .Select(g => g.Max(x => x.MovieRatingId)).ToArray(); // Gets last rating for each user has made

            var Ratings = await (from mr in ctx.MovieRating.AsQueryable()
                                 join u in ctx.Users on mr.UserId equals u.UsersId
                                 join m in ctx.Movies on mr.MovieId equals m.MoviesId
                                 where mr.MovieId == MovieId && mr.Rating != -1 && LastRatingsPerUser.Contains(mr.MovieRatingId)
                                 select new Ratings()
                                 {
                                     MovieId = mr.MovieId,
                                     Name = u.Name,
                                     Surname = u.Surname,
                                     Rating = mr.Rating
                                 }).ToArrayAsync();

            var Comments = await (from mr in ctx.MovieRating.AsQueryable()
                                  join u in ctx.Users on mr.UserId equals u.UsersId
                                  where mr.MovieId == MovieId
                                  select new Comments()
                                  {
                                      Name = u.Name,
                                      Surname = u.Surname,
                                      Comment = mr.Comment
                                  }).ToArrayAsync();

            double Average = 0;
            foreach (var item in Ratings)
            {
                Average += item.Rating;
            }
            Average = Ratings.Count() > 0 ? Average /= Ratings.Count() : 0;

            var MovieDetails = await (from m in ctx.Movies.AsQueryable()
                                      where m.MoviesId == MovieId
                                      select new GetMovieDetails()
                                      {
                                          MovieId = m.MoviesId,
                                          Name = m.Name,
                                          Description = m.Description,
                                          Average = Average,
                                          Comments = Comments,
                                          Ratings = Ratings
                                      }).ToListAsync();
            return MovieDetails;
        }
    }
}
