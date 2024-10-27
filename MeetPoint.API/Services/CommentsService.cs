﻿using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Comments;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly MeetPointContext _context;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;
        private readonly int PAGE_SIZE;

		public CommentsService(
            MeetPointContext context,
            IMapper mapper,
            ILogger<CommentsService> logger,
            IConfiguration configuration)
        {
            this._context = context;
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
				.OrderBy(u => u.PublicationDate)
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
				// Validar que el usuario del comentario existe
				var existingUser = await _context.Users.FindAsync(dto.UserId.ToString());
				if (existingUser is null)
				{
					return new ResponseDto<CommentDto>
					{
						StatusCode = 404,
						Status = false,
						Message = $"UserId: {MessagesConstant.RECORD_NOT_FOUND}"
					};
				}

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

				var commentEntity = _mapper.Map<CommentEntity>(dto);
				commentEntity.PublicationDate = DateTime.Now;

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

				// Validar si el UserId o EventId estan siendo modificados
				if (dto.UserId.ToString() != commentEntity.UserId || dto.EventId != commentEntity.EventId)
				{
					// Validar que UserId existe si se va a editar
					if (dto.UserId.ToString() != commentEntity.UserId)
					{
						var existingUser = await _context.Users.FindAsync(dto.UserId);
						if (existingUser is null)
						{
							return new ResponseDto<CommentDto>
							{
								StatusCode = 404,
								Status = false,
								Message = $"UserId: {MessagesConstant.RECORD_NOT_FOUND}"
							};
						}
						commentEntity.UserId = dto.UserId.ToString();
					}

					// Validar que EventId existe si se va a editar
					if (dto.EventId != commentEntity.EventId)
					{
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
						commentEntity.EventId = dto.EventId;
					}
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
