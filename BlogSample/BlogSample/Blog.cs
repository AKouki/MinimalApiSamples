using System.ComponentModel.DataAnnotations;

namespace BlogSample;
public class Blog
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public ICollection<Post> MyProperty { get; set; }
}
