CREATE TABLE [dbo].[Insurance_NomineeRelation] (
    [NomineeId]   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [NomineeName] VARCHAR (100)    NULL,
    [CreatedBy]   VARCHAR (50)     NULL,
    [CreatedOn]   DATETIME         DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50)     NULL,
    [UpdatedOn]   DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([NomineeId] ASC)
);

