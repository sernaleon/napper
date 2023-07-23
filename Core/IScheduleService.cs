namespace Napper.Core;

public interface IScheduleService 
{
  public List<ScheduleDayMetadata> GetSchedules(List<ScheduleFilter> filterList);
}