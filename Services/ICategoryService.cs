using BusinessObjects.Entities;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        IQueryable<Category> GetAllEntities();
        Task<CategoryResponseDto> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto> AddCategoryAsync(CategoryRequestDto category);
        Task UpdateCategoryAsync(int id, CategoryRequestDto dto);
        Task DeleteCategoryAsync(int id);
    }
}
