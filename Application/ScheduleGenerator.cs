using Napper.Core;

namespace Napper.Application;

public class ScheduleGenerator
{
    private readonly ScheduleConfiguration _configuration;

    public ScheduleGenerator(ScheduleConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<IReadOnlyList<ScheduleItem>> GenerateSchedules(List<ScheduleFilter> filters)
    {
        List<IReadOnlyList<ScheduleItem>> schedules = new();

        var startTime = _configuration.StartOfSchedule;

        var endTime = _configuration.WakeUpTimeMax;

        while (endTime >= _configuration.WakeUpTimeMin)
        {
            var scheduleDay = new ScheduleDay(startTime, endTime, ScheduleActivity.NightTime);
            AddAwake(schedules, scheduleDay, filters);
            
            endTime = endTime.AddMinutes(- _configuration.TimeIncrementMinutes);
        }
        return schedules;
    }

    private void AddNap(List<IReadOnlyList<ScheduleItem>> results, ScheduleDay current, List<ScheduleFilter> filters)
    {
        if (!current.IsValid(filters) || current.GetLastEndTime() > _configuration.BedTimeMax)
        {
            return;
        }

        if (current.GetLastEndTime() >= _configuration.BedTimeMin)
        {
            current.Add(ScheduleActivity.NightTime, endTime: _configuration.EndOfSchedule);
            var schedule = current.GetSchedule();
            results.Add(schedule);
            return;
        }

        var napTimeMinutes = _configuration.NapTimeMinutesMax;

        while (napTimeMinutes >= _configuration.NapTimeMinutesMin)
        {
            var schedule = current.NewWith(ScheduleActivity.Nap, napTimeMinutes);
            AddAwake(results, schedule, filters);

            napTimeMinutes -= _configuration.TimeIncrementMinutes;
        }
    }

    private void AddAwake(List<IReadOnlyList<ScheduleItem>> results, ScheduleDay current, List<ScheduleFilter> filters)
    {
        if (!current.IsValid(filters) || current.GetLastEndTime() > _configuration.BedTimeMax)
        {
            return;
        }

        var awakeTimeMinutes = _configuration.AwakeTimeMinutesMax;

        while (awakeTimeMinutes >= _configuration.AwakeTimeMinutesMin)
        {
            var schedule = current.NewWith(ScheduleActivity.Awake, awakeTimeMinutes);
            AddNap(results, schedule, filters);

            awakeTimeMinutes -= _configuration.TimeIncrementMinutes;
        }
    }
}