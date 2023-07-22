using Microsoft.AspNetCore.Mvc;
using Napper.Application;

namespace napper.Controllers;

[ApiController]
[Route("api")]
public class SchedulesController : ControllerBase
{
    private readonly ScheduleService _scheduleService;

    public SchedulesController(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("schedules/table/")]
    public string[][] Table()
    {
        var result =  _scheduleService.GetSchedules(new());
        var table = result.ConvertToTable();
        return table;
    }

    [HttpGet("schedules/table/{*filters}")]
    public string[][] Table(string filters)
    {
        var scheduleFilters = ParseFiltersFromUrl(filters);
        var result =  _scheduleService.GetSchedules(scheduleFilters);
        var table = result.ConvertToTable();
        return table;
    }

    private static List<ScheduleFilter> ParseFiltersFromUrl(string filters)
    {
        List<ScheduleFilter> scheduleFilters = new();

        var filterSegments = filters.Split('/');
        for (int i = 0; i < filterSegments.Length; i += 3)
        {
            var activity = Enum.Parse<ScheduleActivity>(filterSegments[i], true);
            var action = Enum.Parse<FilterAction>(filterSegments[i+1], true);
            var time = DateTime.Parse(filterSegments[i + 2]);
            var filter = new ScheduleFilter { Activity = activity, Action = action, Time = time };
            scheduleFilters.Add(filter);
        }

        return scheduleFilters;
    }
}
