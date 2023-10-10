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
    private readonly ReservationService _reservationService;

    public ScheduleController(ScheduleService scheduleService, ReservationService reservationService)
    {
        _scheduleService = scheduleService;
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Schedule>>> Get()
    {
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office" && LogUserAccount.UserRole != "Agent")
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
            IsActive = true
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DisableSchedule(string id)
    {
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office" && LogUserAccount.UserRole != "Agent")
        {
            var existingSchedule = await _scheduleService.GetScheduleAsync(id);
            if (existingSchedule == null)
            {
                return NotFound();
            }

            long existingReservations = await _reservationService.GetCountScheduleReservationAsync(id);

            if (existingReservations > 0)
            {
                return NotFound("Cannot cancel a scheduler with existing reservations");
            }

            existingSchedule.IsActive = false;

            await _scheduleService.UpdateScheduleAsync(id, existingSchedule);

            return Ok();
        }
        return Unauthorized();
    }

    [HttpPut("enable/{id}")]
    public async Task<IActionResult> EnableSchedule(string id)
    {
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office" && LogUserAccount.UserRole != "Agent")
        {
            var existingSchedule = await _scheduleService.GetScheduleAsync(id);
            if (existingSchedule == null)
            {
                return NotFound();
            }

            existingSchedule.IsActive = true;

            await _scheduleService.UpdateScheduleAsync(id, existingSchedule);

            return Ok();
        }
        return Unauthorized();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(string id, [FromBody] ScheduleDTO scheduleDTO)
    {
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office" && LogUserAccount.UserRole != "Agent")
        {
            var existingSchedule = await _scheduleService.GetScheduleAsync(id);
            if (scheduleDTO == null)
            {
                return BadRequest("Schedule data is null.");
            }

            if (existingSchedule == null)
            {
                return NotFound();
            }

            existingSchedule.StartCity = scheduleDTO.StartCity ?? existingSchedule.StartCity;
            existingSchedule.Cities = scheduleDTO.Cities ?? existingSchedule.Cities;
            existingSchedule.EndCity = scheduleDTO.EndCity ?? existingSchedule.EndCity;
            existingSchedule.Price = scheduleDTO.Price ?? existingSchedule.Price;
            existingSchedule.Train = scheduleDTO.Train ?? existingSchedule.Train;
            existingSchedule.StartTime = scheduleDTO.StartTime ?? existingSchedule.StartTime;
            existingSchedule.EndTime = scheduleDTO.EndTime ?? existingSchedule.EndTime;
            existingSchedule.Class = scheduleDTO.Class ?? existingSchedule.Class;
            existingSchedule.Type = scheduleDTO.Type ?? existingSchedule.Type;
            existingSchedule.RunBy = scheduleDTO.RunBy ?? existingSchedule.RunBy;

            await _scheduleService.UpdateScheduleAsync(id, existingSchedule);
            return Ok();
        }
        return Unauthorized();
    }
}