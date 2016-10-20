CREATE TABLE [dbo].[Agents]
(
	[AgentId] INT IDENTITY(1,1) NOT NULL,
	[UserId] NVARCHAR(128) NOT NULL, 
    [WorldId] INT NOT NULL, 
    [CreatedOn] DATETIMEOFFSET NOT NULL CONSTRAINT DF_Agents_CreatedOn DEFAULT SYSDATETIMEOFFSET(), 
    [AgentName] NVARCHAR(50) NOT NULL, 
	CONSTRAINT [PK_Agents] PRIMARY KEY ([AgentId]),
	CONSTRAINT [UQ_Agents_AgentId_WorldId] UNIQUE ([AgentId],[WorldId]),
	CONSTRAINT [FK_Agents_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]),
	CONSTRAINT [UQ_Agents_UserId_WorldId] UNIQUE ([UserId],[WorldId]),
	CONSTRAINT [FK_Agents_Worlds] FOREIGN KEY([WorldId]) REFERENCES [dbo].[Worlds]([WorldId]),
	CONSTRAINT [UQ_Agents_PlayerName_WorldId] UNIQUE ([AgentName],[WorldId])
)
