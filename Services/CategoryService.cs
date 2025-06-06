using BusinessObjects.Entities;
using Repositories;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = _repository.GetAll(); // IQueryable
            var result = await Task.Run(() => categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                IsActive = c.IsActive,
                ModifiedAt = c.ModifiedAt,
                //ParentCategory = c.ParentCategory
            }).ToList());

            return result;
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                IsActive = category.IsActive,
                ModifiedAt = category.ModifiedAt,
                // ParentCategory = category.ParentCategory
            };
        }

        public async Task<CategoryResponseDto> AddCategoryAsync(CategoryRequestDto category)
        {
            var request = new Category
            {
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive,
                ParentCategoryId = category.ParentCategoryId,
                CreatedAt = category.CreatedAt
            };

            var created = await _repository.AddAsync(request);

            return new CategoryResponseDto
            {
                CategoryId = created.CategoryId,
                CategoryName = created.CategoryName,
                Description = created.Description,
                CreatedAt = created.CreatedAt,
                IsActive = created.IsActive,
                ModifiedAt = created.ModifiedAt,
                // ParentCategory = created.ParentCategory
            };
        }

        public async Task UpdateCategoryAsync(int id, CategoryRequestDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Id is not found");

            existing.CategoryName = dto.CategoryName;
            existing.Description = dto.Description;
            existing.IsActive = dto.IsActive;
            existing.ModifiedAt = dto.ModifiedAt;
            existing.ParentCategoryId = dto.ParentCategoryId;

            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _repository.DeleteByIdAsync(id);
        }

        public IQueryable<Category> GetAllEntities()
        {
            return _repository.GetAll(); // IQueryable
        }
    }
}
