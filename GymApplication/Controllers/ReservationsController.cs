﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GymApplication.DAL;
using GymApplication.Filters;
using GymApplication.Models;
using Microsoft.AspNet.Identity;

namespace GymApplication.Controllers
{
    [Authorize(Roles ="Admin")]
    [TwoFactorAuth]
    public class ReservationsController : ApiController
    {
        private GymDbContext db = new GymDbContext();

        // GET: api/Reservations
        public IQueryable<Reservations> GetReservations()
        {
            return db.Reservations;
        }

        // GET: api/Reservations/5
        [ResponseType(typeof(Reservations))]
        public async Task<IHttpActionResult> GetReservations(int id)
        {
            Reservations reservations = await db.Reservations.FindAsync(id);
            if (reservations == null)
            {
                return NotFound();
            }

            return Ok(reservations);
        }

        // PUT: api/Reservations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReservations(int id, Reservations reservations)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reservations.ID)
            {
                return BadRequest();
            }

            db.Entry(reservations).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }



        // DELETE: api/Reservations/5
        [ResponseType(typeof(Reservations))]
        public async Task<IHttpActionResult> DeleteReservations(int id)
        {
            Reservations reservations = await db.Reservations.FindAsync(id);
            if (reservations == null)
            {
                return NotFound();
            }

            Schedule schedule = await db.Schedules.FindAsync(reservations.ScheduleID);
            schedule.NumberOfAvailablePlaces++;

            db.Reservations.Remove(reservations);
            await db.SaveChangesAsync();

            return Ok(reservations);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReservationsExists(int id)
        {
            return db.Reservations.Count(e => e.ID == id) > 0;
        }



    }
}