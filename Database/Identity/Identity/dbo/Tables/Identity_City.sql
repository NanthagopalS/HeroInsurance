CREATE TABLE [dbo].[Identity_City] (
    [CityId]    VARCHAR (500) DEFAULT (newid()) NOT NULL,
    [CityName]  VARCHAR (100) NULL,
    [StateId]   VARCHAR (50)  NULL,
    [CreatedBy] VARCHAR (50)  NULL,
    [CreatedOn] DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50)  NULL,
    [UpdatedOn] DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([CityId] ASC)
);

