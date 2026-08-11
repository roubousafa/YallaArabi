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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}