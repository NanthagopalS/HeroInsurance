CREATE TABLE [dbo].[Insurance_Documents] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [InsurerId]    VARCHAR (500)  NULL,
    [DocumentName] VARCHAR (500)  NULL,
    [Code]         VARCHAR (500)  NULL,
    [IsActive]     BIT            NULL,
    [CreatedBy]    INT            NULL,
    [CreatedOn]    DATETIME       NULL,
    [UpdatedBy]    INT            NULL,
    [UpdatedOn]    DATETIME       NULL,
    [DocumentCode] VARCHAR (50)   NULL,
    [Validation]   NVARCHAR (MAX) NULL
);



