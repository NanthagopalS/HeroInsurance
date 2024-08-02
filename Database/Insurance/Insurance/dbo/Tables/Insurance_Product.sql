CREATE TABLE [dbo].[Insurance_Product] (
    [ProductId]   UNIQUEIDENTIFIER CONSTRAINT [DF__Insurance__Produ__70DDC3D8] DEFAULT (newid()) NOT NULL,
    [ProductName] VARCHAR (100)    NULL,
    [ProductCode] VARCHAR (100)    NULL,
    [InsurerId]   UNIQUEIDENTIFIER NULL,
    [CreatedBy]   VARCHAR (50)     NULL,
    [CreatedOn]   DATETIME         CONSTRAINT [DF__Insurance__Creat__71D1E811] DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50)     NULL,
    [UpdatedOn]   DATETIME         NULL,
    CONSTRAINT [PK__Insuranc__B40CC6CD9D1DA30C] PRIMARY KEY CLUSTERED ([ProductId] ASC)
);

