using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class ArticleRequestDto
    {

        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;

        [MaxLength(255)]
        public string Headline { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Source { get; set; }

        public int CategoryId { get; set; }

        public string Status { get; set; } = "Draft";

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
