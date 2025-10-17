using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static readonly List<User> users = new();
        private static int nextId = 1;

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return NotFound(new { message = $"User with ID {id} not found." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user: {ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult<User> AddUser([FromBody] User newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrWhiteSpace(newUser.FirstName) ||
                    string.IsNullOrWhiteSpace(newUser.Email) ||
                    !new EmailAddressAttribute().IsValid(newUser.Email))
                {
                    return BadRequest("Invalid user data. Ensure name and email are valid.");
                }

                newUser.Id = nextId++;
                users.Add(newUser);

                return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return NotFound(new { message = $"User with ID {id} not found." });

                if (string.IsNullOrWhiteSpace(updatedUser.FirstName) ||
                    string.IsNullOrWhiteSpace(updatedUser.Email) ||
                    !new EmailAddressAttribute().IsValid(updatedUser.Email))
                {
                    return BadRequest("Invalid user data.");
                }

                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
                user.Department = updatedUser.Department;

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating user: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return NotFound(new { message = $"User with ID {id} not found." });

                users.Remove(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting user: {ex.Message}");
            }
        }
    }
}
