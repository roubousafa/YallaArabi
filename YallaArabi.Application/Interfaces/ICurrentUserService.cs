namespace YallaArabi.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}