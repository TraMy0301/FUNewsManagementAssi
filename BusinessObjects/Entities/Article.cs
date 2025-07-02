using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Entities;

public partial class Article
{
    public string ArticleId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Headline { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Source { get; set; }

    public int? CategoryId { get; set; }

    public string Status { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
    [MaxLength(500)]
    public string? ImageURL { get; set; } 

    public virtual Category? Category { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
