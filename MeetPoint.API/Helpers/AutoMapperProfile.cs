using AutoMapper;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Attendances;
using MeetPoint.API.Dtos.Categories;
using MeetPoint.API.Dtos.Comments;
using MeetPoint.API.Dtos.Events;
using MeetPoint.API.Dtos.Users;

namespace MeetPoint.API.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			MapsForCategories();
			MapsForEvents();
			MapsForUsers();
			MapsForAttendances();
			MapsForComments();
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
				.ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

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
				.ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title));

			CreateMap<CommentCreateDto, CommentEntity>();
			CreateMap<CommentEditDto, CommentEntity>();
		}
	}
}
