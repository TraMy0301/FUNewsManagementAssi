using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly FuNewsDbContext _context;

        public TagRepository(FuNewsDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> AddTagAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return await _context.Tags.FirstOrDefaultAsync(t => t.TagId == tag.TagId);
        }

        public async Task DeleteTagAsync(int id)
        {
            var tag = await GetByIdAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Tag> GetAll()
        {
            return _context.Tags.AsQueryable(); // Không cần async vì chưa thực thi
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.TagId == id);
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }
    }
}
