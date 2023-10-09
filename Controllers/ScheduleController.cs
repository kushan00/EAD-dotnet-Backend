using backend.Models;
using backend.DTO;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

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
        var accounts = await _scheduleService.GetScheduleAsync();

        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSchedule([FromBody] ScheduleDTO scheduleDTO)
    {
        // Retrieve the account ID from HttpContext.Items
        if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office")
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

    [HttpGet]
    public async Task<IActionResult> SearchSchedule([FromBody] ScheduleSearchDTO scheduleSearchDTO)
    {
        Schedule schedule = new()
        {
            StartCity = scheduleSearchDTO.StartCity,
            EndCity = scheduleSearchDTO.EndCity,
            StartTime = scheduleSearchDTO.StartTime,
            EndTime = scheduleSearchDTO.EndTime,
            Class = scheduleSearchDTO.Class,
            Type = scheduleSearchDTO.Type
        };

        await _scheduleService.CreateScheduleAsync(schedule);

        return CreatedAtAction(nameof(Get), new { id = schedule.Id }, schedule);
    }

}