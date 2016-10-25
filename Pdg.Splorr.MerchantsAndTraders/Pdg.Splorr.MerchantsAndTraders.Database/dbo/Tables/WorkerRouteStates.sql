CREATE TABLE [dbo].[WorkerRouteStates] (
    [WorkerId]      INT                NOT NULL,
    [WorldId]       INT                NOT NULL,
    [WorkerStateId] AS                 ((2)) PERSISTED NOT NULL,
    [RouteId]       INT                NOT NULL,
    [Reversed]      BIT                NOT NULL,
    [CompletesOn]   DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_WorkerRouteStates] PRIMARY KEY CLUSTERED ([WorkerId] ASC),
    CONSTRAINT [UQ_WorkerRouteStates_WorkerId_WorldId] UNIQUE ([WorkerId] ASC, [WorldId] ASC),
    CONSTRAINT [FK_WorkerRouteStates_Sites] FOREIGN KEY ([RouteId], [WorldId]) REFERENCES [dbo].[Routes] ([RouteId], [WorldId]),
    CONSTRAINT [FK_WorkerRouteStates_Workers] FOREIGN KEY ([WorkerId], [WorldId], [WorkerStateId]) REFERENCES [dbo].[Workers] ([WorkerId], [WorldId], [WorkerStateId]),
    CONSTRAINT [FK_WorkerRouteStates_Worlds] FOREIGN KEY ([WorldId]) REFERENCES [dbo].[Worlds] ([WorldId])
);


