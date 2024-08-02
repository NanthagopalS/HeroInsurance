CREATE TABLE [dbo].[Identity_State] (
    [StateId]   VARCHAR (500) DEFAULT (newid()) NOT NULL,
    [StateName] VARCHAR (100) NULL,
    [CreatedBy] VARCHAR (50)  NULL,
    [CreatedOn] DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50)  NULL,
    [UpdatedOn] DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([StateId] ASC)
);

