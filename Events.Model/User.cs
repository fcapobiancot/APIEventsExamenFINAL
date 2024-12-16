
namespace Events.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        

        
        public ICollection<Event> CreatedEvents { get; set; }
        public ICollection<EventAttendance> EventAttendances { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }

}
