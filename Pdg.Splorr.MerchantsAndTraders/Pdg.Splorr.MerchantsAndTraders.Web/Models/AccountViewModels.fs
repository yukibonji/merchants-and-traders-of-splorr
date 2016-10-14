namespace Pdg.Splorr.MerchantsAndTraders.Web.Models

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System

type LoginViewModel() =

    [<Required>]
    [<Display(Name = "Email")>]
    [<EmailAddress>]
    member val Email = String.Empty with get, set

    [<Required>]
    [<DataType(DataType.Password)>]
    [<Display(Name = "Password")>]
    member val Password = String.Empty with get, set

    [<Display(Name = "Remember me?")>]
    member val RememberMe = false with get, set

type VerifyCodeViewModel() =
    [<Required>]
    member val Provider = String.Empty with get,set

    [<Required>]
    [<Display(Name = "Code")>]
    member val Code = String.Empty with get,set
    member val ReturnUrl = String.Empty with get,set

    [<Display(Name = "Remember this browser?")>]
    member val RememberBrowser = false with get,set

    member val RememberMe = false with get,set

type RegisterViewModel() =
    [<Required>]
    [<EmailAddress>]
    [<Display(Name = "Email")>]
    member val Email = String.Empty with get, set

    [<Required>]
    [<StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)>]
    [<DataType(DataType.Password)>]
    [<Display(Name = "Password")>]
    member val Password = String.Empty with get, set

    [<DataType(DataType.Password)>]
    [<Display(Name = "Confirm password")>]
    [<Compare("Password", ErrorMessage = "The password and confirmation password do not match.")>]
    member val ConfirmPassword = String.Empty with get, set

type ForgotPasswordViewModel() =
    [<Required>]
    [<EmailAddress>]
    [<Display(Name = "Email")>]
    member val Email = String.Empty with get,set

type ResetPasswordViewModel() =
    [<Required>]
    [<EmailAddress>]
    [<Display(Name = "Email")>]
    member val Email = String.Empty with get,set

    [<Required>]
    [<StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)>]
    [<DataType(DataType.Password)>]
    [<Display(Name = "Password")>]
    member val Password = String.Empty with get,set

    [<DataType(DataType.Password)>]
    [<Display(Name = "Confirm password")>]
    [<Compare("Password", ErrorMessage = "The password and confirmation password do not match.")>]
    member val ConfirmPassword = String.Empty with get,set

    member val Code = String.Empty with get,set

type SendCodeViewModel () =
    member val SelectedProvider = String.Empty with get,set
    member val Providers = Seq.empty<System.Web.Mvc.SelectListItem> with get,set
    member val ReturnUrl = String.Empty with get,set
    member val RememberMe = false with get,set

type ExternalLoginConfirmationViewModel() =
    [<Required>]
    [<Display(Name = "Email")>]
    member val Email = String.Empty with get,set

type ExternalLoginListViewModel() =
    member val ReturnUrl = String.Empty with get,set
