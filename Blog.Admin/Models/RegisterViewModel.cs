using System.ComponentModel.DataAnnotations;

namespace Blog.Admin.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Please enter email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter email")]
    [MinLength(6, ErrorMessage = "Minimum length is 6")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter password")]
    [Compare("Password", ErrorMessage = "The two passwords must be the same")]
    public string PasswordConfirmation { get; set; } = string.Empty;
}