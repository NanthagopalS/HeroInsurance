CREATE TABLE [dbo].[Insurance_SubProduct] (
    [SubProductId]          UNIQUEIDENTIFIER CONSTRAINT [DF__Insurance__SubPr__797309D9] DEFAULT (newid()) NOT NULL,
    [SubProductName]        VARCHAR (100)    NULL,
    [SubProductCode]        VARCHAR (100)    NULL,
    [ProductId]             VARCHAR (50)     NULL,
    [SubProductDescription] VARCHAR (100)    NULL,
    [IsNewVehicle]          BIT              NULL,
    [CreatedBy]             VARCHAR (50)     NULL,
    [CreatedOn]             DATETIME         CONSTRAINT [DF__Insurance__Creat__7A672E12] DEFAULT (getdate()) NULL,
    [UpdatedBy]             VARCHAR (50)     NULL,
    [UpdatedOn]             DATETIME         NULL,
    CONSTRAINT [PK__Insuranc__65C918A5BD365966] PRIMARY KEY CLUSTERED ([SubProductId] ASC)
);

