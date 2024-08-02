CREATE TABLE [dbo].[Identity_ErrorDetail] (
    [SrNo]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [StrProcedure_Name] VARCHAR (500)  NULL,
    [ErrorDetail]       VARCHAR (1000) NULL,
    [ParameterList]     VARCHAR (2000) NULL,
    [ErrorDate]         DATETIME       DEFAULT (getdate()) NOT NULL
);

