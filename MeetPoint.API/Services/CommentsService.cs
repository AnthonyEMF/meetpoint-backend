using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Comments;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly MeetPointContext _context;
		private readonly IAuditService _auditService;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;
        private readonly int PAGE_SIZE;

		public CommentsService(
            MeetPointContext context,
			IAuditService auditService,
            IMapper mapper,
            ILogger<CommentsService> logger,
            IConfiguration configuration)
        {
            this._context = context;
			this._auditService = auditService;
			this._mapper = mapper;
			this._logger = logger;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

        public async Task<ResponseDto<PaginationDto<List<CommentDto>>>> GetAllCommentsAsync(string searchTerm = "", int page = 1)
        {
			int startIndex = (page - 1) * PAGE_SIZE;

            var commentsEntityQuery = _context.Comments
				.Include(c => c.User)
				.Include(c => c.Event)
				.Include(c => c.Replies)
				.Where(c => c.PublicationDate <= DateTime.Now);

			if (!string.IsNullOrEmpty(searchTerm))
			{
				commentsEntityQuery = commentsEntityQuery
					.Where(c => (c.Content + " " + c.User.FirstName + " " + c.User.LastName)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			var totalComments = await commentsEntityQuery.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalComments / PAGE_SIZE);

			var commentsEntity = await commentsEntityQuery
				.OrderByDescending(u => u.PublicationDate)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var commentsDto = _mapper.Map<List<CommentDto>>(commentsEntity);

			return new ResponseDto<PaginationDto<List<CommentDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<CommentDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalComments,
					TotalPages = totalPages,
					Items = commentsDto,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages
				}
			};
		}

        public async Task<ResponseDto<CommentDto>> GetCommentByIdAsync(Guid id)
        {
            var commentEntity = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Event)
				.Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commentEntity is null)
            {
				return new ResponseDto<CommentDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

            var commentDto = _mapper.Map<CommentDto>(commentEntity);

			return new ResponseDto<CommentDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORD_FOUND,
				Data = commentDto
			};
		}

        public async Task<ResponseDto<CommentDto>> CreateAsync(CommentCreateDto dto)
		{
			try
			{
				// Validar que el evento del comentario existe
				var existingEvent = await _context.Events.FindAsync(dto.EventId);
				if (existingEvent is null)
				{
					return new ResponseDto<CommentDto>
					{
						StatusCode = 404,
						Status = false,
						Message = $"EventId: {MessagesConstant.RECORD_NOT_FOUND}"
					};
				}

				// Validar que el ParentId del comentario existe, si se proporciona
				if (dto.ParentId != null)
				{ 
					var existingParentComment = await _context.Comments.FindAsync(dto.ParentId);
					if (existingParentComment is null)
					{ 
						return new ResponseDto<CommentDto> 
						{ 
							StatusCode = 404, 
							Status = false, 
							Message = $"ParentId: {MessagesConstant.RECORD_NOT_FOUND}" 
						}; 
					} 
				}

				var commentEntity = _mapper.Map<CommentEntity>(dto);
				commentEntity.PublicationDate = DateTime.Now;
				commentEntity.UserId = _auditService.GetUserId();

				_context.Comments.Add(commentEntity);
				await _context.SaveChangesAsync();

				var commentDto = _mapper.Map<CommentDto>(commentEntity);

				return new ResponseDto<CommentDto>
				{
					StatusCode = 201,
					Status = true,
					Message = MessagesConstant.CREATE_SUCCESS,
					Data = commentDto
				};
			}
            catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
				return new ResponseDto<CommentDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.CREATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<CommentDto>> EditAsync(CommentEditDto dto, Guid id)
		{
			try
			{
				var commentEntity = await _context.Comments
					.Include(c => c.User)
					.Include(c => c.Event)
					.FirstOrDefaultAsync(c => c.Id == id);

				if (commentEntity is null)
				{
					return new ResponseDto<CommentDto>
					{
						StatusCode = 404,
						Status = false,
						Message = MessagesConstant.RECORD_NOT_FOUND
					};
				}

				_mapper.Map(dto, commentEntity);

				_context.Comments.Update(commentEntity);
				await _context.SaveChangesAsync();

				var commentDto = _mapper.Map<CommentDto>(commentEntity);

				return new ResponseDto<CommentDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.UPDATE_SUCCESS,
					Data = commentDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.UPDATE_ERROR);
				return new ResponseDto<CommentDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.UPDATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<CommentDto>> DeleteAsync(Guid id)
        {
			try
			{
				var commentEntity = await _context.Comments
					.Include(c => c.User)
					.Include(c => c.Event)
					.Include(c => c.Replies)
					.FirstOrDefaultAsync(c => c.Id == id);

				if (commentEntity is null)
				{
					return new ResponseDto<CommentDto>
					{
						StatusCode = 404,
						Status = false,
						Message = MessagesConstant.RECORD_NOT_FOUND
					};
				}

				// Eliminar todas las respuestas del comentario
				if (commentEntity.Replies != null && commentEntity.Replies.Any()) 
				{ 
					_context.Comments.RemoveRange(commentEntity.Replies); 
				}

				_context.Comments.Remove(commentEntity);
				await _context.SaveChangesAsync();

				var commentDto = _mapper.Map<CommentDto>(commentEntity);

				return new ResponseDto<CommentDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.DELETE_SUCCESS,
					Data = commentDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.DELETE_ERROR);
				return new ResponseDto<CommentDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.DELETE_ERROR
				};
			}
		}
    }
}
