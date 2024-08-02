CREATE TABLE [dbo].[Insurance_RTOCode] (
    [RTOCodeId] UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [RTOCode]   VARCHAR (80)     NULL,
    [RTOId]     VARCHAR (50)     NULL,
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [CreatedOn] DATETIME         DEFAULT (getdate()) NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    [UpdatedOn] DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([RTOCodeId] ASC)
);

