using GymApplication.DAL;
using GymApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace GymApplication.Provider
{
    public class OAuthAppProvider: OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public OAuthAppProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "Nazwa użytkownika lub hasło są niepoprawne.");
                return;
            }

            #region 2FA
            var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user.Id);
            if (isTwoFactorEnabled)
            {
                user.SecondFactorVerified = false;
                var updateResult = await userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    context.SetError("Błąd przy aktualizacji bazy uwierzytelniającej.");
                    return;
                }

                var code = await userManager.GenerateTwoFactorTokenAsync(user.Id, "EmailCode");
                IdentityResult notifyResulty = await userManager.NotifyTwoFactorTokenAsync(user.Id, "EmailCode", code);

                if (!notifyResulty.Succeeded)
                {
                    context.SetError("Błąd przy wysłaniu kod jednorazowego.");
                    return;
                }
            }
            #endregion

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            List<Claim> roles = oAuthIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

            AuthenticationProperties properties = CreateProperties(user, JsonConvert.SerializeObject(roles.Select(x => x.Value)));
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(ApplicationUser user, string role)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                {"userName", user.UserName },
                {"roles", role },
#region 2FA
                {"requireOTP", user.TwoFactorEnabled.ToString() },
                {"secondFactorVerified", user.SecondFactorVerified.ToString() }
#endregion
            };
            return new AuthenticationProperties(data);
        }

    }
}