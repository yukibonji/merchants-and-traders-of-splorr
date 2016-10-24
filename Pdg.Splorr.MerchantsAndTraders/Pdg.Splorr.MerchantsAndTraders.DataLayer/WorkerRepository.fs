namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

open System
open System.Linq

type WorkerRouteState =
    {RouteId:int;
    Reversed:bool;
    CompletesOn:DateTimeOffset}

[<RequireQualifiedAccess>]
type WorkerState =
    |Site of int
    |Route of WorkerRouteState

type Worker =
    {WorkerId:int;
    WorkerName:string;
    WorldId:int;
    WorkerState:WorkerState}

type SiteDisplayState =
    {SiteId:int;
    SiteName:string}

type OnRouteDisplayState =
    {From: SiteDisplayState;
    To:SiteDisplayState;
    CompletesOn:DateTimeOffset}

type WorkerDisplayState =
    | AtSite of SiteDisplayState
    | OnRoute of OnRouteDisplayState

type AgentWorker =
    {AgentId:int;
    WorkerId:int;
    WorkerName:string;
    DisplayState: WorkerDisplayState}
    

module WorkerRepository =
    let fetchAgentId (workerId: int) (context:MaToSplorrProvider.dataContext) : int option =
        let result =
            query{
                for agentWorker in context.Dbo.AgentWorkers do
                where (agentWorker.WorkerId=workerId)
                select (agentWorker.AgentId)
                exactlyOneOrDefault
            }
        if result=0 then None else Some result

    let mapRouteState (dbRecord:MaToSplorrProvider.dataContext.``dbo.WorkerRouteStatesEntity``) :WorkerRouteState =
        {RouteId = dbRecord.RouteId;
        Reversed = dbRecord.Reversed;
        CompletesOn = dbRecord.CompletesOn}

    let mapWorker (dbRecord:MaToSplorrProvider.dataContext.``dbo.WorkersEntity``) : Worker =
        { WorkerId = dbRecord.WorkerId;
          WorkerName = dbRecord.WorkerName;
          WorldId = dbRecord.WorldId;
          WorkerState = 
            match dbRecord.WorkerStateId with
            | 1 ->
                dbRecord.``dbo.WorkerSiteStates by WorkerId``.First().SiteId
                |> WorkerState.Site
            | _ -> //really just 2 at this point
                dbRecord.``dbo.WorkerRouteStates by WorkerId``.First()
                |> mapRouteState
                |> WorkerState.Route }

    let fetchOne (workerId:int) (context:MaToSplorrProvider.dataContext) : Worker =
        query{
            for worker in context.Dbo.Workers do
            where (worker.WorkerId=workerId)
            select (worker)
            exactlyOne
        }
        |> mapWorker

    let private createWorkerState (worker:Worker) (context:MaToSplorrProvider.dataContext) : unit =
        match worker.WorkerState with
        | WorkerState.Site siteId ->
            let siteRow = context.Dbo.WorkerSiteStates.Create()
            siteRow.SiteId <- siteId
            siteRow.WorkerId <- worker.WorkerId
            context.SubmitUpdates()
        | WorkerState.Route routeState -> 
            let routeRow = context.Dbo.WorkerRouteStates.Create()
            routeRow.RouteId <- routeState.RouteId
            routeRow.Reversed <- routeState.Reversed
            routeRow.CompletesOn <- routeState.CompletesOn
            routeRow.WorkerId <- worker.WorkerId
            context.SubmitUpdates()

    let create (worker:Worker) (context:MaToSplorrProvider.dataContext) : Worker =
        let workerRow = context.Dbo.Workers.Create()
        workerRow.WorkerName <- worker.WorkerName
        workerRow.WorldId <- worker.WorldId
        workerRow.WorkerStateId <- 
            match worker.WorkerState with 
            | WorkerState.Site _ -> 1 
            | WorkerState.Route _ -> 2
        context.SubmitUpdates()

        context
        |> createWorkerState {worker with WorkerId = workerRow.WorkerId}

        context
        |> fetchOne workerRow.WorkerId

    let private mapAgentWorkerListItem (agentWorker:MaToSplorrProvider.dataContext.``dbo.AgentWorkerListItemsEntity``) : AgentWorker =
        {AgentWorker.AgentId = agentWorker.AgentId;
        WorkerId = agentWorker.WorkerId;
        WorkerName = agentWorker.WorkerName;
        DisplayState = 
            match agentWorker.WorkerStateId with
            | 1 ->
                {SiteDisplayState.SiteId = agentWorker.AtSiteId.Value; SiteName = agentWorker.AtSiteName.Value} |> AtSite
            | _ -> 
                {OnRouteDisplayState.From = {SiteDisplayState.SiteId = agentWorker.FromSiteId.Value; SiteName = agentWorker.FromSiteName.Value}; To= {SiteDisplayState.SiteId = agentWorker.ToSiteId.Value; SiteName = agentWorker.ToSiteName.Value}; CompletesOn = agentWorker.CompletesOn.Value} |> OnRoute}

    let fetchForAgent (agentId:int) (context:MaToSplorrProvider.dataContext) : seq<AgentWorker> =
        query{
            for agentWorker in context.Dbo.AgentWorkerListItems do
            where (agentWorker.AgentId = agentId)
            select (agentWorker)
        }        
        |> Seq.map (mapAgentWorkerListItem)

    let setAgent (agentId:int option) (workerId:int) (context:MaToSplorrProvider.dataContext) : unit =
        let agentWorkerExists = 
            query{
                for agentWorker in context.Dbo.AgentWorkers do
                exists (agentWorker.WorkerId = workerId)
            }
        match agentWorkerExists, agentId with
        | false, Some id ->
            let worker = context |> fetchOne workerId
            let row = context.Dbo.AgentWorkers.Create()
            row.AgentId <- id
            row.WorkerId <- workerId
            row.WorldId <- worker.WorldId
            context.SubmitUpdates()
        | true, None ->
            let row = 
                query {
                    for agentWorker in context.Dbo.AgentWorkers do 
                    where (agentWorker.WorkerId = workerId)
                    select (agentWorker)
                    exactlyOne}
            row.Delete()
            context.SubmitUpdates()
        | true, Some id ->
            let row = 
                query {
                    for agentWorker in context.Dbo.AgentWorkers do 
                    where (agentWorker.WorkerId = workerId)
                    select (agentWorker)
                    exactlyOne}
            if row.AgentId <> id then
                row.AgentId <- id
                context.SubmitUpdates()
        | _ -> ()
