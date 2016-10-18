CREATE TABLE [dbo].[WorkerSiteStates]
(
	[WorkerId] INT NOT NULL, 
    [WorldId] INT NOT NULL, 
    [WorkerStateId] AS 1 PERSISTED, 
    [SiteId] INT NOT NULL,
	CONSTRAINT PK_WorkerSiteStates PRIMARY KEY(WorkerId,WorldId),
	CONSTRAINT FK_WorkerSiteStates_Workers FOREIGN KEY (WorkerId,WorldId,WorkerStateId) REFERENCES Workers(WorkerId,WorldId,WorkerStateId),
	CONSTRAINT FK_WorkerSiteStates_Worlds FOREIGN KEY(WorldId) REFERENCES Worlds(WorldId),
	CONSTRAINT FK_WorkerSiteStates_Sites FOREIGN KEY (SiteId,WorldId) REFERENCES Sites(SiteId,WorldId)
)
