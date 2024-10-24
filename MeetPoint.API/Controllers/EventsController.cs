using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Events;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeetPoint.API.Controllers
{
	[ApiController]
	[Route("api/events")]
	public class EventsController : ControllerBase
	{
		private readonly IEventsService _eventsService;

		public EventsController(IEventsService eventsService)
        {
			this._eventsService = eventsService;
		}

		[HttpGet]
		public async Task<ActionResult<ResponseDto<List<EventDto>>>> GetAll(string searchTerm = "", int page = 1)
		{
			var response = await _eventsService.GetAllEventsAsync(searchTerm, page);
			return StatusCode(response.StatusCode, response);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ResponseDto<EventDto>>> Get(Guid id)
		{
			var response = await _eventsService.GetEventByIdAsync(id);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost]
		public async Task<ActionResult<ResponseDto<EventDto>>> Create(EventCreateDto dto)
		{
			var response = await _eventsService.CreateAsync(dto);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<ResponseDto<EventDto>>> Edit(EventEditDto dto, Guid id)
		{
			var response = await _eventsService.EditAsync(dto, id);
			return StatusCode(response.StatusCode, response);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<ResponseDto<EventDto>>> Delete(Guid id)
		{
			var response = await _eventsService.DeleteAsync(id);
			return StatusCode(response.StatusCode, response);
		}
	}
}
