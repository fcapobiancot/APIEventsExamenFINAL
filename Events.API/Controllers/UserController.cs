using Microsoft.AspNetCore.Mvc;
using Events.BLL.Services.Contracts;
using Events.DTO;
using Events.Model;

namespace Events.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] SimpleUserDTO userDTO)
        {
            var user = await _userService.CreateUser(userDTO);
            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] SimpleUserDTO userDTO)
        {
            if (userDTO == null)
            {
                return BadRequest(new { message = "Invalid payload." });
            }

            try
            {
                var success = await _userService.UpdateUser(id, userDTO);

                if (!success)
                {
                    return NotFound(new { message = "User not found." });
                }

                return NoContent(); 
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUser(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPost("signup")]
        public async Task<ActionResult> Signup([FromBody] SimpleUserDTO userDTO)
        {
            var existingUser = await _userService.GetUserByEmail(userDTO.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email is already in use." });

            var newUser = await _userService.CreateUser(userDTO);

            var token = _userService.GenerateJwtToken(newUser);

            return Ok(new
            {
                token,
                user = new
                {
                    userId = newUser.UserId,
                    name = newUser.Name,
                    email = newUser.Email,
                    role = newUser.Role
                }
            });
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await _userService.ValidateUser(loginDTO.Email, loginDTO.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            var token = _userService.GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.Name,
                    user.Email,
                    user.Role
                }
            });
        }



    }
}
