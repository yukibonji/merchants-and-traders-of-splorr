namespace Pdg.Splorr.MerchantsAndTraders.BusinessLayer

open Pdg.Splorr.MerchantsAndTraders.DataLayer
open System.Linq
open System.Collections.Generic

open ServiceResult
open Utility

module WorldService =
            
    let private retrieveWorldList (context:MaToSplorrProvider.dataContext) :ServiceListResult<World> =
        context
        |> WorldRepository.fetchList
        |> Success

    let private retrieveAvailableWorldList (userId:string) (context:MaToSplorrProvider.dataContext) : ServiceListResult<World> =
        let currentWorlds =
            context
            |> AgentRepository.fetchList userId
            |> Seq.map(fun e->e.WorldId)
            |> Set.ofSeq
        context
        |> WorldRepository.fetchList
        |> Seq.filter(fun e->currentWorlds.Contains(e.WorldId) |> not)
        |> Success

    let retrieveList (userId:string) : ServiceListResult<World> =
        createContext()
        >>= verifyUserExists userId
        >>= retrieveWorldList

    let retrieveAvailableList (userId:string) : ServiceListResult<World> =
        createContext()
        >>= verifyUserExists userId
        >>= retrieveAvailableWorldList userId