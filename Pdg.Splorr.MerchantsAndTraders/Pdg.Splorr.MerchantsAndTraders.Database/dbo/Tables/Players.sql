CREATE TABLE [dbo].[Players]
(
	[PlayerId] INT IDENTITY(1,1) NOT NULL,
	[UserId] NVARCHAR(128) NOT NULL, 
    [WorldId] INT NOT NULL, 
    CONSTRAINT [PK_Players] PRIMARY KEY([PlayerId]),
	CONSTRAINT [FK_Players_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]),
	CONSTRAINT [UQ_Players_UserId_WorldId] UNIQUE ([UserId],[WorldId]),
	CONSTRAINT [FK_Players_Worlds] FOREIGN KEY([WorldId]) REFERENCES [dbo].[Worlds]([WorldId]),
	CONSTRAINT [UQ_Players_PlayerId_WorldId] UNIQUE ([PlayerId],[WorldId])
)
