using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Memberships;
using MeetPoint.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
	public class MembershipsService : IMembershipsService
	{
		private readonly MeetPointContext _context;
		private readonly IMapper _mapper;
		private readonly IAuditService _auditService;

		public MembershipsService(MeetPointContext context, IMapper mapper, IAuditService auditService)
        {
			this._context = context;
			this._mapper = mapper;
			this._auditService = auditService;
		}

		public async Task<ResponseDto<MembershipDto>> AddMembershipAsync(MembershipCreateDto dto)
		{
			dto.UserId = _auditService.GetUserId();

			var user = await _context.Users.Include(u => u.Membership).FirstOrDefaultAsync(u => u.Id == dto.UserId);
			if (user is null)
			{
				return new ResponseDto<MembershipDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

			if (user.Membership != null && user.Membership.EndDate > DateTime.UtcNow) 
			{ 
				return new ResponseDto<MembershipDto> 
				{ 
					StatusCode = 400, 
					Status = false, 
					Message = "El usuario ya tiene una membresía activa" 
				}; 
			}

			dto.StartDate = DateTime.UtcNow;
			dto.EndDate = dto.Type == "Mensual" ? dto.StartDate.AddMonths(1) : dto.StartDate.AddYears(1);

            var membershipEntity = new MembershipEntity
			{
				Id = Guid.NewGuid(),
				Type = dto.Type,
				Price = dto.Type == "Mensual" ? 2.99m : 29.99m, // Si no es mensual, será de 29.99
				StartDate = dto.StartDate, 
				EndDate = dto.EndDate, 
				UserId = dto.UserId,
			};

			_context.Memberships.Add(membershipEntity);
			user.Membership = membershipEntity;
			await _context.SaveChangesAsync();

			var membershipDto = new MembershipDto 
			{ 
				Id = membershipEntity.Id,
				UserId = membershipEntity.UserId,
				UserName = $"{user.FirstName} {user.LastName}",
				Type = membershipEntity.Type, 
				Price = membershipEntity.Price, 
				StartDate = membershipEntity.StartDate, 
				EndDate = membershipEntity.EndDate 
			}; 
			
			return new ResponseDto<MembershipDto> 
			{ 
				StatusCode = 200,
				Status = true, 
				Message = MessagesConstant.CREATE_SUCCESS,
				Data = membershipDto 
			};
		}
    }
}
