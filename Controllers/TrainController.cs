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

        public TrainController(TrainService trainService)
        {
            _trainService = trainService;
        }

        // GET: api/Train
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainDTO>>> GetTrains()
        {
            var trains = await _trainService.GetTrainAsync();
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
                Id = trainDTO.Id ?? Guid.NewGuid().ToString(), // Generate a new Id if not provided
                TrainId = trainDTO.TrainId,
                Name = trainDTO.Name,
                SeatingCapacity = trainDTO.SeatingCapacity,
                FuelType = trainDTO.FuelType,
                Model = trainDTO.Model,
                IsActive = trainDTO.IsActive ?? false, // Default to false if not provided
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

            var updatedTrain = new Train
            {
                Id = existingTrain.Id,
                TrainId = trainDTO.TrainId,
                Name = trainDTO.Name,
                SeatingCapacity = trainDTO.SeatingCapacity,
                FuelType = trainDTO.FuelType,
                Model = trainDTO.Model,
                IsActive = trainDTO.IsActive ?? false,
            };

            await _trainService.UpdateTrainAsync(id, updatedTrain);

            return NoContent();
        }

        // DELETE: api/Train/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrain(string id)
        {
            var existingTrain = await _trainService.GetTrainAsync(id);

            if (existingTrain == null)
            {
                return NotFound();
            }

            await _trainService.RemoveTrainAsync(id);

            return NoContent();
        }
    }
}
