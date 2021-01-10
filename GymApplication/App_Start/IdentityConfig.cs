using GymApplication.DAL;
using GymApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Configuration;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Nexmo.Api;

namespace GymApplication
{
    #region 2FA
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            await configSendGridasync(message);
        }


        private async Task configSendGridasync(IdentityMessage message)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("befitapp1@gmail.com");
            var to = new EmailAddress(message.Destination);
            var subject = message.Subject;
            var content = message.Body;
            var htmlContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, htmlContent);

            await client.SendEmailAsync(msg);
        }
    }
    #endregion


    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<AuthDbContext>()));

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.MaxFailedAccessAttemptsBeforeLockout = 3;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromDays(365 * 100);

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            #region 2FA
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Kod uwierzytelniający",
                BodyFormat = "Twój kod jednorazowy do uwierzytelnienia to: {0}"
            });
            manager.EmailService = new EmailService();
            #endregion

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

}