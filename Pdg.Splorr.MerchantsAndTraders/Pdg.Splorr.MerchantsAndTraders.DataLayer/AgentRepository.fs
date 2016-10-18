namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

open FSharp.Data.Sql
open System.Linq

type AgentListItem =
    {AgentId:int;
    WorldId:int;
    WorldName:string;
    AgentName:string}

type Agent =
    {AgentId:int;
    AgentName:string;
    UserId:string;
    WorldId:int}

module AgentRepository =
    let fetchList (userId: string) (context:MaToSplorrProvider.dataContext) :IQueryable<AgentListItem> =
        query{
            for agent in context.Dbo.Agents do
            for world in agent.``dbo.Worlds by WorldId`` do
            where (agent.UserId=userId)
            select ({AgentListItem.AgentId = agent.AgentId; WorldId=agent.WorldId; WorldName = world.WorldName;AgentName=agent.AgentName})
        }

    let mapAgent (dbRecord:MaToSplorrProvider.dataContext.``dbo.AgentsEntity``) : Agent =
        { AgentId   = dbRecord.AgentId;
          AgentName = dbRecord.AgentName;
          WorldId   = dbRecord.WorldId;
          UserId    = dbRecord.UserId }

    let fetchOne (agentId:int) (context:MaToSplorrProvider.dataContext) : Agent =
        let result = 
            query{
                for agent in context.Dbo.Agents do
                where (agent.AgentId=agentId)
                select(agent)
                exactlyOne
            }
        result.MapTo<Agent>()

    let exists (agentId:int) (context:MaToSplorrProvider.dataContext) : bool =
        query{
            for agent in context.Dbo.Agents do
            where (agent.AgentId=agentId)
            select(agent.AgentId)
        }
        |> Seq.exists(fun e->true)

    let existsForUserAndWorld (userId:string) (worldId:int) (context:MaToSplorrProvider.dataContext) : bool =
        query{
            for agent in context.Dbo.Agents do
            exists (agent.UserId = userId && agent.WorldId = worldId)
        }

    let create (agent:Agent) (context:MaToSplorrProvider.dataContext) : Agent =
        let row = context.Dbo.Agents.Create()
        row.AgentName <- agent.AgentName
        row.UserId <- agent.UserId
        row.WorldId <- agent.WorldId
        context.SubmitUpdates()
        context
        |> fetchOne row.AgentId

    let delete (agentId:int)  (context:MaToSplorrProvider.dataContext) : unit =
        let row = 
            query{
                for agentEntity in context.Dbo.Agents do
                where (agentEntity.AgentId=agentId)
                select(agentEntity)
            }
            |> Seq.exactlyOne
        row.Delete()
        context.SubmitUpdates()

    let update (agent:Agent) (context:MaToSplorrProvider.dataContext) : Agent =
        let row = 
            query{
                for agentEntity in context.Dbo.Agents do
                where (agentEntity.AgentId=agent.AgentId)
                select(agentEntity)
            }
            |> Seq.exactlyOne
        row.AgentName <- agent.AgentName
        context.SubmitUpdates()
        context
        |> fetchOne agent.AgentId
