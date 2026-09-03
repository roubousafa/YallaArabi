using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class AuditLog : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string UserFullName { get; private set; } = default!;
    public string UserEmail { get; private set; } = default!;
    public string ActionEn { get; private set; } = default!;
    public string ActionAr { get; private set; } = default!;
    public string? EntityType { get; private set; }
    public string? EntityId { get; private set; }
    public string HttpMethod { get; private set; } = default!;
    public string Path { get; private set; } = default!;
    public int StatusCode { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        Guid userId,
        string userFullName,
        string userEmail,
        string actionEn,
        string actionAr,
        string httpMethod,
        string path,
        int statusCode,
        string? entityType = null,
        string? entityId = null,
        string? ipAddress = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserFullName = userFullName,
            UserEmail = userEmail,
            ActionEn = actionEn,
            ActionAr = actionAr,
            EntityType = entityType,
            EntityId = entityId,
            HttpMethod = httpMethod,
            Path = path,
            StatusCode = statusCode,
            IpAddress = ipAddress,
            OccurredAt = DateTime.UtcNow
        };
    }
}