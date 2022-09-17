using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MovieAdvisor.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MovieAdvisor.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace MovieAdvisor.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        MovieAdvisorDBContext DbContext = new MovieAdvisorDBContext();
        public MoviesController(IConfiguration iConfig)
        {
            Configuration = iConfig;
        }

        [HttpGet]
        public async Task<IEnumerable<Movies>> GetTMDBMoviesFromJson()
        {
            var moviesList = new List<Movies>();
            int maxMovieNum = 300;                                                      // indicates how many movies we're gonna get. 300 for this example, since all the data takes too long.
            for (int i = 1; i < 501; i++)                                               // TMDB api lets you get 500 pages max that's the reason.
            {
                string json = await httpRequest("https://api.themoviedb.org/3/discover/movie?api_key=" + Configuration["TMDB:ApiKey"] + "&page=" + i);
                dynamic record = JsonConvert.DeserializeObject(json);

                foreach (var item in record.results)
                {
                    Movies movies = new Movies();
                    movies.TMDBId = item.id;
                    movies.Description = item.overview;
                    movies.Name = item.title;
                    moviesList.Add(movies);
                    if (moviesList.Count == maxMovieNum)
                    {
                        break;
                    }
                }
                if (moviesList.Count == maxMovieNum)
                {
                    break;
                }
            }

            return moviesList;
        }
        [HttpPost("AddDb")]
        public async Task<IActionResult> AddDb()
        {
            using (var _context = new MovieAdvisorDBContext())
            {
                try
                {
                    var movies = await GetTMDBMoviesFromJson();                                                         // Gets data from TMDB
                    var missingRecords = movies.Where(x => !_context.Movies.Any(z => z.TMDBId == x.TMDBId)).ToList();   // Gets missing data from our db comparing to TMDB data
                    await _context.Movies.AddRangeAsync(missingRecords);                                                // inserts only the missing data
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch 
                {
                    return BadRequest();
                }
            }
        }
        [Authorize]
        [HttpPost("SendMailFromUserToUser/{FromUserId}/{Password}/{ToUserId}/{MovieId}")]
        public async Task SendMailU2U(int FromUserId, string Password, int ToUserId, int MovieId)                       // Sends Email from our users data to users
        {
            using (var _context = new MovieAdvisorDBContext())
            {
                var movie = await (from m in _context.Movies.AsQueryable()
                                   where m.MoviesId == MovieId
                                   select new
                                   {
                                       MovieName = m.Name,
                                       Description = m.Description
                                   }).FirstOrDefaultAsync();
                var fromUser = await (from u in _context.Users.AsQueryable()
                                      where u.UsersId == FromUserId
                                      select new
                                      {
                                          Email = u.Email
                                      }).FirstOrDefaultAsync();
                var toUser = await (from u in _context.Users.AsQueryable()
                                    where u.UsersId == ToUserId
                                    select new
                                    {
                                        Email = u.Email
                                    }).FirstOrDefaultAsync();

                SendEmail.Email(movie.MovieName + "\n" + movie.Description, fromUser.Email, Password, toUser.Email, "Movie Advise");
            }
        }
        [Authorize]
        [HttpPost("SendEmailFromConfig/{ToEmail}/{MovieId}")]
        public async Task SendMail(string ToEmail, int MovieId) // sends email from our automated email which is specified in appsettings.json to given mail address
        {
            using (var _context = new MovieAdvisorDBContext())
            {
                var movie = await (from m in _context.Movies.AsQueryable()
                                   where m.MoviesId == MovieId
                                   select new
                                   {
                                       MovieName = m.Name,
                                       Description = m.Description
                                   }).FirstOrDefaultAsync();

                SendEmail.Email(movie.MovieName + "\n" + movie.Description, Configuration["Mail:Address"], Configuration["Mail:Password"], ToEmail, "Movie Advise");
            }
        }
        private async Task<string> httpRequest(string requestUri)                                                   // Gets json object from TMDB asynchronously.
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(requestUri);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
