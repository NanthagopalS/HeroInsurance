CREATE TABLE [dbo].[Identity_DocumentTypeMaster] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_Identity_DocumentTypeMaster_Id] DEFAULT (newid()) NOT NULL,
    [DocumentType]     VARCHAR (100)    NOT NULL,
    [IsMandatory]      BIT              NULL,
    [IsActive]         BIT              CONSTRAINT [DF_Identity_DocumentTypeMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]        VARCHAR (50)     NULL,
    [CreatedOn]        DATETIME         CONSTRAINT [DF_Identity_DocumentTypeMaster_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]        VARCHAR (50)     NULL,
    [UpdatedOn]        DATETIME         NULL,
    [ShortDescription] VARCHAR (255)    NULL,
    [DocumentTypeId]   INT              NULL,
    CONSTRAINT [PK_Identity_DocumentTypeMaster_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

