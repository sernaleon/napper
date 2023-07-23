using Napper.Core;

namespace Napper.Application;

public static class ScheduleDependencyInjectionExtensions
{
  public static IServiceCollection AddSchedule(this IServiceCollection services,ScheduleConfiguration configuration )
  {
    services.AddSingleton(configuration);
    services.AddTransient<IScheduleService, ScheduleService>();
    services.AddTransient<ScheduleGenerator>();
    services.AddTransient<ScheduleDayMetadataCalulator>();
    return services;
  }
}
