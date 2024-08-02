CREATE TABLE [dbo].[Insurance_Insurer] (
    [InsurerId]              UNIQUEIDENTIFIER CONSTRAINT [DF__Insurance__Insur__300424B4] DEFAULT (newid()) NOT NULL,
    [InsurerName]            VARCHAR (100)    NULL,
    [InsurerCode]            VARCHAR (20)     NULL,
    [CreatedBy]              VARCHAR (50)     NULL,
    [CreatedOn]              DATETIME         CONSTRAINT [DF__Insurance__Creat__30F848ED] DEFAULT (getdate()) NULL,
    [UpdatedBy]              VARCHAR (50)     NULL,
    [UpdatedOn]              DATETIME         NULL,
    [IsActive]               BIT              NULL,
    [Logo]                   VARCHAR (100)    NULL,
    [SelfVideoClaims]        NVARCHAR (MAX)   NULL,
    [SelfDescription]        NVARCHAR (MAX)   NULL,
    [InsurerType]            VARCHAR (20)     NULL,
    [IsRecommended]          BIT              NULL,
    [RecommendedDescription] NVARCHAR (MAX)   NULL,
    [GarageDescription]      NVARCHAR (MAX)   NULL,
    [ShortCode]              VARCHAR (20)     NULL,
    [IsCommercialActive]     BIT              NULL,
    CONSTRAINT [PK__Insuranc__7E508CE64A186E87] PRIMARY KEY CLUSTERED ([InsurerId] ASC)
);





