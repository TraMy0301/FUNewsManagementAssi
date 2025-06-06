using BusinessObjects.Data;
using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly FuNewsManagementDbContext _context;

        public ArticleRepository(FuNewsManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Article?> AddArticleAsync(Article article)
        {
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();
            return await _context.Articles.FirstOrDefaultAsync(a => a.ArticleId.Equals(article.ArticleId));
        }

        public async Task DeleteByIdAsync(string id)
        {
            var article = await GetByIdAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Article> GetAll()
        {
            return _context.Articles.Include(a => a.Category)
                            .Include(a => a.Tags);
        }

        public async Task<Article?> GetByIdAsync(string id)
        {
            return await _context.Articles.FirstOrDefaultAsync(a => a.ArticleId.Equals(id));
        }

        public async Task UpdateArticleAsync(Article article)
        {
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GenerateNextArticleIdAsync()
        {
            var lastArticle = await _context.Articles
                .OrderByDescending(a => a.ArticleId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastArticle != null)
            {
                var numberPart = lastArticle.ArticleId.Substring(3);
                if (int.TryParse(numberPart, out int num))
                {
                    nextNumber = num + 1;
                }
            }

            string newId = $"ART{nextNumber:D3}";

            // Check trùng lặp trước khi trả về
            while (await _context.Articles.AnyAsync(a => a.ArticleId == newId))
            {
                nextNumber++;
                newId = $"ART{nextNumber:D3}";
            }

            return newId;
        }

    }
}
