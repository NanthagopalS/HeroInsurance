CREATE TABLE [dbo].[Insurance_ManualPolicyErrorLog] (
    [DumpId]    VARCHAR (100) DEFAULT (newid()) NULL,
    [ErrorLog]  VARCHAR (MAX) NULL,
    [CreatedOn] VARCHAR (100) DEFAULT (getdate()) NULL,
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [PolicyId]  VARCHAR (100) NULL
);

