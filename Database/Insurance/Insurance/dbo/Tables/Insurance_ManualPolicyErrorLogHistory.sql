CREATE TABLE [dbo].[Insurance_ManualPolicyErrorLogHistory] (
    [DumpId]    VARCHAR (100) NULL,
    [ErrorLog]  VARCHAR (MAX) NULL,
    [CreatedOn] VARCHAR (100) NULL,
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [PolicyId]  VARCHAR (100) NULL
);

