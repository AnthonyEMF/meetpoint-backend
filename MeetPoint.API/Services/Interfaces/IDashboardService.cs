using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Dashboard;

namespace MeetPoint.API.Services.Interfaces
{
	public interface IDashboardService
	{
		Task<ResponseDto<DashboardDto>> GetDashboardAsync();
	}
}
