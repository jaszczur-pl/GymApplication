using System;
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

namespace GymApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    [TwoFactorAuth]
    public class ClassesController : ApiController
    {
        private GymDbContext db = new GymDbContext();

        // GET: api/Classes
        public IQueryable<Classes> GetClasses()
        {
            return db.Classes;
        }

        // GET: api/Classes/5
        [ResponseType(typeof(Classes))]
        public async Task<IHttpActionResult> GetClasses(int id)
        {
            Classes classes = await db.Classes.FindAsync(id);
            if (classes == null)
            {
                return NotFound();
            }

            return Ok(classes);
        }

        // PUT: api/Classes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutClasses(int id, Classes classes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != classes.ID)
            {
                return BadRequest();
            }

            db.Entry(classes).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassesExists(id))
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

// POST: api/Classes
[ResponseType(typeof(Classes))]
public async Task<IHttpActionResult> PostClasses(Classes classes)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    db.Classes.Add(classes);
    await db.SaveChangesAsync();

    return CreatedAtRoute("DefaultApi", new { id = classes.ID }, classes);
}

        // DELETE: api/Classes/5
        [ResponseType(typeof(Classes))]
        public async Task<IHttpActionResult> DeleteClasses(int id)
        {
            Classes classes = await db.Classes.FindAsync(id);
            if (classes == null)
            {
                return NotFound();
            }

            db.Classes.Remove(classes);
            await db.SaveChangesAsync();

            return Ok(classes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClassesExists(int id)
        {
            return db.Classes.Count(e => e.ID == id) > 0;
        }
    }
}