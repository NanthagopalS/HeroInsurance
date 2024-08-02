CREATE TABLE [MOTOR].[GoDigit_StateMaster] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [StateCode] INT           NULL,
    [StateName] VARCHAR (100) NULL,
    [CreatedOn] DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedOn] DATETIME      NULL,
    [StateId]   VARCHAR (50)  NULL
);



