namespace Pdg.Splorr.MerchantsAndTraders.Web.Models

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System

type ConfirmDeleteModel() =

    [<Required>]
    [<Display(Name = "Are you sure?")>]
    member val Confirm = false with get, set

    member val Id = 0 with get, set
