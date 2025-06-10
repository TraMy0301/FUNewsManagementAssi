using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _repository;
        public ArticleService(IArticleRepository repository) {
            _repository = repository;
        }

        public async Task<ArticleResponseDto> AddArticle(ArticleRequestDto dto)
        {
            var article = new Article
            {
                ArticleId = await _repository.GenerateNextArticleIdAsync(),
                Title = dto.Title,
                Content = dto.Content,
                Headline = dto.Headline,
                Source = dto.Source,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.Now,
                ModifiedAt = dto.ModifiedAt,
            };

            var created = await _repository.AddArticleAsync(article);
            return new ArticleResponseDto
            {
                ArticleId = created.ArticleId,
                Title = created.Title,
                Content = created.Content,
                Headline = created.Headline,
                Source = created.Source,
                Status = created.Status,
                //CategoryName = created.Category.CategoryName, 
                CreatedAt = created.CreatedAt,
                ModifiedAt = created.ModifiedAt,
            };
        }

        public async Task DeleteArticle(string id)
        {
            await _repository.DeleteByIdAsync(id);
        }

        public IQueryable<ArticleResponseDto> GetAllArticles()
        {
            var articles = _repository.GetAll(); // vẫn là IQueryable
            return articles.Select(a => new ArticleResponseDto
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Headline = a.Headline,
                Content = a.Content,
                Source = a.Source,
              //  CategoryName = a.Category.CategoryName,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                ModifiedAt = a.ModifiedAt
            });
        }

        public IQueryable<Article> GetArticles()
        {
            return _repository.GetAll(); // vẫn là IQueryable
        }

        public async Task<ArticleResponseDto> GetArticleById(string id)
        {
            var article = await _repository.GetByIdAsync(id);
            if (article == null)
                throw new Exception("Article not found");
            return new ArticleResponseDto
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Headline = article.Headline,
                Content = article.Content,
                Source = article.Source,
              //  CategoryName = article.Category.CategoryName,
                Status = article.Status,
                CreatedAt = article.CreatedAt,
                ModifiedAt = article.ModifiedAt
            };
        }


        public async Task UpdateArticle(string id, ArticleRequestDto article)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Id is not found");

            existing.Title = article.Title;
            existing.Headline = article.Headline;
            existing.Content = article.Content;
            existing.Source = article.Source;
            existing.CategoryId = article.CategoryId;
            existing.Status = article.Status;
            existing.ModifiedAt = DateTime.Now;

            await _repository.UpdateArticleAsync(existing);
        }

        

    }
}
