using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Challenges.GetChallengeLeaderboard;

public sealed record GetChallengeLeaderboardQuery(Guid ChallengeId, string? Scope) : IRequest<Result<ChallengeLeaderboardResponse>>;

public sealed record ChallengeLeaderboardResponse(
    Guid ChallengeId,
    string Title,
    string Scope,
    int TotalParticipants,
    IReadOnlyList<LeaderboardEntry> Entries,
    LeaderboardEntry? MyEntry
);

public sealed record LeaderboardEntry(
    int Rank,
    Guid UserId,
    string FullName,
    string? AvatarUrl,
    int CorrectAnswers,
    int TotalQuestions,
    int ScorePercentage,
    DateTime? CompletedAt,
    bool IsMe,
    bool IsFriend
);

public sealed class GetChallengeLeaderboardQueryHandler : IRequestHandler<GetChallengeLeaderboardQuery, Result<ChallengeLeaderboardResponse>>
{
    private const int MaxEntries = 50;
    private const string ScopeAll = "all";
    private const string ScopeFriends = "friends";

    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetChallengeLeaderboardQueryHandler(
        IChallengeRepository challengeRepository,
        IChallengeParticipationRepository participationRepository,
        IFriendshipRepository friendshipRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _challengeRepository = challengeRepository;
        _participationRepository = participationRepository;
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<ChallengeLeaderboardResponse>> Handle(GetChallengeLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<ChallengeLeaderboardResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var scope = string.IsNullOrWhiteSpace(request.Scope) ? ScopeAll : request.Scope.Trim().ToLowerInvariant();

        if (scope is not (ScopeAll or ScopeFriends))
        {
            return Result<ChallengeLeaderboardResponse>.Failure("invalid_scope", "Scope must be 'all' or 'friends'");
        }

        var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);

        // Drafts are admin-only; published and archived challenges keep their leaderboard visible.
        if (challenge is null || challenge.Status == "draft")
        {
            return Result<ChallengeLeaderboardResponse>.Failure("challenge_not_found", "Challenge not found");
        }

        var friendIds = (await _friendshipRepository.GetAcceptedForUserAsync(currentUserId.Value, cancellationToken))
            .Select(f => f.RequesterId == currentUserId.Value ? f.AddresseeId : f.RequesterId)
            .ToHashSet();

        // Same ordering as the admin participants screen so both rankings agree.
        var ranked = (await _participationRepository.GetByChallengeIdAsync(challenge.Id, cancellationToken))
            .Where(p => p.IsCompleted())
            .Where(p => scope == ScopeAll || p.UserId == currentUserId.Value || friendIds.Contains(p.UserId))
            .OrderByDescending(p => p.CorrectAnswers)
            .ThenBy(p => p.CompletedAt ?? DateTime.MaxValue)
            .Select((p, index) => (Participation: p, Rank: index + 1))
            .ToList();

        var myRanked = ranked.FirstOrDefault(r => r.Participation.UserId == currentUserId.Value);
        var visible = ranked.Take(MaxEntries).ToList();

        var userIds = visible.Select(r => r.Participation.UserId).Append(currentUserId.Value).Distinct();
        var users = (await _userRepository.GetByIdsAsync(userIds, cancellationToken)).ToDictionary(u => u.Id);

        LeaderboardEntry ToEntry((Domain.Entities.ChallengeParticipation Participation, int Rank) r)
        {
            var user = users.GetValueOrDefault(r.Participation.UserId);

            return new LeaderboardEntry(
                r.Rank,
                r.Participation.UserId,
                user?.FullName ?? "(deleted user)",
                user?.AvatarUrl,
                r.Participation.CorrectAnswers,
                r.Participation.TotalQuestions,
                r.Participation.ScorePercentage(),
                r.Participation.CompletedAt,
                r.Participation.UserId == currentUserId.Value,
                friendIds.Contains(r.Participation.UserId));
        }

        var response = new ChallengeLeaderboardResponse(
            challenge.Id,
            challenge.Title,
            scope,
            ranked.Count,
            visible.Select(ToEntry).ToList(),
            myRanked.Participation is null ? null : ToEntry(myRanked));

        return Result<ChallengeLeaderboardResponse>.Success(response);
    }
}
