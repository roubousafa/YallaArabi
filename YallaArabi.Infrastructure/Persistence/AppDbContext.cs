using Microsoft.EntityFrameworkCore;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Level> Levels => Set<Level>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<StudentLessonProgress> StudentLessonProgress => Set<StudentLessonProgress>();
    public DbSet<SavedWord> SavedWords => Set<SavedWord>();
    public DbSet<Word> Words => Set<Word>();
    public DbSet<WordReview> WordReviews => Set<WordReview>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<PointRule> PointRules => Set<PointRule>();
    public DbSet<PointTransaction> PointTransactions => Set<PointTransaction>();
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<ChallengeQuestion> ChallengeQuestions => Set<ChallengeQuestion>();
    public DbSet<ChallengeParticipation> ChallengeParticipations => Set<ChallengeParticipation>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<ChallengeAssignment> ChallengeAssignments => Set<ChallengeAssignment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ChallengeAnswer> ChallengeAnswers => Set<ChallengeAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}