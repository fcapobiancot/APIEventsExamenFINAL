
namespace Events.Model
{
    public class Comment
    {
        public int CommentId { get; set; } // Primary Key
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign Keys
        public int EventId { get; set; }
        public Event Event { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

}
