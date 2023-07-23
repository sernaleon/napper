using Napper.Core;

namespace Napper.Application;

public class ScheduleService : IScheduleService
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
    return _generator
    .GenerateSchedules(filterList)
    .ToList()
    .Select(_metadataCalulator.GetMetadata)
    .OrderByDescending(m => m.Score)
    .ToList();
  }
}
