namespace Pdg.Splorr.MerchantsAndTraders.BusinessLayer

open Pdg.Splorr.MerchantsAndTraders.DataLayer
open System.Linq
open System.Collections.Generic


open ServiceResult

module internal Utility =
    let internal createContext () :ServiceResult<MaToSplorrProvider.dataContext> =
        Context.create()
        |> Success

    let internal toEnumerable (queryable: IQueryable<'T>) :IEnumerable<'T> =
        queryable.AsEnumerable()

    let internal verifyAgentExists (userId:string) (agentId:int) (context:MaToSplorrProvider.dataContext) : ServiceResult<MaToSplorrProvider.dataContext> =
        if AgentRepository.exists agentId context then
            let agent = AgentRepository.fetchOne agentId context
            if agent.UserId = userId then
                Success context
            else
                Failure ["Agent not found!"]
        else
            Failure ["Agent not found!"]

    let internal verifyUserExists (userId:string) (context:MaToSplorrProvider.dataContext):ServiceResult<MaToSplorrProvider.dataContext> =
        if UserRepository.exists userId context then
            Success context
        else
            Failure ["User Id not found"]
