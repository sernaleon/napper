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

        var endTime = _configuration.WakeUpTimeMin;

        while (endTime < _configuration.WakeUpTimeMax)
        {
            var scheduleDay = new ScheduleDay(startTime, endTime, ScheduleActivity.NightTime);
            AddAwake(schedules, scheduleDay, filters);
            endTime = endTime.AddMinutes(_configuration.TimeIncrementMinutes);
        }
        return schedules;
    }

    private void AddNap(List<IReadOnlyList<ScheduleItem>> results, ScheduleDay current, List<ScheduleFilter> filters)
    {
        if (current.GetLastEndTime() >= _configuration.LastNapEndTime || !current.IsValid(filters))
        {
            return;
        }

        var napTimeMinutes = _configuration.NapTimeMinutesMin;

        while (napTimeMinutes <= _configuration.NapTimeMinutesMax)
        {
            var schedule = current.NewWith(ScheduleActivity.Nap, napTimeMinutes);
            AddAwake(results, schedule, filters);

            napTimeMinutes += _configuration.TimeIncrementMinutes;
        }
    }

    private void AddAwake(List<IReadOnlyList<ScheduleItem>> results, ScheduleDay current, List<ScheduleFilter> filters)
    {
        if (current.GetLastEndTime() > _configuration.LastNapEndTime || !current.IsValid(filters))
        {
            return;
        }

        if (current.GetLastEndTime() == _configuration.LastNapEndTime)
        {
            current.Add(ScheduleActivity.Awake, endTime: _configuration.BedTime);
            current.Add(ScheduleActivity.NightTime, endTime: _configuration.EndOfSchedule);
            var schedule = current.GetSchedule();
            results.Add(schedule);
            return;
        }

        var awakeTimeMinutes = _configuration.AwakeTimeMinutesMin;

        while (awakeTimeMinutes <= _configuration.AwakeTimeMinutesMax)
        {
            var schedule = current.NewWith(ScheduleActivity.Awake, awakeTimeMinutes);
            AddNap(results, schedule, filters);

            awakeTimeMinutes += _configuration.TimeIncrementMinutes;
        }
    }
}