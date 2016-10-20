CREATE TABLE [dbo].[Workers]
(
	[WorkerId] INT IDENTITY(1,1) NOT NULL, 
    [WorkerName] NVARCHAR(50) NOT NULL,
    [WorldId] INT NOT NULL,
	[WorkerStateId] INT NOT NULL, 
    CONSTRAINT PK_Workers PRIMARY KEY ([WorkerId]),
    CONSTRAINT UQ_Workers_WorkerId_WorldId UNIQUE ([WorkerId],[WorldId]),
    CONSTRAINT UQ_Workers_WorkerId_WorldId_WorkerStateId UNIQUE ([WorkerId],[WorldId], WorkerStateId),
	CONSTRAINT FK_Workers_Worlds FOREIGN KEY ([WorldId]) REFERENCES Worlds([WorldId]), 
    CONSTRAINT [FK_Workers_WorkerStates] FOREIGN KEY (WorkerStateId) REFERENCES WorkerStates(WorkerStateId)
)
