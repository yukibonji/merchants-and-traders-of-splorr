﻿namespace Pdg.Splorr.MerchantsAndTraders.Web.Controllers

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

type internal ChallengeResult(provider:string, redirectUri:string, userId:string) as this=

    inherit HttpUnauthorizedResult()

    do
        this.LoginProvider <- provider
        this.RedirectUri <- redirectUri
        this.UserId <- userId

    member val LoginProvider = String.Empty with get,set
    member val RedirectUri = String.Empty with get,set
    member val UserId = String.Empty with get,set

    override this.ExecuteResult(context:ControllerContext) : unit =
        let properties = new AuthenticationProperties(RedirectUri = this.RedirectUri)
        if this.UserId <> null then
            properties.Dictionary.["XsrfId"] <- this.UserId
        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, this.LoginProvider);

[<Authorize>]
type AccountController() =
    inherit Controller()

    [<DefaultValue>]val mutable _signInManager: ApplicationSignInManager
    [<DefaultValue>]val mutable _userManager: ApplicationUserManager

    member this.SignInManager 
        with public get() = 
            if this._signInManager<> null then 
                this._signInManager 
            else 
                this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>()

        and private set value = 
            this._signInManager <- value

    member this.UserManager 
        with public get() = 
            if this._userManager <> null then 
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
        if userId = null || code = null then
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

            if user = null || (this.UserManager.IsEmailConfirmed(user.Id)|> not) then
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
        (if code = null then this.View("Error") else this.View()) :> ActionResult

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member this.ResetPassword(model:ResetPasswordViewModel) : ActionResult =
        if this.ModelState.IsValid |> not then
            this.View(model) :> ActionResult
        else
            let user = this.UserManager.FindByName(model.Email);
            if user = null then
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

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member this.ExternalLogin(provider:string, returnUrl:string):ActionResult =
        let rvd = new RouteValueDictionary()
        rvd.Add("ReturnUrl",returnUrl)
        new ChallengeResult(provider, this.Url.Action("ExternalLoginCallback", "Account", rvd), null) :> ActionResult

    [<AllowAnonymous>]
    member this.SendCode(returnUrl:string, rememberMe:bool) :ActionResult =
        let userId = this.SignInManager.GetVerifiedUserId()
        if userId = null then
            this.View("Error") :> ActionResult
        else
            let factorOptions = 
                this.UserManager.GetValidTwoFactorProviders(userId)
                |> Seq.map (fun purpose -> new SelectListItem(Text = purpose, Value = purpose));
            this.View(new SendCodeViewModel(Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe)) :> ActionResult

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member this.SendCode(model:SendCodeViewModel):ActionResult =
        if this.ModelState.IsValid |> not then
            this.View() :> ActionResult
        else
            if this.SignInManager.SendTwoFactorCode(model.SelectedProvider) |> not then
                this.View("Error") :> ActionResult
            else
                let rvd = new RouteValueDictionary()
                rvd.Add("Provider",model.SelectedProvider)
                rvd.Add("ReturnUrl",model.ReturnUrl)
                rvd.Add("RememberMe",model.RememberMe)
                this.RedirectToAction("VerifyCode", rvd) :> ActionResult

    member this.AuthenticationManager
        with get() =
            this.HttpContext.GetOwinContext().Authentication

    [<AllowAnonymous>]
    member this.ExternalLoginCallback(returnUrl:string) :ActionResult =
        let loginInfo = this.AuthenticationManager.GetExternalLoginInfo();
        if loginInfo = null then
            this.RedirectToAction("Login") :> ActionResult
        else
            let result = this.SignInManager.ExternalSignIn(loginInfo, isPersistent= false)
            match result with
            | SignInStatus.Success ->
                this.RedirectToLocal(returnUrl)
            | SignInStatus.LockedOut ->
                this.View("Lockout") :> ActionResult
            | SignInStatus.RequiresVerification ->
                let rvd = new RouteValueDictionary()
                rvd.Add("ReturnUrl", returnUrl)
                rvd.Add("RememberMe", false)
                this.RedirectToAction("SendCode", rvd) :> ActionResult
            | _ ->
                this.ViewData.Add("ReturnUrl", returnUrl)
                this.ViewData.Add("LoginProvider", loginInfo.Login.LoginProvider)
                this.View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel(Email = loginInfo.Email)) :> ActionResult

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member this.ExternalLoginConfirmation(model:ExternalLoginConfirmationViewModel, returnUrl:string):ActionResult=
        if this.User.Identity.IsAuthenticated then
            this.RedirectToAction("Index", "Manage") :> ActionResult
        else
            if this.ModelState.IsValid then
                let info = this.AuthenticationManager.GetExternalLoginInfo();
                if info = null then
                    this.View("ExternalLoginFailure") :> ActionResult
                else
                    let user = new ApplicationUser(UserName = model.Email, Email = model.Email)
                    let result = this.UserManager.Create(user)
                    if result.Succeeded then
                        let result = this.UserManager.AddLogin(user.Id, info.Login)
                        if result.Succeeded then
                            this.SignInManager.SignIn(user, isPersistent= false, rememberBrowser= false)
                            this.RedirectToLocal(returnUrl)
                        else
                            this.AddErrors(result);
                            this.ViewData.Add("ReturnUrl",returnUrl)
                            this.View(model) :> ActionResult
                    else
                        this.AddErrors(result);
                        this.ViewData.Add("ReturnUrl",returnUrl)
                        this.View(model) :> ActionResult
            else
                this.ViewData.Add("ReturnUrl",returnUrl)
                this.View(model) :> ActionResult

    [<HttpPost>]
    [<ValidateAntiForgeryToken>]
    member this.LogOff() : ActionResult =
        this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie)
        this.RedirectToAction("Index", "Home") :> ActionResult

    [<AllowAnonymous>]
    member this.ExternalLoginFailure() : ActionResult =
        this.View() :> ActionResult

    override this.Dispose(disposing:bool):unit =
        if disposing then
            if this._userManager <> null then
                this._userManager.Dispose()
                this._userManager <- null

            if this._signInManager <> null then
                this._signInManager.Dispose()
                this._signInManager <- null

        base.Dispose(disposing)

