using Events.DTO;
using Events.Model;

namespace Events.BLL.Services.Contracts
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetAllUsers();
        Task<UserDTO> GetUserById(int userId);
        Task<User> CreateUser(SimpleUserDTO userDTO);
        Task<bool> UpdateUser(int userId, SimpleUserDTO userDTO);
        Task<bool> DeleteUser(int userId);
        Task<User> ValidateUser(string email, string password);
        string GenerateJwtToken(User user);
        Task<User> GetUserByEmail(string email);


    }
}
