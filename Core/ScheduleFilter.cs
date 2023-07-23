namespace Napper.Core;

public record ScheduleFilter
{
  public ScheduleActivity Activity { get; set; }
  public FilterAction Action { get; set; }
  public DateTime Time { get; set; }
}