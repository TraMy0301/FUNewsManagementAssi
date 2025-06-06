using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class TagRequestDto
    {
        [Required, MaxLength(100)]
        public string TagName { get; set; } = null!;

        [MaxLength(255)]
        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
