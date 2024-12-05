using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Reports;

namespace MeetPoint.API.Services.Interfaces
{
	public interface IReportsService
	{
		Task<ResponseDto<PaginationDto<List<ReportDto>>>> GetPaginationReportsAsync(string searchTerm = "", int page = 1);
		Task<ResponseDto<ReportDto>> GetReportByIdAsync(Guid id);
		Task<ResponseDto<ReportDto>> CreateAsync(ReportCreateDto dto);
		Task<ResponseDto<ReportDto>> EditAsync(ReportEditDto dto, Guid id);
		Task<ResponseDto<ReportDto>> DeleteAsync(Guid id);
	}
}
