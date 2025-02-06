using System.ComponentModel.DataAnnotations;

namespace TokenTemplate.Api.Model;

public class User
{
    public User()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; } 
    
    public string Username { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}