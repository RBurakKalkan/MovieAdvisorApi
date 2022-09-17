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
    public class UserController : ControllerBase
    {
        [Authorize]
        [HttpPost("AddUser/{UserName}/{UserSurname}/{Email}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Users))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddUser([Required] string UserName, [Required] string UserSurname, string Email)
        {
            using (var _context = new MovieAdvisorDBContext())
            {
                Users Users = new Users();
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserSurname)|| string.IsNullOrEmpty(Email))
                {
                    return NotFound();
                }

                Users.Name = UserName;
                Users.Surname = UserSurname;

                Users.Email = Email;
                await _context.Users.AddAsync(Users);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }
    }
}
