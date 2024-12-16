
namespace Events.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public List<EventDTO> CreatedEvents { get; set; } = new List<EventDTO>();
        public List<NotificationDTO> Notifications { get; set; } = new List<NotificationDTO>();
        public List<EventDTO> AttendingEvents { get; set; } = new List<EventDTO>();
    }
}
