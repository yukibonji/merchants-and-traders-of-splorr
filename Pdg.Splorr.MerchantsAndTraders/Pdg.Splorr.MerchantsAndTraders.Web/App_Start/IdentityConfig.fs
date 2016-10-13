namespace Pdg.Splorr.MerchantsAndTraders.Web

open System
open System.Collections.Generic
open System.Data.Entity
open System.Linq
open System.Security.Claims
open System.Threading.Tasks
open System.Web
open Microsoft.AspNet.Identity
open Microsoft.AspNet.Identity.EntityFramework
open Microsoft.AspNet.Identity.Owin
open Microsoft.Owin
open Microsoft.Owin.Security

type ApplicationUser() =
    inherit IdentityUser()

    member this.GenerateUserIdentityAsync (manager:UserManager<ApplicationUser>) =
        manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie)        

type ApplicationDbContext() =
    inherit IdentityDbContext<ApplicationUser>("DefaultConnection", throwIfV1Schema=false)

    static member Create () =
        new ApplicationDbContext()

type EmailService() =
    interface IIdentityMessageService with
        member this.SendAsync(message:IdentityMessage): Task =
            Task.FromResult(0) :> Task

type SmsService() =
    interface IIdentityMessageService with
        member this.SendAsync(message:IdentityMessage): Task =
            Task.FromResult(0) :> Task

type ApplicationUserManager(store:IUserStore<ApplicationUser>) =
    inherit UserManager<ApplicationUser>(store)

    static member Create (options:IdentityFactoryOptions<ApplicationUserManager>) (context:IOwinContext):ApplicationUserManager =
            let manager =
                new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()))

            manager.UserValidator <- new UserValidator<ApplicationUser>(manager, AllowOnlyAlphanumericUserNames=false, RequireUniqueEmail=true)

            manager.PasswordValidator <- new PasswordValidator(RequiredLength = 6, RequireNonLetterOrDigit = true, RequireDigit = true, RequireLowercase = true, RequireUppercase = true)

            manager.UserLockoutEnabledByDefault <- true
            manager.DefaultAccountLockoutTimeSpan <- TimeSpan.FromMinutes(5.0)
            manager.MaxFailedAccessAttemptsBeforeLockout <- 5

            manager.RegisterTwoFactorProvider("Phone Code",new PhoneNumberTokenProvider<ApplicationUser>(MessageFormat = "Your security code is {0}"))
            manager.RegisterTwoFactorProvider("Email Code",new EmailTokenProvider<ApplicationUser>(Subject = "Security Code", BodyFormat = "Your security code is {0}"))
            
            manager.EmailService <- new EmailService()
            manager.SmsService <- new SmsService()


            let dataProtectionProvider = options.DataProtectionProvider
            if dataProtectionProvider <> null then
                manager.UserTokenProvider <-
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))

            manager

type ApplicationSignInManager( userManager:ApplicationUserManager,  authenticationManager:IAuthenticationManager) =
    inherit SignInManager<ApplicationUser, string>(userManager, authenticationManager)
    
    override this.CreateUserIdentityAsync(user:ApplicationUser) =
        user.GenerateUserIdentityAsync(this.UserManager :?> UserManager<ApplicationUser>)

    static member Create(options:IdentityFactoryOptions<ApplicationSignInManager>) (context:IOwinContext) :ApplicationSignInManager =
        new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication)

