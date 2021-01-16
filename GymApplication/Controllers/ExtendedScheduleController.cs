using GymApplication.DAL;
using GymApplication.Filters;
using GymApplication.Models;
using GymApplication.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GymApplication.Controllers
{
    [Authorize]
    [TwoFactorAuth]
    public class ExtendedScheduleController : ApiController
    {
        private ScheduleRepository _repository;
        private GymDbContext db = new GymDbContext();

        public ExtendedScheduleController()
        {
            _repository = new ScheduleRepository();
        }

        // GET: api/ExtendedSchedules
        public IEnumerable<ExtendedScheduleView> GetExtendedSchedules()
        {
            var result = _repository.GetSchedules();

            return result;
        }

        [Route("api/ExtendedSchedule/GetCustomerReservation/{scheduleID}")]
        public IHttpActionResult GetCustomerReservation(int scheduleID)
        {
            string customerID = RequestContext.Principal.Identity.GetUserId();
            Reservations reservation = db.Reservations.SingleOrDefault(e => e.ScheduleID == scheduleID && e.CustomerID == customerID);

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }


        [HttpPost]
        [Route("api/ExtendedSchedule/PostReservation")]
        public async Task<IHttpActionResult> PostReservation(ReservationModel reservationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string customerID = RequestContext.Principal.Identity.GetUserId();

            if (ReservationExists(reservationModel.ScheduleID, customerID))
            {
                return BadRequest("Rezerwacja już istnieje");
            }

            if (!DecrementScheduleAvailablePlaces(reservationModel.ScheduleID))
            {
                return BadRequest("Wszystkie miejsca są już zajęte");
            }

            var reservation = new Reservations
            {
                CustomerID = customerID,
                ScheduleID = reservationModel.ScheduleID
            };

            db.Reservations.Add(reservation);
            await db.SaveChangesAsync();

            return Ok("Dodano rezerwację");
        }

        [HttpDelete]
        [Route("api/ExtendedSchedule/RemoveReservation/{scheduleID}")]
        public async Task<IHttpActionResult> RemoveReservation(int scheduleID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string customerID = RequestContext.Principal.Identity.GetUserId();
            var reservation = db.Reservations.SingleOrDefault(e => e.ScheduleID == scheduleID && e.CustomerID == customerID);

            if (reservation == null)
            {
                BadRequest("Nie znaleziono rezerwacji");
            }

            if (!IncrementScheduleAvailablePlaces(scheduleID))
            {
                return BadRequest("Nie znaleziono rezerwacji");
            }

            db.Reservations.Remove(reservation);
            await db.SaveChangesAsync();

            return Ok("Usunięto rezerwację");
        }


        private bool ReservationExists(int scheduleID, string customerID)
        {
            return db.Reservations.Count(e => e.ScheduleID == scheduleID && e.CustomerID == customerID) > 0;
        }

        private bool DecrementScheduleAvailablePlaces(int ScheduleID)
        {
            var schedule = db.Schedules.SingleOrDefault(e => e.ID == ScheduleID);
            if (schedule == null || schedule.NumberOfAvailablePlaces <= 0)
            {
                return false;
            }

            schedule.NumberOfAvailablePlaces--;

            return true;
        }

        private bool IncrementScheduleAvailablePlaces(int ScheduleID)
        {
            var schedule = db.Schedules.SingleOrDefault(e => e.ID == ScheduleID);
            if (schedule == null)
            {
                return false;
            }

            schedule.NumberOfAvailablePlaces++;

            return true;
        }

    }
}
