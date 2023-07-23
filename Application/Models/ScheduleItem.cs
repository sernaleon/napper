namespace Napper.Application;

public record ScheduleItem(DateTime StartTime, DateTime EndTime, ScheduleActivity Activity);
