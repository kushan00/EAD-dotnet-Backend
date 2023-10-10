using backend.DTO;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainController : ControllerBase
    {
        private readonly TrainService _trainService;
        private readonly ScheduleService _scheduleService;
        private readonly ReservationService _reservationService;


        public TrainController(TrainService trainService, ScheduleService scheduleService, ReservationService reservationService)
        {
            _trainService = trainService;
            _scheduleService = scheduleService;
            _reservationService = reservationService;
        }

        // GET: api/Train
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainDTO>>> GetTrains()
        {
            var trains = await _trainService.GetTrainAsync();
            return Ok(trains);
        }
        // GET: api/Train
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TrainDTO>>> GetActiveTrains()
        {
            var trains = await _trainService.GetActiveTrainAsync();
            return Ok(trains);
        }

        // GET: api/Train/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TrainDTO>> GetTrain(string id)
        {
            var train = await _trainService.GetTrainAsync(id);

            if (train == null)
            {
                return NotFound();
            }

            return Ok(train);
        }

        // POST: api/Train
        [HttpPost]
        public async Task<ActionResult<TrainDTO>> CreateTrain(TrainDTO trainDTO)
        {
            var train = new Train
            {
                TrainId = trainDTO.TrainId,
                Name = trainDTO.Name,
                SeatingCapacity = trainDTO.SeatingCapacity,
                FuelType = trainDTO.FuelType,
                Model = trainDTO.Model,
                IsActive = true, // Default to false if not provided
            };

            await _trainService.CreateTrainAsync(train);

            return CreatedAtAction(nameof(GetTrain), new { id = train.Id }, train);
        }

        // PUT: api/Train/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrain(string id, TrainDTO trainDTO)
        {
            var existingTrain = await _trainService.GetTrainAsync(id);

            if (existingTrain == null)
            {
                return NotFound();
            }

            existingTrain.Id = existingTrain.Id;
            existingTrain.TrainId = trainDTO.TrainId ?? existingTrain.TrainId;
            existingTrain.Name = trainDTO.Name ?? existingTrain.Name;
            existingTrain.SeatingCapacity = trainDTO.SeatingCapacity ?? existingTrain.SeatingCapacity;
            existingTrain.FuelType = trainDTO.FuelType ?? existingTrain.FuelType;
            existingTrain.Model = trainDTO.Model ?? existingTrain.Model;

            await _trainService.UpdateTrainAsync(id, existingTrain);

            return NoContent();
        }

        // PUT: api/Train/5
        [HttpPut("enable/{id}")]
        public async Task<IActionResult> EnableTrain(string id)
        {
            var existingTrain = await _trainService.GetTrainAsync(id);

            if (existingTrain == null)
            {
                return NotFound();
            }

            existingTrain.IsActive = true;

            await _trainService.UpdateTrainAsync(id, existingTrain);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DisableTrain(string id)
        {
            // Retrieve the account ID from HttpContext.Items
            if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount && LogUserAccount.UserRole != "Back_Office")
            {
                return Unauthorized();
            }

            var train = await _trainService.GetTrainAsync(id);

            if (train is null)
            {
                return NotFound("Train not found");
            }

            var schedules = await _scheduleService.GetTrainScheduleAsync(id);

            if (schedules is null)
            {
                return NotFound("Schedule not found");
            }

            foreach (var schedule in schedules)
            {
                long result = await _reservationService.GetCountScheduleReservationAsync(schedule.Id);
                if (result > 0)
                {
                    return NotFound("Cannot cancel a train with existing reservations");
                }
            }

            train.IsActive = false;

            await _trainService.UpdateTrainAsync(id, train);

            foreach (var schedule in schedules)
            {
                schedule.IsActive = false;

                await _scheduleService.UpdateScheduleAsync(id, schedule);
            }

            return Ok();
        }
    }
}
