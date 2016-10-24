namespace Pdg.Splorr.MerchantsAndTraders.BusinessLayer

open Pdg.Splorr.MerchantsAndTraders.DataLayer
open System.Linq

open ServiceResult
open Utility

module WorkerService =
    let private retrieveAgentWorkers (agentId:int) (context:MaToSplorrProvider.dataContext) : ServiceListResult<AgentWorker> =
        context
        |> WorkerRepository.fetchForAgent agentId
        |> ServiceResult.Success
        

    let retrieveListForAgent (userId:string) (agentId:int) : ServiceListResult<AgentWorker> =
        createContext()
        >>= verifyUserExists userId
        >>= verifyAgentExists userId agentId
        >>= retrieveAgentWorkers agentId
