namespace Napper.Application;

using System.Linq;
using Napper.Core;

public class ScheduleDayMetadataCalulator
{
  private readonly ScheduleConfiguration _configuration;

  public ScheduleDayMetadataCalulator(ScheduleConfiguration configuration)
  {
    _configuration = configuration;
  }

  public ScheduleDayMetadata GetMetadata(IReadOnlyList<ScheduleItem> schedule)
  {
    var result = new ScheduleDayMetadata
    {
      Score = GetScore(schedule),
      NumberOfNaps = GetNumberOfNaps(schedule),
      NapHours = GetNapHours(schedule),
      AwakeHours = GetAwakeHours(schedule),
      NightHours = GetNightHours(schedule),
      TotalSleepHours = GetTotalSleepHours(schedule),
      ActivitiesIn30MinuteSpans = GetActivitiesIn30MinuteSpans(schedule)
    };
    return result;
  }

  private int GetScore(IReadOnlyList<ScheduleItem> schedule)
  {

    var napCountScore = CalculateScore(GetNumberOfNaps(schedule), _configuration.TargetNapCount);
    var napHoursScore = CalculateScore(GetNapHours(schedule), _configuration.TargetNapHours);
    var awakeHoursScore = CalculateScore(GetAwakeHours(schedule), _configuration.TargetAwakeHours);
    var nightHoursScore = CalculateScore(GetNightHours(schedule), _configuration.TargetNightHours);
    var totalSleepHoursScore = CalculateScore(GetTotalSleepHours(schedule), _configuration.TargetTotalSleepHours);

    var result = napCountScore
    + napHoursScore
    + awakeHoursScore
    + nightHoursScore
    + totalSleepHoursScore;

    return (int)result;
  }

  private static int GetNumberOfNaps(IReadOnlyList<ScheduleItem> schedule)
  {
    return schedule.Count(x => x.Activity == ScheduleActivity.Nap);
  }

  private static float GetNapHours(IReadOnlyList<ScheduleItem> schedule)
  {
    return GetActivityHours(schedule, ScheduleActivity.Nap);
  }

  private static float GetAwakeHours(IReadOnlyList<ScheduleItem> schedule)
  {
    return GetActivityHours(schedule, ScheduleActivity.Awake);
  }

  private float GetNightHours(IReadOnlyList<ScheduleItem> schedule)
  {
    return GetActivityHours(schedule, ScheduleActivity.NightTime) 
    + 24
    - (float)(_configuration.EndOfSchedule - _configuration.StartOfSchedule).TotalHours;
  }

  private float GetTotalSleepHours(IReadOnlyList<ScheduleItem> schedule)
  {
    return GetNightHours(schedule) + GetNapHours(schedule);
  }

  private static float GetActivityHours(IReadOnlyList<ScheduleItem> schedule, ScheduleActivity activity)
  {
    return schedule
    .Where(x => x.Activity == activity)
    .Sum(n => (int)(n.EndTime - n.StartTime).TotalMinutes) / (float)60;
  }

  private static string[] GetActivitiesIn30MinuteSpans(IReadOnlyList<ScheduleItem> schedule)
  {
    var result = new List<string>();

    var startTime = schedule[0].StartTime;
    var endTime = schedule[schedule.Count - 1].StartTime;
    while (startTime <= endTime)
    {
      var matchingSchedule = schedule.First(schedule => startTime >= schedule.StartTime && startTime < schedule.EndTime);
      var activity = matchingSchedule.Activity.ToString();

      result.Add(activity);

      startTime = startTime.AddMinutes(30);
    }

    return result.ToArray();
  }

  private static float CalculateScore(float actual, float target)
  {
    return Math.Abs(1000 - 100 * Math.Abs(actual - target));
  }
}
