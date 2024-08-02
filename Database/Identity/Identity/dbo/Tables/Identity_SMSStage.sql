CREATE TABLE [dbo].[Identity_SMSStage] (
    [StageId]   UNIQUEIDENTIFIER CONSTRAINT [DF__Identity___Stage__286302EC] DEFAULT (newid()) NOT NULL,
    [StageName] VARCHAR (80)     NULL,
    [CreatedBy] VARCHAR (50)     NULL,
    [CreatedOn] DATETIME         CONSTRAINT [DF__Identity___Creat__29572725] DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50)     NULL,
    [UpdatedOn] DATETIME         NULL,
    CONSTRAINT [PK__Identity__03EB7AD88B6CA747] PRIMARY KEY CLUSTERED ([StageId] ASC)
);

