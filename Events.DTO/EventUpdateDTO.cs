
namespace Events.DTO
{
    public class EventUpdateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public bool IsPrivate { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; }
    }
}
