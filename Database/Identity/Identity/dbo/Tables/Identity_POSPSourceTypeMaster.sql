CREATE TABLE [dbo].[Identity_POSPSourceTypeMaster] (
    [Id]             VARCHAR (100) CONSTRAINT [DF_Identity_POSPSourceTypeMaster_Id] DEFAULT (newid()) NOT NULL,
    [POSPSourceType] VARCHAR (100) NOT NULL,
    [IsActive]       BIT           CONSTRAINT [DF_Identity_POSPSourceTypeMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]      VARCHAR (50)  NULL,
    [CreatedOn]      DATETIME      CONSTRAINT [DF_Identity_POSPSourceTypeMaster_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]      VARCHAR (50)  NULL,
    [UpdatedOn]      DATETIME      NULL,
    CONSTRAINT [PK_POSPSourceTypeMaster_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

