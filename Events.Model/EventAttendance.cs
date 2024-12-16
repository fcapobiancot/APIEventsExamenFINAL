
namespace Events.Model
{
    public class EventAttendance
    {
        public int EventAttendanceId { get; set; } 

     
        public int EventId { get; set; }
        public Event Event { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime RegisteredAt { get; set; }
        public bool Attended { get; set; }

        public string Justification { get; set; }
    }

}
