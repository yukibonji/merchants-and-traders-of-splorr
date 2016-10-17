namespace Pdg.Splorr.MerchantsAndTraders.Web.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax
open Pdg.Splorr.MerchantsAndTraders.BusinessLayer
open Microsoft.AspNet.Identity

[<Authorize>]
type HomeController() =
    inherit Controller()

    member this.Index () = 

        let worldListResult =
            (this.User.Identity.GetUserId())
            |> WorldService.retrieveList 

        match worldListResult with
        | ServiceResult.Success worldList ->
            this.View(worldList) :> ActionResult
        | _ ->
            this.View("Error") :> ActionResult

