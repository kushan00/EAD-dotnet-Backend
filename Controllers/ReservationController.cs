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
            if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount)
            {
                if (reservationDTO == null || reservationDTO.ReserveTime == null)
                {
                    return BadRequest("Reservation data is null.");
                }
                bool isAgentBooked = false;
                string user = LogUserAccount.Id;

                if (LogUserAccount.UserRole == "Back_Office")
                {
                    isAgentBooked = true;
                    user = reservationDTO.User;
                    // Calculate the difference in days between ReserveTime and BookedTime
                    TimeSpan difference = (TimeSpan)(reservationDTO.ReserveTime - DateTime.UtcNow);

                    bool isWithin30Days = difference.TotalDays <= 30;
                    long UserRecordCount = await _reservationService.GetCountUserReservationAsync(user);

                    if (!isWithin30Days)
                    {
                        return BadRequest("Reserve date is more than 30 days from Booked date");
                    }
                    if (UserRecordCount > 4)
                    {
                        return BadRequest("Agent reserved more than 4 times for this user");
                    }
                }

                // Find the count of existing records
                long previousRecordCount = await _reservationService.GetCountReservationAsync();

                // Generate the new BookingId based on the count
                string newBookingId = "Bkg" + previousRecordCount + 1;

                Reservation reservation = new()
                {
                    BookingId = newBookingId,
                    User = user,
                    Schedule = reservationDTO.Schedule,
                    BookedTime = DateTime.UtcNow,
                    ReserveTime = reservationDTO.ReserveTime,
                    StartCity = reservationDTO.StartCity,
                    EndCity = reservationDTO.EndCity,
                    PaxCount = reservationDTO.PaxCount,
                    TotalPrice = reservationDTO.TotalPrice,
                    Status = 1,
                    IsAgentBooked = isAgentBooked,
                };

                await _reservationService.CreateReservationAsync(reservation);

                return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
            }
            return Unauthorized();

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
            if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount)
            {
                var existingReservation = await _reservationService.GetReservationAsync(id);
                if (reservationDTO == null)
                {
                    return BadRequest("Reservation data is null.");
                }

                if (existingReservation == null)
                {
                    return NotFound();
                }
                if (LogUserAccount.UserRole == "Back_Office")
                {
                    TimeSpan difference = (TimeSpan)(existingReservation.ReserveTime - DateTime.UtcNow);

                    bool isWithin5days = difference.TotalDays <= 5;

                    if (!isWithin5days)
                    {
                        return BadRequest("Reserve Date is less than 5 days from Booked date");
                    }
                }

                existingReservation.User = reservationDTO.User ?? existingReservation.User;
                existingReservation.Schedule = reservationDTO.Schedule ?? existingReservation.Schedule;
                existingReservation.BookedTime = reservationDTO.BookedTime ?? existingReservation.BookedTime;
                existingReservation.ReserveTime = reservationDTO.ReserveTime ?? existingReservation.ReserveTime;
                existingReservation.StartCity = reservationDTO.StartCity ?? existingReservation.StartCity;
                existingReservation.EndCity = reservationDTO.EndCity ?? existingReservation.EndCity;
                existingReservation.PaxCount = reservationDTO.PaxCount ?? existingReservation.PaxCount;
                existingReservation.TotalPrice = reservationDTO.TotalPrice ?? existingReservation.TotalPrice;
                existingReservation.Status = reservationDTO.Status ?? existingReservation.Status;

                await _reservationService.UpdateReservationAsync(id, existingReservation);
                return Ok();
            }
            return Unauthorized();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DisableReservation(string id)
        {
            if (HttpContext.Items.TryGetValue("UserDetails", out var accountObj) && accountObj is Account LogUserAccount)
            {
                var existingReservation = await _reservationService.GetReservationAsync(id);
                if (existingReservation == null)
                {
                    return NotFound();
                }
                if (LogUserAccount.UserRole == "Back_Office")
                {
                    TimeSpan difference = (TimeSpan)(existingReservation.ReserveTime - DateTime.UtcNow);

                    bool isWithin5days = difference.TotalDays <= 5;

                    if (!isWithin5days)
                    {
                        return BadRequest("Reserve Date is less than 5 days from Booked date");
                    }

                }
                existingReservation.Status = 2;

                await _reservationService.UpdateReservationAsync(id, existingReservation);

                return Ok();
            }
            return Unauthorized();
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetByUserReservation(string id)
        {
            var reservation = await _reservationService.GetByUserReservationAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }
    }
}
