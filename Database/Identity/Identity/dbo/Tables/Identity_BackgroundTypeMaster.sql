CREATE TABLE [dbo].[Identity_BackgroundTypeMaster] (
    [Id]             VARCHAR (100) CONSTRAINT [DF_Identity_BackgroundTypeMaster_Id] DEFAULT (newid()) NOT NULL,
    [BackgroundType] VARCHAR (100) NOT NULL,
    [IsActive]       BIT           CONSTRAINT [DF_Identity_BackgroundTypeMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]      VARCHAR (50)  NULL,
    [CreatedOn]      DATETIME      CONSTRAINT [DF_Identity_BackgroundTypeMaster_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]      VARCHAR (50)  NULL,
    [UpdatedOn]      DATETIME      NULL,
    CONSTRAINT [PK_BackgroundTypeMaster_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

