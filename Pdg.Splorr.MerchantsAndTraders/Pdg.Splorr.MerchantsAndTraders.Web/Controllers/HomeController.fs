namespace Pdg.Splorr.MerchantsAndTraders.Web.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax
open Pdg.Splorr.MerchantsAndTraders.DataLayer

type HomeController() =
    inherit Controller()

    member this.Index () = 
        let ctx = 
            Context.create()

        let worldList =
            ctx
            |> Worlds.fetch

        this.View(worldList.AsEnumerable())

