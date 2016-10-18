namespace Pdg.Splorr.MerchantsAndTraders.BusinessLayer

open Pdg.Splorr.MerchantsAndTraders.DataLayer
open System.Linq

open ServiceResult
open Utility

module AgentService =
    let private retrieveAgentList (userId:string) (context:MaToSplorrProvider.dataContext) :ServiceListResult<AgentListItem> =
        context
        |> AgentRepository.fetchList userId
        |> toEnumerable
        |> Success

    let private verifyAgentCreatable (agent:Agent) (context:MaToSplorrProvider.dataContext) :ServiceResult<MaToSplorrProvider.dataContext> =
        if AgentRepository.existsForUserAndWorld agent.UserId agent.WorldId context |> not then
            Success context
        else
            Failure ["Agent already exists for that user and world!"]

    let private verifyAgentExists (userId:string) (agentId:int) (context:MaToSplorrProvider.dataContext) : ServiceResult<MaToSplorrProvider.dataContext> =
        if AgentRepository.exists agentId context then
            let agent = AgentRepository.fetchOne agentId context
            if agent.UserId = userId then
                Success context
            else
                Failure ["Agent not found!"]
        else
            Failure ["Agent not found!"]

    let private createAgent (agent:Agent) (context:MaToSplorrProvider.dataContext) : ServiceResult<Agent> =
        context
        |> AgentRepository.create agent
        |> Success

    let private removeAgent (agentId:int) (context:MaToSplorrProvider.dataContext) : ServiceResult<unit> =
        context
        |> AgentRepository.delete agentId

        Success ()

    let private updateAgent (agent:Agent) (context:MaToSplorrProvider.dataContext) : ServiceResult<Agent> =
        context
        |> AgentRepository.update agent
        |> Success

    let private retrieveAgent (agentId:int) (context:MaToSplorrProvider.dataContext) : ServiceResult<Agent> =
        context
        |> AgentRepository.fetchOne agentId
        |> Success

    let retrieveList (userId:string) : ServiceListResult<AgentListItem> =
        createContext()
        >>= verifyUserExists userId
        >>= retrieveAgentList userId

    let create (agent:Agent) : ServiceResult<Agent> =
        createContext()
        >>= verifyUserExists agent.UserId
        >>= verifyAgentCreatable agent
        >>= createAgent agent

    let delete (userId:string) (agentId:int) : ServiceResult<unit> =
        createContext()
        >>= verifyUserExists userId
        >>= verifyAgentExists userId agentId
        >>= removeAgent agentId

    let retrieve (userId:string) (agentId: int) : ServiceResult<Agent> =
        createContext()
        >>= verifyUserExists userId
        >>= verifyAgentExists userId agentId
        >>= retrieveAgent agentId

    let update (agent:Agent) : ServiceResult<Agent> =
        createContext()
        >>= verifyUserExists agent.UserId
        >>= verifyAgentExists agent.UserId agent.AgentId
        >>= updateAgent agent
