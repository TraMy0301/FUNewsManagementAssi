using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IArticleRepository
    {
        Task<Article?> AddArticleAsync(Article article);
        Task DeleteByIdAsync(string id);
        IQueryable<Article> GetAll();
        Task<Article?> GetByIdAsync(string id);
        Task UpdateArticleAsync(Article article);
        Task<string> GenerateNextArticleIdAsync();
    }

}
