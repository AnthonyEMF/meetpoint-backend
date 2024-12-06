using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Categories;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
	public class CategoriesService : ICategoriesService
	{
		private readonly MeetPointContext _context;
		private readonly IMapper _mapper;
		private readonly int PAGE_SIZE;

		public CategoriesService(MeetPointContext context, IMapper mapper, IConfiguration configuration)
        {
			this._context = context;
			this._mapper = mapper;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

        public async Task<ResponseDto<PaginationDto<List<CategoryDto>>>> GetAllCategoriesAsync(string searchTerm = "", int page = 1)
		{
			int startIndex = (page - 1) * PAGE_SIZE;

			var categoriesEntityQuery = _context.Categories
				.Where(x => x.Description.ToLower().Contains(searchTerm.ToLower()));

			var totalCategories = await categoriesEntityQuery.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalCategories / PAGE_SIZE);

			var categoriesEntity = await categoriesEntityQuery
				.OrderByDescending(u => u.CreatedDate)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var categoriesDtos = _mapper.Map<List<CategoryDto>>(categoriesEntity);

			return new ResponseDto<PaginationDto<List<CategoryDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<CategoryDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalCategories,
					TotalPages = totalPages,
					Items = categoriesDtos,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages
				}
			};
		}

		public async Task<ResponseDto<CategoryDto>> GetCategoryByIdAsync(Guid id)
		{
			var categoryEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
			if (categoryEntity is null)
			{
				return new ResponseDto<CategoryDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

			var categoryDto = _mapper.Map<CategoryDto>(categoryEntity);

			return new ResponseDto<CategoryDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORD_FOUND,
				Data = categoryDto
			};
		}

		public async Task<ResponseDto<CategoryDto>> CreateAsync(CategoryCreateDto dto)
		{
			// Validar si ya existe una categoría con el mismo nombre
			var existingCategory = await _context.Categories
				.FirstOrDefaultAsync(c => c.Name == dto.Name);

			if (existingCategory is not null)
			{
				return new ResponseDto<CategoryDto>
				{
					StatusCode = 400,
					Status = false,
					Message = "Ya existe una categoría con el mismo nombre."
				};
			}

			var categoryEntity = _mapper.Map<CategoryEntity>(dto);

			_context.Categories.Add(categoryEntity);
			await _context.SaveChangesAsync();

			var categoryDto = _mapper.Map<CategoryDto>(categoryEntity);

			return new ResponseDto<CategoryDto>
			{
				StatusCode = 201,
				Status = true,
				Message = MessagesConstant.CREATE_SUCCESS,
				Data = categoryDto
			};
		}

		public async Task<ResponseDto<CategoryDto>> EditAsync(CategoryEditDto dto, Guid id)
		{
			var categoryEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

			if (categoryEntity is null)
			{
				return new ResponseDto<CategoryDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND,
				};
			}

			// Validar si ya existe otra categoría con el mismo nombre que no sea la actual
			var existingCategory = await _context.Categories
				.FirstOrDefaultAsync(c => c.Name == dto.Name && c.Id != id);

			if (existingCategory is not null)
			{
				return new ResponseDto<CategoryDto>
				{
					StatusCode = 400,
					Status = false,
					Message = "Ya existe una categoría con el mismo nombre."
				};
			}

			_mapper.Map<CategoryEditDto, CategoryEntity>(dto, categoryEntity);

			_context.Categories.Update(categoryEntity);
			await _context.SaveChangesAsync();

			var categoryDto = _mapper.Map<CategoryDto>(categoryEntity);

			return new ResponseDto<CategoryDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.UPDATE_SUCCESS,
				Data = categoryDto
			};
		}

		public async Task<ResponseDto<CategoryDto>> DeleteAsync(Guid id)
		{
			var categoryEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
			if (categoryEntity is null)
			{
				return new ResponseDto<CategoryDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

			// Validar que la categoría no tenga eventos asociados
			bool hasEvents = await _context.Events.AnyAsync(e => e.CategoryId == id);
			if (hasEvents) 
			{ 
				return new ResponseDto<CategoryDto> 
				{ 
					StatusCode = 400, 
					Status = false, 
					Message = "La categoría no puede ser eliminada porque tiene eventos asociados." 
				}; 
			}

			_context.Categories.Remove(categoryEntity);
			await _context.SaveChangesAsync();

			return new ResponseDto<CategoryDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.DELETE_SUCCESS
			};
		}
	}
}
