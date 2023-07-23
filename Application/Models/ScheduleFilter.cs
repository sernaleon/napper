namespace Napper.Application;

public class ScheduleFilter
{
  public ScheduleActivity Activity { get; set; }
  public FilterAction Action { get; set; }
  public DateTime Time { get; set; }
}
