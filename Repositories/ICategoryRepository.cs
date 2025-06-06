using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetAll();
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> AddAsync(Category category);
        Task DeleteByIdAsync(int id);
        Task UpdateAsync(Category category);
    }

}
