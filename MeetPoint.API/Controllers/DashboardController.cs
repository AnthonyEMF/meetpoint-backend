using MeetPoint.API.Constants;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Dashboard;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeetPoint.API.Controllers
{
	[Route("api/dashboard")]
	[ApiController]
	public class DashboardController : ControllerBase
	{
		private readonly IDashboardService _dashboardService;

		public DashboardController(IDashboardService dashboardService)
        {
			this._dashboardService = dashboardService;
		}

		[HttpGet("info")]
		[Authorize(Roles = $"{RolesConstant.ADMIN}")]
		public async Task<ActionResult<ResponseDto<DashboardDto>>> GetDashboardInfo()
		{
			var response = await _dashboardService.GetDashboardAsync();
			return StatusCode(response.StatusCode, response);
		}
	}
}
