using MeetPoint.API.Dtos.Comments;
using MeetPoint.API.Dtos.Common;

namespace MeetPoint.API.Services.Interfaces
{
    public interface ICommentsService
    {
		Task<ResponseDto<PaginationDto<List<CommentDto>>>> GetAllCommentsAsync(string searchTerm = "", int page = 1);
		Task<ResponseDto<CommentDto>> GetCommentByIdAsync(Guid id);
        Task<ResponseDto<CommentDto>> CreateAsync(CommentCreateDto dto);
        Task<ResponseDto<CommentDto>> EditAsync(CommentEditDto dto, Guid id);
        Task<ResponseDto<CommentDto>> DeleteAsync(Guid id);
	}
}
