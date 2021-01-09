using GymApplication.DAL;
using GymApplication.Filters;
using GymApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;

namespace GymApplication.Controllers
{
    [Authorize]
    public class AccountController : ApiController

    {
        private ApplicationUserManager _userManager;
        private GymDbContext db = new GymDbContext();

        // POST: api/Account
        [AllowAnonymous]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { 
                UserName = model.Email, 
                Email = model.Email, 
                TwoFactorEnabled = true };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var customer = new Customer() { 
                    ID = user.Id, 
                    Email = model.Email, 
                    Name = model.Name, 
                    Surname = model.Surname };

                db.Customers.Add(customer);
                await db.SaveChangesAsync();

                await UserManager.AddToRoleAsync(user.Id, "User");
            }
            else
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }



        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }


        #region 2FA
        [HttpGet]
        [Route("api/Account/VerifyOTP/{code}")]
        public async Task<IHttpActionResult> VerifyOTP(string code)
        {
            try
            {
                var userID = User.Identity.GetUserId();
                bool verified = 
                    await UserManager.VerifyTwoFactorTokenAsync(userID, "EmailCode", code);

                if (!verified)
                    return BadRequest($"Kod {code} nie jest poprawny.");

                var user = await UserManager.FindByNameAsync(User.Identity.Name);
                user.SecondFactorVerified = true;
                var result = await UserManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest();
                }

                return Ok("Kod zweryfikowany pomyślnie.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        [HttpGet]
        [Route("api/Account/Logout")]
        public async Task<IHttpActionResult> Logout()
        {
            try
            {
                var user = await UserManager.FindByNameAsync(User.Identity.Name);
                user.SecondFactorVerified = false;
                var result = await UserManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest();
                }

                return Ok("OK");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [TwoFactorAuth]
        [HttpPut]
        [Route("api/Account/SetClientData")]
        public async Task<IHttpActionResult> SetClientData(ClientProfileBindingModel model)
        {
            try
            {
                string id = RequestContext.Principal.Identity.GetUserId();
                var result = await UserManager.SetTwoFactorEnabledAsync(id, model.TwoFactorEnabled);

                if (!result.Succeeded)
                {
                    return BadRequest();
                }

                Customer customer = await db.Customers.FindAsync(id);
                customer.Name = model.Name;
                customer.Surname = model.Surname;
                customer.Email = model.Email;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok("OK");
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [TwoFactorAuth]
        [Route("api/Account/GetClientData")]
        public async Task<IHttpActionResult> GetClientData()
        {
            string id = RequestContext.Principal.Identity.GetUserId();
            Customer customer = await db.Customers.FindAsync(id);
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            
            if (customer == null || user == null)
            {
                return NotFound();
            }

            ClientProfileBindingModel profile = new ClientProfileBindingModel
            {
                Name = customer.Name,
                Surname = customer.Surname,
                Email = customer.Email,
                TwoFactorEnabled = user.TwoFactorEnabled
            };

            return Ok(profile);
        }

        private bool CustomerExists(string id)
        {
            return db.Customers.Count(e => e.ID == id) > 0;
        }
    }

}
