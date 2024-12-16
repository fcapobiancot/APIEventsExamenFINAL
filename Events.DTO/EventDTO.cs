using Events.DTO;

public class EventDTO
{
    public int EventId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public bool IsPrivate { get; set; }
    public int Capacity { get; set; }
    public string OrganizerName { get; set; }
    public string ImageUrl { get; set; }
    public List<string> AttendeeNames { get; set; }
    public List<CommentDTO> Comments { get; set; }
}
