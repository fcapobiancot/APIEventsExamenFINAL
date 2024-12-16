using Events.DTO;
using Events.BLL.Services.Contracts;
using Events.DAL.Repository.Contracts;
using Events.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;


namespace Events.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Event> _eventRepository;
        private readonly IGenericRepository<EventAttendance> _eventAttendanceRepository;
        private readonly string _secretKey;

        public UserService(
            IGenericRepository<User> userRepository,
            IGenericRepository<Event> eventRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _secretKey = configuration["JwtSettings:SecretKey"];
        }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var users = await _userRepository.Consult();
            return users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            }).ToList();
        }

        public async Task<UserDTO> GetUserById(int userId)
        {
            var user = await _userRepository.Get(u => u.UserId == userId);

            if (user == null)
                return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                PasswordHash = user.PasswordHash,
                CreatedEvents = user.CreatedEvents?.Select(e => new EventDTO
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    DateTime = e.DateTime,
                    Location = e.Location
                }).ToList(),
                Notifications = user.Notifications?.Select(n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    SentAt = n.SentAt
                }).ToList(),
                AttendingEvents = user.EventAttendances?.Select(a => new EventDTO
                {
                    EventId = a.Event.EventId,
                    Title = a.Event.Title,
                    Description = a.Event.Description,
                    DateTime = a.Event.DateTime,
                    Location = a.Event.Location
                }).ToList()
            };
        }

        public async Task<User> CreateUser(SimpleUserDTO userDTO)
        {
            var newUser = new User
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                PasswordHash = userDTO.PasswordHash,
                Role = userDTO.Role
            };

            return await _userRepository.Create(newUser);
        }

        public async Task<bool> UpdateUser(int userId, SimpleUserDTO userDTO)
        {
            var user = await _userRepository.Get(u => u.UserId == userId);

            if (user == null)
                return false;

            
            if (!string.IsNullOrEmpty(userDTO.Email) && userDTO.Email != user.Email)
            {
                var emailExists = await _userRepository.Consult(u => u.Email == userDTO.Email);
                if (emailExists.Any())
                {
                    throw new ApplicationException("The email is already in use by another user.");
                }

                user.Email = userDTO.Email;
            }

            
            if (!string.IsNullOrEmpty(userDTO.Name))
                user.Name = userDTO.Name;

            if (!string.IsNullOrEmpty(userDTO.PasswordHash))
                user.PasswordHash = userDTO.PasswordHash;

            if (!string.IsNullOrEmpty(userDTO.Role))
                user.Role = userDTO.Role;

            return await _userRepository.Update(user);
        }


        public async Task<bool> DeleteUser(int userId)
        {
            var user = await _userRepository.Get(u => u.UserId == userId);

            if (user == null)
                return false;

            return await _userRepository.Delete(user);
        }

        public async Task<User> ValidateUser(string email, string password)
        {
            var user = await _userRepository.Get(u => u.Email == email);
            if (user == null || user.PasswordHash != password) 
                return null;

            return user;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            
            var user = await _userRepository.Get(u => u.Email == email);
            return user;
        }


        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        


    }
}
