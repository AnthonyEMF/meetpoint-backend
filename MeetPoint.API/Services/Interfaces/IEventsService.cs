using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Events;

namespace MeetPoint.API.Services.Interfaces
{
	public interface IEventsService
	{
		Task<ResponseDto<PaginationDto<List<EventDto>>>> GetAllEventsAsync(string searchTerm = "", int page = 1);
		Task<ResponseDto<EventDto>> GetEventByIdAsync(Guid id);
		Task<ResponseDto<EventDto>> CreateAsync(EventCreateDto dto);
		Task<ResponseDto<EventDto>> EditAsync(EventEditDto dto, Guid id);
		Task<ResponseDto<EventDto>> DeleteAsync(Guid id);
	}
}
