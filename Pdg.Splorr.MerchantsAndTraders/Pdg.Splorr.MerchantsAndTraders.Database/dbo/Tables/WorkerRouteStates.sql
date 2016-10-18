CREATE TABLE [dbo].[WorkerRouteStates]
(
	[WorkerId] INT NOT NULL, 
    [WorldId] INT NOT NULL, 
    [WorkerStateId] AS 2 PERSISTED, 
    [RouteId] INT NOT NULL,
	CONSTRAINT PK_WorkerRouteStates PRIMARY KEY(WorkerId,WorldId),
	CONSTRAINT FK_WorkerRouteStates_Workers FOREIGN KEY (WorkerId,WorldId,WorkerStateId) REFERENCES Workers(WorkerId,WorldId,WorkerStateId),
	CONSTRAINT FK_WorkerRouteStates_Worlds FOREIGN KEY(WorldId) REFERENCES Worlds(WorldId),
	CONSTRAINT FK_WorkerRouteStates_Sites FOREIGN KEY ([RouteId],WorldId) REFERENCES [Routes](RouteId,WorldId)
)
