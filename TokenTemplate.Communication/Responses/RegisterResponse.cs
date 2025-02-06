namespace TokenTemplate.Communication.Responses;

public class RegisterResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
}