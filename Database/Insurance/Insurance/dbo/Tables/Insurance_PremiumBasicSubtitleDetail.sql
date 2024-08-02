CREATE TABLE [dbo].[Insurance_PremiumBasicSubtitleDetail] (
    [Id]                   VARCHAR (50)   DEFAULT (newid()) NULL,
    [PremiumBasicDetailId] VARCHAR (50)   NULL,
    [Subtitle]             NVARCHAR (MAX) NULL,
    [Description]          NVARCHAR (MAX) NULL,
    [Icon]                 NVARCHAR (MAX) NULL,
    [IsActive]             BIT            DEFAULT ((1)) NULL,
    [CreatedBy]            VARCHAR (100)  NULL,
    [CreatedOn]            DATETIME       CONSTRAINT [DF_Insurance_PremiumBasicSubtitleDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]            VARCHAR (100)  NULL,
    [UpdatedOn]            DATETIME       NULL,
    [InsurerId]            VARCHAR (50)   NULL
);

