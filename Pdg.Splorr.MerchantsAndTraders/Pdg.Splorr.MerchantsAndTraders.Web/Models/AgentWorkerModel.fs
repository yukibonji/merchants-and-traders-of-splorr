namespace Pdg.Splorr.MerchantsAndTraders.Web.Models

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System

type AgentWorkerModel() =
    member val AgentId = 0 with get,set
    member val WorkerId = 0 with get,set
    member val WorkerName = String.Empty with get,set
    member val Location = String.Empty with get,set
