CREATE TABLE [MOTOR].[ITGI_FinancierMaster] (
    [Code]      NVARCHAR (255) NULL,
    [Financier] NVARCHAR (255) NULL,
    [CreatedOn] DATETIME       DEFAULT (getdate()) NULL,
    [CreatedBy] VARCHAR (50)   DEFAULT ((1)) NULL,
    [UpdatedOn] DATETIME       NULL,
    [UpdatedBy] VARCHAR (50)   NULL
);

