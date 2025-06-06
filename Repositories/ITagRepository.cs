using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> AddTagAsync(Tag tag);
        Task DeleteTagAsync(int id);
        IQueryable<Tag> GetAll();
        Task<Tag?> GetByIdAsync(int id);
        Task UpdateTagAsync(Tag tag);
    }

}
