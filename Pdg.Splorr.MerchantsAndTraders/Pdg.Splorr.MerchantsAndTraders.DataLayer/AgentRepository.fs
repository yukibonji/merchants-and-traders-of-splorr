namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

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
            join world in context.Dbo.Worlds on (agent.WorldId=world.WorldId)
            where (agent.UserId=userId)
            select ({AgentListItem.AgentId = agent.AgentId; WorldId=agent.WorldId; WorldName = world.WorldName;AgentName=agent.AgentName})
        }

    let fetchOne (agentId:int) (context:MaToSplorrProvider.dataContext) : Agent =
        query{
            for agent in context.Dbo.Agents do
            where (agent.AgentId=agentId)
            select({Agent.AgentId=agent.AgentId;AgentName=agent.AgentName;WorldId=agent.WorldId;UserId=agent.UserId})
        }
        |> Seq.exactlyOne

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
            where (agent.UserId = userId && agent.WorldId = worldId)
            select (agent.AgentId)
        }
        |> Seq.exists(fun e->true)

    let create (agent:Agent) (context:MaToSplorrProvider.dataContext) : Agent =
        let row = context.Dbo.Agents.Create()
        row.AgentName <- agent.AgentName
        row.UserId <- agent.UserId
        row.WorldId <- agent.WorldId
        context.SubmitUpdates()
        context
        |> fetchOne row.AgentId

    let delete (agentId:int)  (context:MaToSplorrProvider.dataContext) : unit =
        let row = context.Dbo.Agents.Single(fun x->x.AgentId = agentId)
        row.Delete()