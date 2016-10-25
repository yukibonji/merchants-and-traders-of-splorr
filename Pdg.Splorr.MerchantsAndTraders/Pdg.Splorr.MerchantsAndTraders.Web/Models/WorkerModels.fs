namespace Pdg.Splorr.MerchantsAndTraders.Web.Models

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System

type NewWorkerModel() =

    [<Required>]
    [<Display(Name = "New Worker Name")>]
    member val WorkerName = String.Empty with get, set

    member val AgentId = 0 with get, set

type AtSiteModel() =
    member val SiteId = 0 with get,set
    member val SiteName = 0 with get,set

type OnRouteModel() =
    member val From = AtSiteModel() with get,set
    member val To = AtSiteModel() with get,set
    member val CompletesOn = DateTimeOffset.MinValue with get,set

type AgentWorkerDisplayModel() =
    member val AgentId = 0 with get, set
    member val WorkerId = 0 with get, set
    member val WorkerName = String.Empty with get, set
    member val AtSite = AtSiteModel() with get, set
    member val OnRoute = OnRouteModel() with get, set
