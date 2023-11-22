namespace Identity.App.DtoModels;

public class AuthorizationDto
{
    public string Token { get; set; }
    public AuthorizedUserDto User { get; set; }
}