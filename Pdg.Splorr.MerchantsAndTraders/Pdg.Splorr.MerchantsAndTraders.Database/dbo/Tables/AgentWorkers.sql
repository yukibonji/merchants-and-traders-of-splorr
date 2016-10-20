CREATE TABLE [dbo].[AgentWorkers]
(
	[WorkerId] INT NOT NULL,
	[WorldId] INT NOT NULL, 
    [AgentId] INT NOT NULL, 
    CONSTRAINT PK_AgentWorkers PRIMARY KEY(WorkerId),
	CONSTRAINT FK_AgentWorkers_Workers FOREIGN KEY (WorkerId, WorldId) REFERENCES Workers(WorkerId, WorldId),
	CONSTRAINT FK_AgentWorkers_Agents FOREIGN KEY (AgentId, WorldId) REFERENCES Agents(AgentId, WorldId),
	CONSTRAINT FK_AgentWorkers_Worlds FOREIGN KEY (WorldId) REFERENCES Worlds(WorldId)
)
