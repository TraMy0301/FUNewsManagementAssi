using BusinessObjects.Entities;
using Repositories;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repository;
        public TagService(ITagRepository repository)
        {
            _repository = repository;
        }

        public Task<TagResponseDto> AddTag(TagRequestDto tag)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTag(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TagResponseDto>> GetAllTags()
        {
            var tags = _repository.GetAll(); // vẫn là IQueryable
            return await Task.Run(() => tags.Select(t => new TagResponseDto
            {
                TagId = t.TagId,
                TagName = t.TagName,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                ModifiedAt = t.ModifiedAt
            }).ToList());
        }

        public async Task<TagResponseDto> GetTagById(int id)
        {
            var tag = await _repository.GetByIdAsync(id);
            if (tag == null)
                throw new Exception("Tag not found");

            return new TagResponseDto
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                Description = tag.Description,
                CreatedAt = tag.CreatedAt,
                ModifiedAt = tag.ModifiedAt
            };
        }

        public Task UpdateTag(int id, TagRequestDto tag)
        {
            throw new NotImplementedException();
        }
    }
}
