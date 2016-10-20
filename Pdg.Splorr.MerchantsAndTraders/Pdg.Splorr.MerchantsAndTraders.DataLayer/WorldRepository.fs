namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

open System
open System.Linq

type World =
    {WorldId:int;
    WorldName:string;
    CreatedOn:DateTimeOffset}

module WorldRepository =
    let fetchList (context:MaToSplorrProvider.dataContext) : seq<World> =
        query{
            for world in context.Dbo.Worlds do
            select (world)
        }
        |> Seq.map(fun x->x.MapTo<World>())

    let fetchOne (worldId:int) (context:MaToSplorrProvider.dataContext) : World =
        let result = 
            query{
                for world in context.Dbo.Worlds do
                where (world.WorldId=worldId)
                select (world)
                exactlyOne
            }
        result.MapTo<World>()

    let create (world:World) (context:MaToSplorrProvider.dataContext) : World =
        let row = context.Dbo.Worlds.Create()

        row.WorldName <- world.WorldName
        row.CreatedOn <- DateTimeOffset.UtcNow

        context.SubmitUpdates()

        context
        |> fetchOne row.WorldId
