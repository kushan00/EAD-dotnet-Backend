using backend.Models;
using backend.DTO;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ScheduleService _scheduleService;
    public ScheduleController(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Schedule>>> Get()
    {
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office")
        {
            return Unauthorized();
        }
        var schedules = await _scheduleService.GetScheduleAsync();

        return Ok(schedules);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSchedule([FromBody] ScheduleDTO scheduleDTO)
    {
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office" && LogUserAccount.UserRole != "Agent")
        {
            return Unauthorized();
        }

        Schedule schedule = new()
        {
            StartCity = scheduleDTO.StartCity,
            Cities = scheduleDTO.Cities,
            EndCity = scheduleDTO.EndCity,
            Price = scheduleDTO.Price,
            Train = scheduleDTO.Train,
            StartTime = scheduleDTO.StartTime,
            EndTime = scheduleDTO.EndTime,
            Class = scheduleDTO.Class,
            Type = scheduleDTO.Type,
            RunBy = scheduleDTO.RunBy,
            IsActive = scheduleDTO.IsActive
        };

        await _scheduleService.CreateScheduleAsync(schedule);

        return CreatedAtAction(nameof(Get), new { id = schedule.Id }, schedule);
    }

    [HttpGet("Search")]
    public async Task<IActionResult> SearchSchedule([FromBody] ScheduleSearchDTO scheduleSearchDTO)
    {
        var result = await _scheduleService.SearchScheduleAsync(scheduleSearchDTO.StartCity, scheduleSearchDTO.EndCity);

        List<Schedule> filteredSchedules = result.Where(s => _scheduleService.CheckTime(s.StartCity, s.EndCity, scheduleSearchDTO.Time)).ToList();

        return Ok(result);
    }

}