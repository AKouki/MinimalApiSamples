using System.ComponentModel.DataAnnotations;

namespace BlogSample;
public class Post
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Content { get; set; }
    public string ThumbnailUrl { get; set; }
    public string AuthorName { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [Required]
    public string Category { get; set; }
    [Required]
    public string Slug { get; set; }
    public string ReadTime { get; set; }

    public int BlogId { get; set; }
}
