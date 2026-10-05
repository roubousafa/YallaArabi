using System.Reflection;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using YallaArabi.Application.Behaviors;
using YallaArabi.Application.Interfaces;
using YallaArabi.Application.Services;

namespace YallaArabi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<IPointsService, PointsService>();
        services.AddScoped<IFriendCodeService, FriendCodeService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<ILessonContentService, LessonContentService>();
        services.AddScoped<IReviewService, ReviewService>();
        return services;
    }
}