
namespace Events.Model
{
    public class Event
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public bool IsPrivate { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; }
        // Foreign Keys
        public int OrganizerId { get; set; }
        public User Organizer { get; set; } 

        // Navigation properties
        public ICollection<EventAttendance> EventAttendances { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }

}
