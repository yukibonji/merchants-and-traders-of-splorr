namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

open System

type WorldListItem =
    {WorldId:int;
    WorldName:string;
    CreatedOn:DateTimeOffset}

module WorldRepository =
    let fetchList (context:MaToSplorrProvider.dataContext) =
        query{
            for world in context.Dbo.Worlds do
            select ({WorldListItem.WorldId=world.WorldId;WorldName=world.WorldName;CreatedOn=world.CreatedOn})
        }
