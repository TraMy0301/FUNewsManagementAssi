using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class Category
    {
        [Key] 
        public int CategoryId { get; set; }

        [Required, MaxLength(200)]
        public string CategoryName { get; set; } = null!;

        [MaxLength(255)]
        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ModifiedAt { get; set; }

        [JsonIgnore]
        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

        public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

        [JsonIgnore]
        public virtual Category? ParentCategory { get; set; }
    }
}
