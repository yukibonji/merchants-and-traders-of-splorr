CREATE TABLE [dbo].[Sites]
(
	[SiteId] INT IDENTITY(1,1) NOT NULL,
	[SiteName] NVARCHAR(50) NOT NULL, 
    [SiteX] FLOAT NOT NULL, 
    [SiteY] FLOAT NOT NULL, 
	[WorldId] INT NOT NULL,
	CONSTRAINT PK_Sites PRIMARY KEY([SiteId],[WorldId]),
	CONSTRAINT FK_Sites_Worlds FOREIGN KEY ([WorldId]) REFERENCES Worlds(WorldId),
	CONSTRAINT UQ_Sites_WorldId_SiteName UNIQUE([WorldId],[SiteName]),
	CONSTRAINT UQ_Sites_WorldId_SiteX_SiteY UNIQUE([WorldId],[SiteX],[SiteY])
)
