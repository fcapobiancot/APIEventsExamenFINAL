
namespace Events.Model
{
    public class Notification
    {
        public int NotificationId { get; set; } // Primary Key
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public User User { get; set; }
    }

}
