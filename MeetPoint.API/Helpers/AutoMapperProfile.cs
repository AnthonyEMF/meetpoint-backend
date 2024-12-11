using AutoMapper;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Attendances;
using MeetPoint.API.Dtos.Categories;
using MeetPoint.API.Dtos.Comments;
using MeetPoint.API.Dtos.Dashboard;
using MeetPoint.API.Dtos.Events;
using MeetPoint.API.Dtos.Memberships;
using MeetPoint.API.Dtos.Ratings;
using MeetPoint.API.Dtos.Reports;
using MeetPoint.API.Dtos.Users;

namespace MeetPoint.API.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			MapsForDashboard();
			MapsForCategories();
			MapsForEvents();
			MapsForUsers();
			MapsForAttendances();
			MapsForComments();
			MapsForReports();
			MapsForRatings();
			MapsForMemberships();
		}

		private void MapsForDashboard()
		{
			CreateMap<CategoryEntity, DashboardCategoryDto>();
			CreateMap<EventEntity, DashboardEventDto>();
			CreateMap<UserEntity, DashboardUserDto>()
				.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
			CreateMap<ReportEntity, DashboardReportDto>()
				.ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter.FirstName + " " + src.Reporter.LastName))
				.ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.Organizer.FirstName + " " + src.Organizer.LastName));
		}

		private void MapsForCategories()
		{
			CreateMap<CategoryEntity, CategoryDto>();
			CreateMap<CategoryCreateDto, CategoryEntity>();
			CreateMap<CategoryEditDto, CategoryEntity>();
		}

		private void MapsForEvents()
		{
			CreateMap<EventEntity, EventDto>()
				.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
				.ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.Organizer.FirstName + " " + src.Organizer.LastName))
				.ForMember(dest => dest.Attendances, opt => opt.MapFrom(src => src.Attendances))
				.ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));
			CreateMap<EventCreateDto, EventEntity>();
			CreateMap<EventEditDto, EventEntity>();
		}

		private void MapsForUsers()
		{
			CreateMap<UserEntity, UserDto>()
				.ForMember(dest => dest.Attendances, opt => opt.MapFrom(src => src.Attendances))
				.ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
				.ForMember(dest => dest.Membership, opt => opt.MapFrom(src => src.Membership))
				.ForMember(dest => dest.Reports, opt => opt.MapFrom(src => src.Reports))
				.ForMember(dest => dest.MadeReports, opt => opt.MapFrom(src => src.MadeReports))
				.ForMember(dest => dest.Ratings, opt => opt.MapFrom(src => src.Ratings))
				.ForMember(dest => dest.MadeRatings, opt => opt.MapFrom(src => src.MadeRatings));
			CreateMap<UserCreateDto, UserEntity>();
			CreateMap<UserEditDto, UserEntity>();
		}

		private void MapsForAttendances()
		{
			CreateMap<AttendanceEntity, AttendanceDto>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
				.ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title));
			CreateMap<AttendanceCreateDto, AttendanceEntity>();
			CreateMap<AttendanceEditDto, AttendanceEntity>();
		}

		private void MapsForComments()
		{
			CreateMap<CommentEntity, CommentDto>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
				.ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
				.ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title));
			CreateMap<CommentCreateDto, CommentEntity>();
			CreateMap<CommentEditDto, CommentEntity>();
		}

		private void MapsForReports()
		{
			CreateMap<ReportEntity, ReportDto>()
				.ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter.FirstName + " " + src.Reporter.LastName))
				.ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.Organizer.FirstName + " " + src.Organizer.LastName));
			CreateMap<ReportCreateDto, ReportEntity>();
			CreateMap<ReportEditDto, ReportEntity>();
		}

		private void MapsForRatings()
		{
			CreateMap<RatingEntity, RatingDto>()
				.ForMember(dest => dest.RaterName, opt => opt.MapFrom(src => src.Rater.FirstName + " " + src.Rater.LastName))
				.ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title))
				.ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.Organizer.FirstName + " " + src.Organizer.LastName));
			CreateMap<RatingCreateDto, RatingEntity>();
			CreateMap<RatingEditDto, RatingEntity>();
		}

		private void MapsForMemberships()
		{
			CreateMap<MembershipEntity, MembershipDto>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));
			CreateMap<MembershipCreateDto, MembershipEntity>();
			//CreateMap<AttendanceEditDto, AttendanceEntity>();
		}
	}
}
