namespace Napper.Application;

using System.Linq;

public class ScheduleDay
{
  private readonly List<ScheduleItem> _schedule;

  public ScheduleDay(DateTime startTime, DateTime endTime, ScheduleActivity activity)
  {
    _schedule = new() { new ScheduleItem(startTime, endTime, activity) };
  }

  private ScheduleDay(List<ScheduleItem> schedule, ScheduleItem scheduleItem)
  {
    _schedule = new(schedule) { scheduleItem };
  }

  public IReadOnlyList<ScheduleItem> GetSchedule()
  {
    return _schedule;
  }

  public ScheduleDay NewWith(ScheduleActivity activity, int durationMinutes)
  {
    var item = NewScheduleItem(activity, durationMinutes);

    var result = new ScheduleDay(_schedule, item);
    return result;
  }

  public void Add(ScheduleActivity activity, DateTime endTime)
  {
    _schedule.Add(new ScheduleItem(GetLastEndTime(), endTime, activity));
  }

  public DateTime GetLastEndTime()
  {
    return _schedule.Last().EndTime;
  }

  private ScheduleItem NewScheduleItem(ScheduleActivity activity, int durationMinutes)
  {
    var startTime = GetLastEndTime();
    var endTime = startTime.AddMinutes(durationMinutes);
    var item = new ScheduleItem(startTime, endTime, activity);
    return item;
  }
}
