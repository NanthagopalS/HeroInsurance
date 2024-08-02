CREATE TABLE [dbo].[Identity_BenefitsDetail] (
    [Id]                  VARCHAR (100) CONSTRAINT [DF_Identity_BenefitsDetail_Id] DEFAULT (newid()) NOT NULL,
    [BenefitsTitle]       VARCHAR (200) NOT NULL,
    [BenefitsDescription] VARCHAR (500) NULL,
    [IsActive]            BIT           CONSTRAINT [DF_Identity_BenefitsDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedOn]           DATETIME      CONSTRAINT [DF_Identity_BenefitsDetail_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]           VARCHAR (50)  NULL,
    [UpdatedBy]           VARCHAR (50)  NULL,
    [UpdatedOn]           DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

