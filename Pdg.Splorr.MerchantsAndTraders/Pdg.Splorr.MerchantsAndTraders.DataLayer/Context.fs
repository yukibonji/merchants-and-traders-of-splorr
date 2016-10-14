namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

open FSharp.Data.Sql
open System

module Constants =
    let [<Literal>] connectionString="Initial Catalog=MerchantsAndTraders;Data Source=(local);Integrated Security=True"

type MaToSplorrProvider = 
    SqlDataProvider<
        Common.DatabaseProviderTypes.MSSQLSERVER,
        Constants.connectionString,
        UseOptionTypes = true>

module Context =
    let create () =
        MaToSplorrProvider.GetDataContext();

type WorldListItem =
    {WorldId:int;
    WorldName:string;
    CreatedOn:DateTimeOffset}

module Worlds =
    let fetchList (context:MaToSplorrProvider.dataContext) =
        query{
            for world in context.Dbo.Worlds do
            select ({WorldListItem.WorldId=world.WorldId;WorldName=world.WorldName;CreatedOn=world.CreatedOn})
        }

type PlayerListItem =
    {PlayerId:int;
    WorldId:int;
    WorldName:string}

module Players =
    let fetchList (context:MaToSplorrProvider.dataContext) (userId: string) =
        query{
            for player in context.Dbo.Players do
                join world in context.Dbo.Worlds on (player.WorldId=world.WorldId)
                where (player.UserId=userId)
                select ({PlayerListItem.PlayerId = player.PlayerId; WorldId=player.WorldId; WorldName = world.WorldName})
        }