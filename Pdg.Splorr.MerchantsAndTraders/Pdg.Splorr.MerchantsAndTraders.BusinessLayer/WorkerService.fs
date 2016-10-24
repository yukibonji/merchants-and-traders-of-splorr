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
        
    let private retrieveAgentWorldId (agentId:int) (context:MaToSplorrProvider.dataContext) : ServiceResult<int * MaToSplorrProvider.dataContext> = //world id
        let agent = 
            context
            |> AgentRepository.fetchOne agentId 
        (agent.WorldId, context)
        |> ServiceResult.Success

    let private selectStartingSite (worldId:int, context:MaToSplorrProvider.dataContext) : ServiceResult<int * int * MaToSplorrProvider.dataContext> = //world id, site id
        let random = new System.Random()
        let selectedSite = 
            context
            |> SiteRepository.fetchForWorld worldId
            |> Seq.sortBy (fun x->random.Next())
            |> Seq.head
        (worldId, selectedSite.SiteId, context)
        |> ServiceResult.Success

    let private createWorkerAtSite (workerName:string) (worldId:int, siteId:int, context:MaToSplorrProvider.dataContext) : ServiceResult<Worker * MaToSplorrProvider.dataContext> =
        let worker = 
            ({Worker.WorkerId=0;
            WorkerName = workerName;
            WorldId = worldId;
            WorkerState = WorkerState.Site siteId}, context)
            ||> WorkerRepository.create

        (worker, context)
        |> ServiceResult.Success

    let private attachWorkerToAgent (agentId:int) (worker:Worker, context:MaToSplorrProvider.dataContext) : ServiceResult<Worker> =
        context
        |> WorkerRepository.setAgent (agentId |> Some) (worker.WorkerId)

        worker
        |> ServiceResult.Success

    let retrieveListForAgent (userId:string) (agentId:int) : ServiceListResult<AgentWorker> =
        createContext()
        >>= verifyUserExists userId
        >>= verifyAgentExists userId agentId
        >>= retrieveAgentWorkers agentId

    let createWorkerForAgent (userId:string) (agentId:int) (workerName:string) : ServiceResult<Worker> =
        createContext()
        >>= verifyUserExists userId
        >>= verifyAgentExists userId agentId
        >>= retrieveAgentWorldId agentId
        >>= selectStartingSite
        >>= createWorkerAtSite workerName
        >>= attachWorkerToAgent agentId

