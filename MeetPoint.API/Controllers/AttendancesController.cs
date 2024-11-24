using MeetPoint.API.Constants;
using MeetPoint.API.Dtos.Attendances;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetPoint.API.Controllers
{
    [ApiController]
    [Route("api/attendances")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class AttendancesController : ControllerBase
    {
        private readonly IAttendancesService _attendancesService;

        public AttendancesController(IAttendancesService attendancesService)
        {
            this._attendancesService = attendancesService;
        }

        [HttpGet]
        [AllowAnonymous]
		public async Task<ActionResult<ResponseDto<List<AttendanceDto>>>> GetAll(string searchTerm = "", int page = 1)
        {
            var response = await _attendancesService.GetAllAttendancesAsync(searchTerm, page);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<AttendanceDto>>> Get(Guid id)
        {
            var response = await _attendancesService.GetAttendanceByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<AttendanceDto>>> Create(AttendanceCreateDto dto)
        {
            var response = await _attendancesService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<AttendanceDto>>> Edit(AttendanceEditDto dto, Guid id)
        {
            var response = await _attendancesService.EditAsync(dto, id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}, {RolesConstant.ADMIN}, {RolesConstant.ORGANIZER}")]
		public async Task<ActionResult<ResponseDto<AttendanceDto>>> Delete(Guid id)
        {
            var response = await _attendancesService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
