using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Ratings;

namespace MeetPoint.API.Services.Interfaces
{
	public interface IRatingsService
	{
		Task<ResponseDto<PaginationDto<List<RatingDto>>>> GetPaginationRatingsAsync(string searchTerm = "", int page = 1);
		Task<ResponseDto<RatingDto>> GetRatingByIdAsync(Guid id);
		Task<ResponseDto<RatingDto>> CreateAsync(RatingCreateDto dto);
		Task<ResponseDto<RatingDto>> EditAsync(RatingEditDto dto, Guid id);
		Task<ResponseDto<RatingDto>> DeleteAsync(Guid id);
	}
}
