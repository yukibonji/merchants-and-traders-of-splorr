CREATE TABLE [dbo].[Routes]
(
	[RouteId] INT IDENTITY(1,1) NOT NULL,
	[WorldId] INT NOT NULL, 
    [FromSiteId] INT NOT NULL, 
    [ToSiteId] INT NOT NULL, 
    CONSTRAINT PK_Routes PRIMARY KEY ([RouteId],[WorldId]),
	CONSTRAINT FK_Routes_Worlds FOREIGN KEY ([WorldId]) REFERENCES Worlds([WorldId]),
	CONSTRAINT FK_Routes_Sites_From FOREIGN KEY([FromSiteId],[WorldId]) REFERENCES Sites([SiteId],[WorldId]),
	CONSTRAINT FK_Routes_Sites_To FOREIGN KEY([ToSiteId],[WorldId]) REFERENCES Sites([SiteId],[WorldId])

)
