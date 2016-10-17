namespace Pdg.Splorr.MerchantsAndTraders.Web.Models

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System

type NewAgentModel() =

    [<Required>]
    [<Display(Name = "New Agent Name")>]
    member val AgentName = String.Empty with get, set

    [<Display(Name = "World")>]
    member val WorldId = 0 with get, set

type EditAgentModel () =

    member val AgentId = 0 with get,set

    [<Required>]
    [<Display(Name = "Agent Name")>]
    member val AgentName = String.Empty with get, set

