namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

open System
open System.Linq

type Route =
    {RouteId:int;
    WorldId:int;
    FromSiteId:int;
    ToSiteId:int}

module RouteRepository =
    let fetchOne (routeId:int) (context:MaToSplorrProvider.dataContext) : Route =
        let result =
            query {
                for route in context.Dbo.Routes do
                where (route.RouteId = routeId)
                select(route)
                exactlyOne
            }
        result.MapTo<Route>()

    let create (route:Route) (context:MaToSplorrProvider.dataContext) : Route =
        let row = context.Dbo.Routes.Create()

        row.WorldId <- route.WorldId
        row.FromSiteId <- route.FromSiteId
        row.ToSiteId <- route.ToSiteId

        context.SubmitUpdates()

        context
        |> fetchOne row.RouteId
        
