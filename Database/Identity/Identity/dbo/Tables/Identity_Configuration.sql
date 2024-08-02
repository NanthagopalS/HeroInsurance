CREATE TABLE [dbo].[Identity_Configuration] (
    [Id]                 VARCHAR (100) CONSTRAINT [DF_Identity_Configuration_Id] DEFAULT (newid()) NOT NULL,
    [ConfigurationKey]   VARCHAR (500) NULL,
    [ConfigurationValue] VARCHAR (MAX) NULL,
    [ConfigurationLable] VARCHAR (500) NULL,
    [ConfigurationIcon]  VARCHAR (500) NULL,
    [IsActive]           BIT           CONSTRAINT [DF_Identity_Configuration_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]          VARCHAR (50)  NULL,
    [CreatedOn]          DATETIME      CONSTRAINT [DF_Identity_Configuration_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]          VARCHAR (50)  NULL,
    [UpdatedOn]          DATETIME      NULL,
    CONSTRAINT [PK_Identity_Configuration_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

