CREATE TABLE [dbo].[Insurance_Financier] (
    [FinancierID]   VARCHAR (50)  DEFAULT (newid()) NOT NULL,
    [FinancierName] VARCHAR (100) NULL,
    [CreatedBy]     VARCHAR (50)  NULL,
    [CreatedOn]     DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (50)  NULL,
    [UpdatedOn]     DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([FinancierID] ASC)
);

