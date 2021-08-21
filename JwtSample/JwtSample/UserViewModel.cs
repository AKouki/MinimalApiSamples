using System.ComponentModel.DataAnnotations;

namespace JwtSample;
public class UserViewModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}
