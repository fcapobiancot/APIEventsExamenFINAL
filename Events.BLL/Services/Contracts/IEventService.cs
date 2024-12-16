using Events.DTO;
using Events.Model;

namespace Events.BLL.Services.Contracts
{
    public interface IEventService
    {
        Task<List<EventDTO>> GetAllEvents();
        Task<EventDTO> GetEventById(int eventId);
        Task<Event> CreateEvent(EventCreateDTO eventCreateDTO);
        Task<bool> UpdateEvent(int eventId, EventUpdateDTO eventUpdateDTO);
        Task<bool> DeleteEvent(int eventId);
        Task<List<EventDTO>> GetEventsByOrganizerId(int organizerId);
        Task<(bool Success, string Message)> RegisterForEvent(int eventId, int userId);
        Task<List<string>> GetAttendeeNames(int eventId);
        Task<(bool Success, string Message)> UnregisterFromEvent(int eventId, int userId);
        Task<bool> CheckUserAttendance(int eventId, int userId);
        Task<List<AttendeeDTO>> GetEventAttendees(int eventId);
        Task<(bool Success, string Message)> RemoveAttendeeByName(int eventId, string name, string justification);


    }
}
