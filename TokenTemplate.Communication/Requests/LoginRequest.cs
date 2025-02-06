using System.ComponentModel.DataAnnotations;

namespace TokenTemplate.Communication.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    [MinLength(4, ErrorMessage = "A usuario deve ter pelo menos 4 caracteres.")]
    [MaxLength(255, ErrorMessage = "O nome de usuário pode ter no máximo 50 caracteres.")]
    public string Username { get; set; } = String.Empty;
    
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    [MaxLength(255, ErrorMessage = "A senha pode ter no máximo 255 caracteres.")]
    public string Password { get; set; } = String.Empty;
}