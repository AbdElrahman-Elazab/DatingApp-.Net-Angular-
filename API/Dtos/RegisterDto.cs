using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string DisplayName { get; set; }
    [Required]
    [MinLength(4)]
    public string Password { get; set; }

    [Required] public string Gender { get; set; } = string.Empty;
    [Required] public string City { get; set; } = string.Empty;
    [Required] public string Country { get; set; } = string.Empty;
    [Required] public DateTime DateOfBirth { get; set; }

}
