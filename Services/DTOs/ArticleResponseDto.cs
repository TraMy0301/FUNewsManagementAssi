using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class ArticleResponseDto
    {
        [Key]
        public string ArticleId { get; set; }

        public string Title { get; set; }

        public string Headline { get; set; }

        public string Content { get; set; }

        public string? Source { get; set; }

        public int? CategoryId { get; set; }

        public string Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public CategoryResponseDto Category { get; set; }

        public List<TagResponseDto> Tags { get; set; } = new();
        public string ImageURL { get; set; }
        //public List<TagDto>? Tags { get; set; }

    }
}

