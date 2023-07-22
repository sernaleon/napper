namespace Napper.Application;

public class ScheduleService
{
  private readonly ScheduleGenerator _generator;
  private readonly ScheduleDayMetadataCalulator _metadataCalulator;

  public ScheduleService(ScheduleGenerator generator, ScheduleDayMetadataCalulator metadataCalulator)
  {
    _generator = generator;
    _metadataCalulator = metadataCalulator;
  }

  public List<ScheduleDayMetadata> GetSchedules(List<ScheduleFilter> filterList)
  {
    IEnumerable<IReadOnlyList<ScheduleItem>> schedules = _generator.GenerateSchedules();

    foreach (var filter in filterList)
    {
      schedules = schedules.Where(ConvertFilterToPredicate(filter));
    }

    return schedules
    .ToList()
    .Select(_metadataCalulator.GetMetadata)
    .OrderByDescending(m => m.Score)
    .ToList();
  }

  private static Func<IReadOnlyList<ScheduleItem>, bool> ConvertFilterToPredicate(ScheduleFilter filter)
  {
    return filter.Action switch
    {
      FilterAction.Starts => schedule => schedule.Any(item => item.Activity == filter.Activity && item.StartTime == filter.Time),
      FilterAction.Ends => schedule => schedule.Any(item => item.Activity == filter.Activity && item.EndTime == filter.Time),
      _ => throw new ArgumentException("Invalid filter action."),
    };
  }
}
