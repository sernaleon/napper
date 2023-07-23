namespace Napper.Core;

public record ScheduleItem(DateTime StartTime, DateTime EndTime, ScheduleActivity Activity);
