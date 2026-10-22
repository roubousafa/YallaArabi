namespace YallaArabi.Application.Interfaces;

// Links that go out in emails and point back to the app or the server's own pages.
public interface IAppLinks
{
    string PasswordReset(string token, string email);
}
