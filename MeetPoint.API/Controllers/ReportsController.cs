using MeetPoint.API.Constants;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Reports;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetPoint.API.Controllers
{
	[ApiController]
	[Route("api/reports")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class ReportsController : ControllerBase
	{
		private readonly IReportsService _reportsService;

		public ReportsController(IReportsService reportsService)
        {
			this._reportsService = reportsService;
		}

		[HttpGet]
		[Authorize(Roles = $"{RolesConstant.ADMIN}")]
		public async Task<ActionResult<ResponseDto<List<ReportDto>>>> GetAll(string searchTerm = "", int page = 1)
		{
			var response = await _reportsService.GetPaginationReportsAsync(searchTerm, page);
			return StatusCode(response.StatusCode, response);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<ReportDto>>> Get(Guid id)
		{
			var response = await _reportsService.GetReportByIdAsync(id);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<ReportDto>>> Create(ReportCreateDto dto)
		{
			var response = await _reportsService.CreateAsync(dto);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<ReportDto>>> Edit(ReportEditDto dto, Guid id)
		{
			var response = await _reportsService.EditAsync(dto, id);
			return StatusCode(response.StatusCode, response);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<ReportDto>>> Delete(Guid id)
		{
			var response = await _reportsService.DeleteAsync(id);
			return StatusCode(response.StatusCode, response);
		}
	}
}
