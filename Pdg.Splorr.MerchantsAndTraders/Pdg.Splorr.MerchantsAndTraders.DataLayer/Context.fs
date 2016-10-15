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

type AgentListItem =
    {AgentId:int;
    WorldId:int;
    WorldName:string;
    AgentName:string}

module Agents =
    let fetchList (context:MaToSplorrProvider.dataContext) (userId: string) =
        query{
            for agent in context.Dbo.Agents do
                join world in context.Dbo.Worlds on (agent.WorldId=world.WorldId)
                where (agent.UserId=userId)
                select ({AgentListItem.AgentId = agent.AgentId; WorldId=agent.WorldId; WorldName = world.WorldName;AgentName=agent.AgentName})
        }