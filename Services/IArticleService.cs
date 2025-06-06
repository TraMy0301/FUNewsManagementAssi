using BusinessObjects.Entities;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IArticleService
    {
        IQueryable<ArticleResponseDto> GetAllArticles();
        IQueryable<Article> GetArticles();
        Task<ArticleResponseDto> GetArticleById(string id);

        Task<ArticleResponseDto> AddArticle(ArticleRequestDto article);
        Task UpdateArticle(string id, ArticleRequestDto article);

        Task DeleteArticle(string id);
    }
}
