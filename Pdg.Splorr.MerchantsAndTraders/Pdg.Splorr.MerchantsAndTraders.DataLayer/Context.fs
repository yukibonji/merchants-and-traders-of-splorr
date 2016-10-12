namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

open FSharp.Data.Sql
open System

module Constants =
    let [<Literal>] connectionString="Initial Catalog=MerchantsAndTraders;Data Source=(local);Integrated Security=True"
    let [<Literal>] resolutionPath = __SOURCE_DIRECTORY__

type MaToSplorrProvider = 
    SqlDataProvider<
        Common.DatabaseProviderTypes.MSSQLSERVER,
        Constants.connectionString,
        ResolutionPath = Constants.resolutionPath,
        UseOptionTypes = true>

module Context =
    let create () =
        MaToSplorrProvider.GetDataContext();

type WorldListItem =
    {WorldId:int;
    WorldName:string;
    CreatedOn:DateTimeOffset}

module Worlds =
    let fetch (context:MaToSplorrProvider.dataContext) =
        query{
            for world in context.Dbo.Worlds do
            select ({WorldListItem.WorldId=world.WorldId;WorldName=world.WorldName;CreatedOn=world.CreatedOn})
        }
