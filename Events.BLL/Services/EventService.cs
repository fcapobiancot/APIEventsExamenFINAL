using Events.DTO;
using Events.BLL.Services.Contracts;
using Events.DAL.Repository.Contracts;
using Events.Model;
using Microsoft.EntityFrameworkCore;

namespace Events.BLL.Services
{
    public class EventService : IEventService
    {
        private readonly IGenericRepository<Event> _eventRepository;
        private readonly IGenericRepository<EventAttendance> _eventAttendanceRepository; 

        public EventService(IGenericRepository<Event> eventRepository, IGenericRepository<EventAttendance> eventAttendanceRepository)
        {
            _eventRepository = eventRepository;
            _eventAttendanceRepository = eventAttendanceRepository;
        }

        public async Task<List<EventDTO>> GetAllEvents()
        {
            try
            {
                var eventsList = await _eventRepository.Consult();

                var eventDtoList = eventsList.Select(e => new EventDTO
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    DateTime = e.DateTime,
                    Location = e.Location,
                    IsPrivate = e.IsPrivate,
                    Capacity = e.Capacity,
                    OrganizerName = e.Organizer.Name,
                    AttendeeNames = e.EventAttendances.Select(a => a.User.Name).ToList(),
                    ImageUrl = e.ImageUrl,
                    Comments = e.Comments.Select(c => new CommentDTO
                    {
                        Content = c.Content,
                        UserName = c.User.Name
                    }).ToList()
                }).ToList();

                return eventDtoList;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching events.", ex);
            }
        }

        public async Task<EventDTO> GetEventById(int eventId)
        {
            try
            {
                
                var eventsQuery = await _eventRepository.Consult(
                    e => e.EventId == eventId,
                    e => e.Organizer,
                    e => e.EventAttendances,
                    e => e.Comments
                );

                var eventEntity = await eventsQuery
                    .Include(e => e.EventAttendances)
                        .ThenInclude(a => a.User)
                    .Include(e => e.Comments)
                        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync();

                if (eventEntity == null)
                    return null;

                
                return new EventDTO
                {
                    EventId = eventEntity.EventId,
                    Title = eventEntity.Title,
                    Description = eventEntity.Description,
                    DateTime = eventEntity.DateTime,
                    Location = eventEntity.Location,
                    IsPrivate = eventEntity.IsPrivate,
                    Capacity = eventEntity.Capacity,
                    OrganizerName = eventEntity.Organizer?.Name,
                    ImageUrl = eventEntity.ImageUrl,
                    AttendeeNames = eventEntity.EventAttendances?.Select(a => a.User?.Name).ToList(),
                    Comments = eventEntity.Comments?.Select(c => new CommentDTO
                    {
                        Content = c.Content,
                        UserName = c.User?.Name
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching the event.", ex);
            }
        }





        public async Task<Event> CreateEvent(EventCreateDTO eventCreateDTO)
        {
            try
            {
                var newEvent = new Event
                {
                    Title = eventCreateDTO.Title,
                    Description = eventCreateDTO.Description,
                    DateTime = eventCreateDTO.DateTime,
                    Location = eventCreateDTO.Location,
                    IsPrivate = eventCreateDTO.IsPrivate,
                    Capacity = eventCreateDTO.Capacity,
                    ImageUrl = eventCreateDTO.ImageUrl,
                    OrganizerId = eventCreateDTO.OrganizerId
                };

                var createdEvent = await _eventRepository.Create(newEvent);
                return createdEvent;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the event.", ex);
            }
        }


        public async Task<bool> UpdateEvent(int eventId, EventUpdateDTO eventUpdateDTO)
        {
            try
            {
                var existingEvent = await _eventRepository.Get(e => e.EventId == eventId);

                if (existingEvent == null)
                    return false;

                existingEvent.Title = eventUpdateDTO.Title;
                existingEvent.Description = eventUpdateDTO.Description;
                existingEvent.DateTime = eventUpdateDTO.DateTime;
                existingEvent.Location = eventUpdateDTO.Location;
                existingEvent.IsPrivate = eventUpdateDTO.IsPrivate;
                existingEvent.Capacity = eventUpdateDTO.Capacity;
                existingEvent.ImageUrl = eventUpdateDTO.ImageUrl;

                return await _eventRepository.Update(existingEvent);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the event.", ex);
            }
        }

        public async Task<bool> DeleteEvent(int eventId)
        {
            try
            {
                var existingEvent = await _eventRepository.Get(e => e.EventId == eventId);

                if (existingEvent == null)
                    return false;

                return await _eventRepository.Delete(existingEvent);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while deleting the event.", ex);
            }
        }

        public async Task<List<EventDTO>> GetEventsByOrganizerId(int organizerId)
        {
            var events = await _eventRepository.Consult(e => e.OrganizerId == organizerId);

            return events.Select(e => new EventDTO
            {
                EventId = e.EventId,
                Title = e.Title,
                Description = e.Description,
                DateTime = e.DateTime,
                Location = e.Location,
                IsPrivate = e.IsPrivate,
                Capacity = e.Capacity,
                ImageUrl = e.ImageUrl
            }).ToList();
        }

        public async Task<(bool Success, string Message)> RegisterForEvent(int eventId, int userId)
        {
           

            

            var eventEntity = await _eventRepository.Get(e => e.EventId == eventId);
            if (eventEntity == null)
            {
                return (false, "Event not found.");
            }

            
            if (eventEntity.Capacity <= 0)
            {
                return (false, "Event is fully booked.");
            }

            
            var attendanceQuery = await _eventAttendanceRepository.Consult(a => a.EventId == eventId && a.UserId == userId);
            var isAlreadyRegistered = attendanceQuery.Any(); 

            if (isAlreadyRegistered)
            {
                return (false, "User is already registered for this event.");
            }

            


            var newAttendance = new EventAttendance
            {
                EventId = eventId,
                UserId = userId,
                RegisteredAt = DateTime.UtcNow,
                Attended = true ,
                Justification = ""
            };
            await _eventAttendanceRepository.Create(newAttendance);

            
            eventEntity.Capacity -= 1;
            await _eventRepository.Update(eventEntity);

            return (true, "Successfully registered for the event.");
        }

        public async Task<List<string>> GetAttendeeNames(int eventId)
        {
            try
            {
                
                var eventAttendanceQuery = await _eventAttendanceRepository.Consult(
                    a => a.EventId == eventId,
                    a => a.User 
                );

                
                var attendeeNames = eventAttendanceQuery.Select(a => a.User.Name).ToList();
                return attendeeNames;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while fetching attendees for EventId {eventId}.", ex);
            }
        }

        public async Task<List<AttendeeDTO>> GetEventAttendees(int eventId)
        {
            var attendees = await _eventAttendanceRepository.Consult(
                a => a.EventId == eventId, 
                a => a.User
            );

            return attendees.Select(a => new AttendeeDTO
            {
                UserId = a.UserId,
                Name = a.User.Name,
                Email = a.User.Email,
                RegisteredAt = a.RegisteredAt
            }).ToList();
        }


        public async Task<(bool Success, string Message)> UnregisterFromEvent(int eventId, int userId)
        {
            
            var eventEntity = await _eventRepository.Get(e => e.EventId == eventId);
            if (eventEntity == null)
            {
                return (false, "No se encontraron eventos con el ID proporcionado.");
            }

            
            var attendance = await _eventAttendanceRepository.Get(a => a.EventId == eventId && a.UserId == userId);
            if (attendance == null)
            {
                return (false, "No estas registrado para este evento.");
            }

            
            await _eventAttendanceRepository.Delete(attendance);

           
            eventEntity.Capacity += 1;
            await _eventRepository.Update(eventEntity);

            return (true, "exitosament eliminado.");
        }



        public async Task<bool> CheckUserAttendance(int eventId, int userId)
        {
            var attendance = await _eventAttendanceRepository.Get(a => a.EventId == eventId && a.UserId == userId);
            return attendance != null;
        }



        public async Task<(bool Success, string Message)> RemoveAttendeeByName(int eventId, string name, string justification)
        {
            try
            {
                
                var attendanceRecord = await _eventAttendanceRepository.Get(a =>
                    a.EventId == eventId && a.User.Name == name);

                if (attendanceRecord == null)
                    return (false, "No se encontró un asistente con el nombre proporcionado.");

               
                await _eventAttendanceRepository.Delete(attendanceRecord);

                
                

                return (true, "asistente eliminado.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al eliminar el asistente.", ex);
            }
        }



    }
}
