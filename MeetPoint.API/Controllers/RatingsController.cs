using MeetPoint.API.Constants;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Ratings;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetPoint.API.Controllers
{
	[ApiController]
	[Route("api/ratings")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class RatingsController : ControllerBase
	{
		private readonly IRatingsService _ratingsService;

		public RatingsController(IRatingsService ratingsService)
        {
			this._ratingsService = ratingsService;
		}

		[HttpGet]
		[Authorize(Roles = $"{RolesConstant.ADMIN}")]
		public async Task<ActionResult<ResponseDto<List<RatingDto>>>> GetAll(string searchTerm = "", int page = 1)
		{
			var response = await _ratingsService.GetPaginationRatingsAsync(searchTerm, page);
			return StatusCode(response.StatusCode, response);
		}

		[HttpGet("{id}")]
		[AllowAnonymous]
		public async Task<ActionResult<ResponseDto<RatingDto>>> Get(Guid id)
		{
			var response = await _ratingsService.GetRatingByIdAsync(id);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<RatingDto>>> Create(RatingCreateDto dto)
		{
			var response = await _ratingsService.CreateAsync(dto);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<RatingDto>>> Edit(RatingEditDto dto, Guid id)
		{
			var response = await _ratingsService.EditAsync(dto, id);
			return StatusCode(response.StatusCode, response);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<RatingDto>>> Delete(Guid id)
		{
			var response = await _ratingsService.DeleteAsync(id);
			return StatusCode(response.StatusCode, response);
		}
	}
}
