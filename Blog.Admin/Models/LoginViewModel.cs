using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog.Admin.Models;

public class LoginViewModel
{
    [EmailAddress] public string Email { get; set; } = string.Empty;

    [Required] [MinLength(6)] public string Password { get; set; } = string.Empty;
}