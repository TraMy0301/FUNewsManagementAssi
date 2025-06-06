using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITagService
    {
        Task<List<TagResponseDto>> GetAllTags();
        Task<TagResponseDto> GetTagById(int id);

        Task<TagResponseDto> AddTag(TagRequestDto tag);
        Task UpdateTag(int id, TagRequestDto tag);

        Task DeleteTag(int id);
    }
}
