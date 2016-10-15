namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

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
    let fetchList (context:MaToSplorrProvider.dataContext) (userId: string) =
        query{
            for agent in context.Dbo.Agents do
            join world in context.Dbo.Worlds on (agent.WorldId=world.WorldId)
            where (agent.UserId=userId)
            select ({AgentListItem.AgentId = agent.AgentId; WorldId=agent.WorldId; WorldName = world.WorldName;AgentName=agent.AgentName})
        }

    let fetchOne (context:MaToSplorrProvider.dataContext) (agentId:int) : Agent =
        query{
            for agent in context.Dbo.Agents do
            where (agent.AgentId=agentId)
            select({Agent.AgentId=agent.AgentId;AgentName=agent.AgentName;WorldId=agent.WorldId;UserId=agent.UserId})
        }
        |> Seq.exactlyOne

    let create (context:MaToSplorrProvider.dataContext) (agent:Agent) : Agent =
        let row = context.Dbo.Agents.Create()
        row.AgentName <- agent.AgentName
        row.UserId <- agent.UserId
        row.WorldId <- agent.WorldId
        context.SubmitUpdates()
        {agent with AgentId=row.AgentId}
