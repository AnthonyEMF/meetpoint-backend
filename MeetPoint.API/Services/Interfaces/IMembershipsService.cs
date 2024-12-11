using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Memberships;

namespace MeetPoint.API.Services.Interfaces
{
	public interface IMembershipsService
	{
		Task<ResponseDto<MembershipDto>> AddMembershipAsync(MembershipCreateDto dto);
	}
}
