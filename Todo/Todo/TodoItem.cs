using System.ComponentModel.DataAnnotations;

namespace Todo;
public class TodoItem
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}
