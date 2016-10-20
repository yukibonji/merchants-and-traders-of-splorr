namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

open System
open System.Linq

[<RequireQualifiedAccess>]
type WorkerState =
    |Site of int
    |Route of int

type Worker =
    {WorkerId:int;
    WorkerName:string;
    WorldId:int;
    WorkerState:WorkerState}

module WorkerRepository =
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
                dbRecord.``dbo.WorkerRouteStates by WorkerId``.First().RouteId
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
        | WorkerState.Route routeId -> 
            let routeRow = context.Dbo.WorkerRouteStates.Create()
            routeRow.RouteId <- routeId
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