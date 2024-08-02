CREATE TABLE [dbo].[Insurance_Year] (
    [YearId]    UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [CreatedOn] DATETIME         DEFAULT (getdate()) NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    [UpdatedOn] DATETIME         NULL,
    [Year]      VARCHAR (4)      NULL,
    PRIMARY KEY CLUSTERED ([YearId] ASC)
);

