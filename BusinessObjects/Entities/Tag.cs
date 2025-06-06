using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required, MaxLength(100)]
        public string TagName { get; set; } = null!;

        [MaxLength(255)]
        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [JsonIgnore]
        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
