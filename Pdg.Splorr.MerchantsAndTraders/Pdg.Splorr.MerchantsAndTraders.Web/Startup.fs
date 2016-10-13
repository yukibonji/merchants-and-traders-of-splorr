namespace Pdg.Splorr.MerchantsAndTraders.Web

open System
open Owin
open Microsoft.AspNet.Identity
open Microsoft.AspNet.Identity.Owin
open Microsoft.Owin.Security.Cookies
open Microsoft.Owin
open Pdg.Splorr.MerchantsAndTraders.Web.Models

type Startup() =
    member this.ConfigureAuth(app:IAppBuilder) =
        app
            .CreatePerOwinContext(ApplicationDbContext.Create)
            .CreatePerOwinContext(ApplicationUserManager.Create)
            .CreatePerOwinContext(ApplicationSignInManager.Create)
            .UseCookieAuthentication(
                CookieAuthenticationOptions(
                    AuthenticationType=DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = PathString("/Account/Login"),
                    Provider = CookieAuthenticationProvider(
                        OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                            validateInterval= TimeSpan.FromMinutes(30.0),
                            regenerateIdentity= fun manager user -> user.GenerateUserIdentityAsync(manager))
                        )
                    ))
            .UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie)

        app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5.0))

        app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie)

    member this.Configuration (app:IAppBuilder) =
        this.ConfigureAuth(app)

[<assembly: OwinStartup(typedefof<Startup>)>]
do
    ()
