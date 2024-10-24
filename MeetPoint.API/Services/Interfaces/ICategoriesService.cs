using MeetPoint.API.Dtos.Categories;
using MeetPoint.API.Dtos.Common;

namespace MeetPoint.API.Services.Interfaces
{
	public interface ICategoriesService
	{
		Task<ResponseDto<PaginationDto<List<CategoryDto>>>> GetAllCategoriesAsync(string searchTerm = "", int page = 1);
		Task<ResponseDto<CategoryDto>> GetCategoryByIdAsync(Guid id);
		Task<ResponseDto<CategoryDto>> CreateAsync(CategoryCreateDto dto);
		Task<ResponseDto<CategoryDto>> EditAsync(CategoryEditDto dto, Guid id);
		Task<ResponseDto<CategoryDto>> DeleteAsync(Guid id);
	}
}
