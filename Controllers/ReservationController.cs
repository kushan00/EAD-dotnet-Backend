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
    public class ReservationController : ControllerBase
    {
        private readonly ReservationService _reservationService;

        public ReservationController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationDTO reservationDTO)
        {
            if (reservationDTO == null)
            {
                return BadRequest("Reservation data is null.");
            }

            Reservation reservation = new Reservation
            {
                BookingId = reservationDTO.BookingId,
                User = reservationDTO.User,
                Schedule = reservationDTO.Schedule,
                BookedTime = reservationDTO.BookedTime,
                ReserveTime = reservationDTO.ReserveTime,
                StartCity = reservationDTO.StartCity,
                EndCity = reservationDTO.EndCity,
                PaxCount = reservationDTO.PaxCount,
                Status = reservationDTO.Status
            };

            await _reservationService.CreateReservationAsync(reservation);

            return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation(string id)
        {
            var reservation = await _reservationService.GetReservationAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _reservationService.GetReservationAsync();
            return Ok(reservations);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(string id, [FromBody] ReservationDTO reservationDTO)
        {
            if (reservationDTO == null)
            {
                return BadRequest("Reservation data is null.");
            }

            var existingReservation = await _reservationService.GetReservationAsync(id);

            if (existingReservation == null)
            {
                return NotFound();
            }

            existingReservation.BookingId = reservationDTO.BookingId;
            existingReservation.User = reservationDTO.User;
            existingReservation.Schedule = reservationDTO.Schedule;
            existingReservation.BookedTime = reservationDTO.BookedTime;
            existingReservation.ReserveTime = reservationDTO.ReserveTime;
            existingReservation.StartCity = reservationDTO.StartCity;
            existingReservation.EndCity = reservationDTO.EndCity;
            existingReservation.PaxCount = reservationDTO.PaxCount;
            existingReservation.Status = reservationDTO.Status;

            await _reservationService.UpdateReservationAsync(id, existingReservation);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(string id)
        {
            var existingReservation = await _reservationService.GetReservationAsync(id);

            if (existingReservation == null)
            {
                return NotFound();
            }

            await _reservationService.RemoveReservationAsync(id);

            return NoContent();
        }
    }
}
