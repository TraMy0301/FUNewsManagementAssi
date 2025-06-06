using BusinessObjects.Data;
using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FuNewsManagementDbContext _context;

        public CategoryRepository(FuNewsManagementDbContext context)
        {
            _context = context;
        }

        public IQueryable<Category> GetAll()
        {
            return _context.Categories.AsQueryable(); // Không async vì IQueryable chưa thực thi
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<Category?> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
