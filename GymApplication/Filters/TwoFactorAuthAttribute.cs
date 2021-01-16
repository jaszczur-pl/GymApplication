using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GymApplication.Filters
{
    public class TwoFactorAuthAttribute: AuthorizeAttribute
    {
        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            var userManager = 
                        HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();

            if (userManager == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized, new ResponseData
                    {
                        Code = 100,
                        Message = "Błąd przy autoryzacji użytkownika."
                    });
                return;
            }

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            var user = await userManager.FindByNameAsync(principal?.Identity?.Name);
            if (user == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized, new ResponseData
                    {
                        Code = 100,
                        Message = "Błąd przy autoryzacji użytkownika."
                    });
                return;
            }

            if (user.TwoFactorEnabled && !user.SecondFactorVerified)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized, new ResponseData
                    {
                        Code = 101,
                        Message = "Użytkownik musi być uwierzytelniony dwoma składnikami."
                    });
            }
            return;
        }
    }

    public class ResponseData
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
