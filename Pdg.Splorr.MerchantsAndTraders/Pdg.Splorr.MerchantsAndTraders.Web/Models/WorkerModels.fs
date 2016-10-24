namespace Pdg.Splorr.MerchantsAndTraders.Web.Models

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System

type NewWorkerModel() =

    [<Required>]
    [<Display(Name = "New Worker Name")>]
    member val WorkerName = String.Empty with get, set

    member val AgentId = 0 with get, set
