using BusinessObjects;
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
        private readonly FuNewsDbContext _context;
        public ArticleService(IArticleRepository repository, FuNewsDbContext fuNewsDbContext) {
            _repository = repository;
            _context = fuNewsDbContext;
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
                ImageURL = dto.ImageURL,
                Status = "Pending",
                Tags = new List<Tag>()
            };

            // Gán Tags nếu có
            if (dto.TagIds != null && dto.TagIds.Any())
            {
                var tags = await _context.Tags
                .Where(t => dto.TagIds.Contains(t.TagId))
                .ToListAsync();
                article.Tags = tags;
            }

            var created = await _repository.AddArticleAsync(article);

            return new ArticleResponseDto
            {
                ArticleId = created.ArticleId,
                Title = created.Title,
                Content = created.Content,
                Headline = created.Headline,
                Source = created.Source,
                Status = created.Status,
                CreatedAt = created.CreatedAt,
                ModifiedAt = created.ModifiedAt,
                ImageURL = created.ImageURL,
                Tags = created.Tags.Select(t => new TagResponseDto
                {
                    TagId = t.TagId,
                    TagName = t.TagName
                }).ToList()
            };
        }


        public async Task DeleteArticle(string id)
        {
            await _repository.DeleteByIdAsync(id);
        }

        public IQueryable<ArticleResponseDto> GetAllArticles()
        {
            var articles = _repository.GetAll(); // đã Include Tags

            return articles.Select(a => new ArticleResponseDto
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Headline = a.Headline,
                Content = a.Content,
                Source = a.Source,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                ModifiedAt = a.ModifiedAt,
                ImageURL = a.ImageURL,
                Tags = a.Tags.Select(t => new TagResponseDto
                {
                    TagId = t.TagId,
                    TagName = t.TagName
                }).ToList()
            });
        }



        public IQueryable<Article> GetArticles()
        {
            return _repository.GetAll(); // vẫn là IQueryable
        }

        public async Task<ArticleResponseDto> GetArticleById(string id)
        {
            var article = await _context.Articles
            .Include(a => a.Tags) 
            .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null) throw new KeyNotFoundException("Không tìm thấy bài viết");

            return new ArticleResponseDto
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Headline = article.Headline,
                Content = article.Content,
                Source = article.Source,
                Status = article.Status,
                CategoryId = article.CategoryId,
                CreatedAt = article.CreatedAt,
                ModifiedAt = article.ModifiedAt,
                ImageURL = article.ImageURL,

                Tags = article.Tags.Select(t => new TagResponseDto
                {
                    TagId = t.TagId,
                    TagName = t.TagName
                }).ToList()
            };
        }


        public async Task UpdateArticle(string id, ArticleRequestDto dto)
        {
            var existingArticle = await _context.Articles
                .Include(a => a.Tags) 
                .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (existingArticle == null)
                throw new KeyNotFoundException("Không tìm thấy bài báo.");

            existingArticle.Title = dto.Title ?? existingArticle.Title;
            existingArticle.Headline = dto.Headline ?? existingArticle.Headline;
            existingArticle.Content = dto.Content ?? existingArticle.Content;
            existingArticle.Source = dto.Source ?? existingArticle.Source;
            existingArticle.ImageURL = dto.ImageURL ?? existingArticle.ImageURL;
            existingArticle.Status = dto.Status ?? existingArticle.Status;
            existingArticle.CategoryId = dto.CategoryId != 0 ? dto.CategoryId : existingArticle.CategoryId;
            existingArticle.ModifiedAt = DateTime.Now;

            if (dto.TagIds != null)
            {
                var selectedTags = await _context.Tags
                    .Where(t => dto.TagIds.Contains(t.TagId))
                    .ToListAsync();

                existingArticle.Tags.Clear();
                foreach (var tag in selectedTags)
                {
                    existingArticle.Tags.Add(tag);
                }
            }

            // Nếu có ảnh mới khác ảnh cũ
            if (dto.ImageURL != null && dto.ImageURL != existingArticle.ImageURL)
            {
                var oldImagePath = Path.Combine("wwwroot", existingArticle.ImageURL.TrimStart('/'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                existingArticle.ImageURL = dto.ImageURL;
            }


            await _context.SaveChangesAsync();
        }
    }
}
