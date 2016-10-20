CREATE TABLE [dbo].[Routes]
(
	[RouteId] INT IDENTITY(1,1) NOT NULL,
	[WorldId] INT NOT NULL, 
    [FromSiteId] INT NOT NULL, 
    [ToSiteId] INT NOT NULL, 
    CONSTRAINT PK_Routes PRIMARY KEY ([RouteId]),
    CONSTRAINT UQ_Routes_RouteId_WorldId UNIQUE ([RouteId],[WorldId]),
	CONSTRAINT FK_Routes_Worlds FOREIGN KEY ([WorldId]) REFERENCES Worlds([WorldId]),
	CONSTRAINT FK_Routes_Sites_From FOREIGN KEY([FromSiteId],[WorldId]) REFERENCES Sites([SiteId],[WorldId]),
	CONSTRAINT FK_Routes_Sites_To FOREIGN KEY([ToSiteId],[WorldId]) REFERENCES Sites([SiteId],[WorldId]),
	CONSTRAINT CK_Routes_Site_From_To CHECK (FromSiteId < ToSiteId) -- a strange constraint, no? it ensures that A) I cannot specify the same route twice (like 1->2 and 2->1) and B) that I cannot route from and to the same site

)
