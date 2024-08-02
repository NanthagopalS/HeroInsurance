CREATE TABLE [dbo].[Insurance_PreviousPolicyTypeMapping] (
    [PreviousPolicyTypeId]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PreviousPolicyTypeCode]        VARCHAR (20)     NULL,
    [PreviousPolicyTypeDescription] VARCHAR (100)    NULL,
    [CreatedBy]                     VARCHAR (50)     NULL,
    [CreatedOn]                     DATETIME         DEFAULT (getdate()) NULL,
    [UpdatedBy]                     VARCHAR (50)     NULL,
    [UpdatedOn]                     DATETIME         NULL,
    [ProductId]                     UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([PreviousPolicyTypeId] ASC)
);

