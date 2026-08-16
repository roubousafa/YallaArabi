using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class Friendship : AggregateRoot
{
    public Guid RequesterId { get; private set; }
    public Guid AddresseeId { get; private set; }
    public string Status { get; private set; } = default!;
    public DateTime RequestedAt { get; private set; }
    public DateTime? RespondedAt { get; private set; }

    private Friendship() { }

    public static Friendship SendRequest(Guid requesterId, Guid addresseeId)
    {
        if (requesterId == addresseeId)
            throw new ArgumentException("A user cannot send a friend request to themselves", nameof(addresseeId));

        return new Friendship
        {
            Id = Guid.NewGuid(),
            RequesterId = requesterId,
            AddresseeId = addresseeId,
            Status = "pending",
            RequestedAt = DateTime.UtcNow,
            RespondedAt = null
        };
    }

    public void Accept()
    {
        Status = "accepted";
        RespondedAt = DateTime.UtcNow;
    }

    public void Decline()
    {
        Status = "declined";
        RespondedAt = DateTime.UtcNow;
    }

    public bool IsAccepted() => Status == "accepted";

    public bool IsPending() => Status == "pending";

    public Guid GetOtherUserId(Guid currentUserId) =>
        currentUserId == RequesterId ? AddresseeId : RequesterId;
}