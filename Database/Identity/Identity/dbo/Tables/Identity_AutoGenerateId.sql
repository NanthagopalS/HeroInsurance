CREATE TABLE [dbo].[Identity_AutoGenerateId] (
    [Id]              VARCHAR (100) CONSTRAINT [DF_Identity_AutoGenerateId_Id] DEFAULT (newid()) NOT NULL,
    [Code]            VARCHAR (100) NULL,
    [CodeDescription] VARCHAR (100) NULL,
    [CodePattern]     VARCHAR (100) NULL,
    [NextValue]       INT           NULL,
    [IsActive]        BIT           CONSTRAINT [DF_Identity_AutoGenerateId_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]       VARCHAR (50)  NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_Identity_AutoGenerateId_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)  NULL,
    [UpdatedOn]       DATETIME      NULL,
    CONSTRAINT [PK_Identity_AutoGenerateId_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

