CREATE TABLE [dbo].[WorkerSiteStates] (
    [WorkerId]      INT NOT NULL,
    [WorldId]       INT NOT NULL,
    [WorkerStateId] AS  ((1)) PERSISTED NOT NULL,
    [SiteId]        INT NOT NULL,
    CONSTRAINT [PK_WorkerSiteStates] PRIMARY KEY CLUSTERED ([WorkerId] ASC, [WorldId] ASC),
    CONSTRAINT [FK_WorkerSiteStates_Sites] FOREIGN KEY ([SiteId], [WorldId]) REFERENCES [dbo].[Sites] ([SiteId], [WorldId]),
    CONSTRAINT [FK_WorkerSiteStates_Workers] FOREIGN KEY ([WorkerId], [WorldId], [WorkerStateId]) REFERENCES [dbo].[Workers] ([WorkerId], [WorldId], [WorkerStateId]),
    CONSTRAINT [FK_WorkerSiteStates_Worlds] FOREIGN KEY ([WorldId]) REFERENCES [dbo].[Worlds] ([WorldId])
);


