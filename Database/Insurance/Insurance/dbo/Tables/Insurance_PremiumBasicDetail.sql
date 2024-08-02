CREATE TABLE [dbo].[Insurance_PremiumBasicDetail] (
    [Id]        VARCHAR (50)   CONSTRAINT [DF__Insurance__Premi__5A846E65] DEFAULT (newid()) NOT NULL,
    [Title]     NVARCHAR (MAX) NULL,
    [IsActive]  BIT            CONSTRAINT [DF__Insurance__IsAct__5B78929E] DEFAULT ((1)) NULL,
    [CreatedBy] VARCHAR (100)  NULL,
    [CreatedOn] DATETIME       CONSTRAINT [DF_Insurance_PremiumBasicDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (100)  NULL,
    [UpdatedOn] DATETIME       NULL,
    [InsurerId] VARCHAR (50)   NULL
);

