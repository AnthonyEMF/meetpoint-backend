using MeetPoint.API.Constants;
using MeetPoint.API.Dtos.Comments;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetPoint.API.Controllers
{
    [ApiController]
    [Route("api/comments")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            this._commentsService = commentsService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<List<CommentDto>>>> GetAll(string searchTerm = "", int page = 1)
        {
            var response = await _commentsService.GetAllCommentsAsync(searchTerm, page);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<CommentDto>>> Get(Guid id)
        {
            var response = await _commentsService.GetCommentByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<CommentDto>>> Create(CommentCreateDto dto)
        {
            var response = await _commentsService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<CommentDto>>> Edit(CommentEditDto dto, Guid id)
        {
            var response = await _commentsService.EditAsync(dto, id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<CommentDto>>> Delete(Guid id)
        {
            var response = await _commentsService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
