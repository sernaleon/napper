using Microsoft.AspNetCore.Mvc;
using Napper.Core;
using Napper.Mappers;

namespace napper.Controllers;

[ApiController]
[Route("api")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public SchedulesController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("schedules/table/")]
    public string[][] Table()
    {
        return Table("");
    }

    [HttpGet("schedules/table/{*filters}")]
    public string[][] Table(string filters)
    {
        var scheduleFilters = ParseFiltersFromUrl(filters);

        var result =  _scheduleService.GetSchedules(scheduleFilters);
        var table = result.ToScheduleTable();
        return table;
    }

    private static List<ScheduleFilter> ParseFiltersFromUrl(string filtersUrl)
    {
        List<ScheduleFilter> scheduleFilters = new();

        var filterSegments = filtersUrl.Split('/');

        if (filterSegments.Length % 3 != 0)
        {
            return scheduleFilters;
        }

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
