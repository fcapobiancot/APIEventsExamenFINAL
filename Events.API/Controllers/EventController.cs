using Events.BLL.Services.Contracts;
using Events.DTO;
using Events.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Events.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventDTO>>> GetAllEvents()
        {
            var events = await _eventService.GetAllEvents();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDTO>> GetEventById(int id)
        {
            var eventItem = await _eventService.GetEventById(id);
            if (eventItem == null)
                return NotFound();
            return Ok(eventItem);
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] EventCreateDTO eventCreateDTO)
        {
            var createdEvent = await _eventService.CreateEvent(eventCreateDTO);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.EventId }, createdEvent);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventUpdateDTO eventUpdateDTO)
        {
            var success = await _eventService.UpdateEvent(id, eventUpdateDTO);
            if (!success)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var success = await _eventService.DeleteEvent(id);
            if (!success)
                return NotFound();
            return NoContent();
        }

        [HttpGet("organizer/{organizerId}")]
        public async Task<IActionResult> GetEventsByOrganizerId(int organizerId)
        {
            try
            {
                var events = await _eventService.GetEventsByOrganizerId(organizerId);
                return Ok(events); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }


        [HttpPost("{eventId}/attend")]
        public async Task<IActionResult> RegisterForEvent(int eventId, [FromBody] int userId)
        {
            try
            {
                Console.WriteLine($"Registering user ID {userId} for event ID {eventId}");

                var result = await _eventService.RegisterForEvent(eventId, userId);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = "Successfully registered for the event." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while registering for the event.", details = ex.Message });
            }
        }


        [HttpGet("{eventId}/attendees")]
        public async Task<IActionResult> GetEventAttendees(int eventId)
        {
            try
            {
                var attendeeNames = await _eventService.GetAttendeeNames(eventId);
                return Ok(attendeeNames);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching attendee names.", details = ex.Message });
            }
        }


        [HttpDelete("{eventId}/unregister/{userId}")]
        public async Task<IActionResult> UnregisterFromEvent(int eventId, int userId)
        {
            try
            {
                var result = await _eventService.UnregisterFromEvent(eventId, userId);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = "Successfully unregistered from the event." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while unregistering from the event.", details = ex.Message });
            }
        }


        [HttpGet("{eventId}/check-attendance/{userId}")]
        public async Task<IActionResult> CheckAttendance(int eventId, int userId)
        {
            try
            {
                var isAttending = await _eventService.CheckUserAttendance(eventId, userId);
                return Ok(new { isAttending });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking attendance.", details = ex.Message });
            }
        }

        [HttpGet("{id}/attenddees")]
        public async Task<ActionResult<List<AttendeeDTO>>> GetEventAttendee(int id)
        {
            var attendees = await _eventService.GetEventAttendees(id);
            if (attendees == null)
                return NotFound(new { message = "Event not found or no attendees yet." });

            return Ok(attendees);
        }
        [HttpDelete("{eventId}/attendees/remove")]
        public async Task<IActionResult> RemoveAttendee(int eventId, [FromBody] RemoveAttendeeDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest(new { message = "Attendee name is required." });

            try
            {
                var result = await _eventService.RemoveAttendeeByName(eventId, dto.Name, dto.Justification);

                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(new { message = "Attendee removed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing the attendee.", details = ex.Message });
            }
        }


    }
}
