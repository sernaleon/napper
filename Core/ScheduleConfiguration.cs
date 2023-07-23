namespace Napper.Core;

public record ScheduleConfiguration
{
    public int TimeIncrementMinutes { get; set; }
    public DateTime StartOfSchedule { get; set; }
    public DateTime WakeUpTimeMin { get; set; }
    public DateTime WakeUpTimeMax { get; set; }
    public DateTime BedTimeMin { get; set; }
    public DateTime BedTimeMax { get; set; }
    public DateTime EndOfSchedule { get; set; }
    public int NapTimeMinutesMin { get; set; }
    public int NapTimeMinutesMax { get; set; }
    public int AwakeTimeMinutesMin { get; set; }
    public int AwakeTimeMinutesMax { get; set; }
    public int TargetNapCount { get; set; }
    public int TargetNapHours { get; set; }
    public int TargetAwakeHours { get; set; }
    public int TargetNightHours { get; set; }
    public int TargetTotalSleepHours { get; set; }
}
