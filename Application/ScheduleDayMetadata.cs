namespace Napper.Application;

public record ScheduleDayMetadata
{
  public int Score { get; set; }
  public int NumberOfNaps { get; init;}
  public double NapHours { get; init;}
  public double AwakeHours { get; init;}
  public double NightHours { get; init;}
  public double TotalSleepHours { get; init;}
  public string[] ActivitiesIn30MinuteSpans { get; init; } = new string[36];
}
