namespace Pdg.Splorr.MerchantsAndTraders.Web.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Routing
open System.Web.Mvc.Ajax
open Pdg.Splorr.MerchantsAndTraders.BusinessLayer
open Pdg.Splorr.MerchantsAndTraders.DataLayer
open Microsoft.AspNet.Identity
open Pdg.Splorr.MerchantsAndTraders.Web.Models

[<Authorize>]
type WorkerController() =
    inherit Controller()

    member this.Detail (id:int) : ActionResult =
        this.View() :> ActionResult
