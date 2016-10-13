namespace Pdg.Splorr.MerchantsAndTraders.Web.Controllers

open System
open System.Globalization
open System.Linq
open System.Security.Claims
open System.Threading.Tasks
open System.Web
open System.Web.Mvc
open System.Web.Routing
open Microsoft.AspNet.Identity
open Microsoft.AspNet.Identity.Owin
open Microsoft.Owin.Security
open Pdg.Splorr.MerchantsAndTraders.DataLayer
open Pdg.Splorr.MerchantsAndTraders.Web
open Pdg.Splorr.MerchantsAndTraders.Web.Models
open FSharp.Interop.Dynamic

type AccountController() =
    inherit Controller()

    [<DefaultValue>]val mutable _signInManager: ApplicationSignInManager
    [<DefaultValue>]val mutable _userManager: ApplicationUserManager

    member this.SignInManager 
        with public get() = 
            if obj.ReferenceEquals(this._signInManager, null) |> not then 
                this._signInManager 
            else 
                this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>()

        and private set value = 
            this._signInManager <- value

    member this.UserManager 
        with public get() = 
            if obj.ReferenceEquals(this._userManager, null) |> not then 
                this._userManager 
            else 
                this.HttpContext.GetOwinContext().Get<ApplicationUserManager>()

        and private set value = 
            this._userManager <- value

    [<AllowAnonymous>]
    member this.Login (returnUrl:string) : ActionResult =
        this.ViewData.Add("ReturnUrl", returnUrl)
        this.View() :> ActionResult

    member private this.RedirectToLocal (returnUrl:string) : ActionResult =
        if (this.Url.IsLocalUrl(returnUrl)) then
            this.Redirect(returnUrl) :> ActionResult
        else
            this.RedirectToAction("Index", "Home") :> ActionResult

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member this.Login (model:LoginViewModel, returnUrl: string) : ActionResult =
        if this.ModelState.IsValid |> not then
            this.View(model) :> ActionResult
        else
            let result = 
                this.SignInManager.PasswordSignIn(model.Email, model.Password, model.RememberMe, shouldLockout = false)

            match result with
            | SignInStatus.Success ->
                this.RedirectToLocal returnUrl

            | SignInStatus.LockedOut ->
                this.View("Lockout") :> ActionResult

            | SignInStatus.RequiresVerification ->
                let rvd = new RouteValueDictionary()
                rvd.Add("ReturnUrl",returnUrl)
                rvd.Add("RememberMe", model.RememberMe)
                this.RedirectToAction("SendCode", rvd) :> ActionResult

            | _ ->
                this.ModelState.AddModelError("","Invalid login attempt.")
                this.View(model) :> ActionResult

    [<AllowAnonymous>]
    member this.VerifyCode (provider:string, returnUrl:string, rememberMe:bool) : ActionResult =
        if this.SignInManager.HasBeenVerified() |> not then
            this.View("Error") :> ActionResult
        else
            this.View(new VerifyCodeViewModel(Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe)) :> ActionResult

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member this.VerifyCode (model:VerifyCodeViewModel) : ActionResult =
        if this.ModelState.IsValid |> not then
            this.View(model) :> ActionResult
        else
            let result = this.SignInManager.TwoFactorSignIn(model.Provider, model.Code, isPersistent=  model.RememberMe, rememberBrowser= model.RememberBrowser)
            match result with 
            | SignInStatus.Success ->
                this.RedirectToLocal(model.ReturnUrl)

            | SignInStatus.LockedOut ->
                this.View("Lockout") :> ActionResult

            | _ ->
                this.ModelState.AddModelError("", "Invalid code.")
                this.View(model) :> ActionResult

    [<AllowAnonymous>]
    member this.Register() : ActionResult =
        this.View() :> ActionResult

    member private this.AddErrors (result:IdentityResult) : unit =
        result.Errors
        |> Seq.iter(fun error -> this.ModelState.AddModelError("",error))

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member this.Register(model:RegisterViewModel) :ActionResult= 
        if this.ModelState.IsValid then
            let user = new ApplicationUser(UserName = model.Email, Email = model.Email)
            let result = this.UserManager.Create(user, model.Password)
            if result.Succeeded then
                this.SignInManager.SignIn(user, isPersistent=false, rememberBrowser=false)
                this.RedirectToAction("Index","Home") :> ActionResult
            else
                this.AddErrors(result);
                this.View(model) :> ActionResult
        else
            this.View(model) :> ActionResult

    [<AllowAnonymous>]
    member this.ConfirmEmail(userId:string, code:string) : ActionResult =
        if obj.ReferenceEquals(userId,null) || obj.ReferenceEquals(code,null) then
            this.View("Error") :> ActionResult
        else
            let result = this.UserManager.ConfirmEmail(userId, code)
            this.View(if result.Succeeded then "ConfirmEmail" else "Error") :> ActionResult

    [<AllowAnonymous>]
    member this.ForgotPassword() : ActionResult =
        this.View() :> ActionResult

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member this.ForgotPassword(model:ForgotPasswordViewModel) : ActionResult =
        if this.ModelState.IsValid then
            let user = this.UserManager.FindByName(model.Email)
            if obj.ReferenceEquals(user,null) || (this.UserManager.IsEmailConfirmed(user.Id)|> not) then
                this.View("ForgotPasswordConfirmation") :> ActionResult
            else
                this.View(model) :> ActionResult
        else
            this.View(model) :> ActionResult

    [<AllowAnonymous>]
    member this.ForgotPasswordConfirmation() : ActionResult =
        this.View() :> ActionResult

    [<AllowAnonymous>]
    member this.ResetPassword(code:string) : ActionResult =
        (if obj.ReferenceEquals(code,null) then this.View("Error") else this.View()) :> ActionResult

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member this.ResetPassword(model:ResetPasswordViewModel) : ActionResult =
        if this.ModelState.IsValid |> not then
            this.View(model) :> ActionResult
        else
            let user = this.UserManager.FindByName(model.Email);
            if obj.ReferenceEquals(user,null) then
                this.RedirectToAction("ResetPasswordConfirmation", "Account") :> ActionResult
            else
                let result = this.UserManager.ResetPassword(user.Id, model.Code, model.Password)
                if result.Succeeded then
                    this.RedirectToAction("ResetPasswordConfirmation", "Account") :> ActionResult
                else
                    this.AddErrors(result)
                    this.View() :> ActionResult

    [<AllowAnonymous>]
    member this.ResetPasswordConfirmation() : ActionResult =
        this.View() :> ActionResult

//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public ActionResult ExternalLogin(string provider, string returnUrl)
//        {
//            // Request a redirect to the external login provider
//            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
//        }
//
//        //
//        // GET: /Account/SendCode
//        [AllowAnonymous]
//        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
//        {
//            var userId = await SignInManager.GetVerifiedUserIdAsync();
//            if (userId == null)
//            {
//                return View("Error");
//            }
//            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
//            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
//            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
//        }
//
//        //
//        // POST: /Account/SendCode
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> SendCode(SendCodeViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View();
//            }
//
//            // Generate the token and send it
//            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
//            {
//                return View("Error");
//            }
//            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
//        }
//
//        //
//        // GET: /Account/ExternalLoginCallback
//        [AllowAnonymous]
//        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
//        {
//            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
//            if (loginInfo == null)
//            {
//                return RedirectToAction("Login");
//            }
//
//            // Sign in the user with this external login provider if the user already has a login
//            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
//            switch (result)
//            {
//                case SignInStatus.Success:
//                    return RedirectToLocal(returnUrl);
//                case SignInStatus.LockedOut:
//                    return View("Lockout");
//                case SignInStatus.RequiresVerification:
//                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
//                case SignInStatus.Failure:
//                default:
//                    // If the user does not have an account, then prompt the user to create an account
//                    ViewBag.ReturnUrl = returnUrl;
//                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
//                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
//            }
//        }
//
//        //
//        // POST: /Account/ExternalLoginConfirmation
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
//        {
//            if (User.Identity.IsAuthenticated)
//            {
//                return RedirectToAction("Index", "Manage");
//            }
//
//            if (ModelState.IsValid)
//            {
//                // Get the information about the user from the external login provider
//                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
//                if (info == null)
//                {
//                    return View("ExternalLoginFailure");
//                }
//                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
//                var result = await UserManager.CreateAsync(user);
//                if (result.Succeeded)
//                {
//                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
//                    if (result.Succeeded)
//                    {
//                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
//                        return RedirectToLocal(returnUrl);
//                    }
//                }
//                AddErrors(result);
//            }
//
//            ViewBag.ReturnUrl = returnUrl;
//            return View(model);
//        }
//
//        //
//        // POST: /Account/LogOff
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult LogOff()
//        {
//            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//            return RedirectToAction("Index", "Home");
//        }

    [<AllowAnonymous>]
    member this.ExternalLoginFailure() : ActionResult =
        this.View() :> ActionResult
//
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (_userManager != null)
//                {
//                    _userManager.Dispose();
//                    _userManager = null;
//                }
//
//                if (_signInManager != null)
//                {
//                    _signInManager.Dispose();
//                    _signInManager = null;
//                }
//            }
//
//            base.Dispose(disposing);
//        }
//
//        #region Helpers
//        // Used for XSRF protection when adding external logins
//        private const string XsrfKey = "XsrfId";
//
//        private IAuthenticationManager AuthenticationManager
//        {
//            get
//            {
//                return HttpContext.GetOwinContext().Authentication;
//            }
//        }
//
//
//
//        #endregion
//    }

