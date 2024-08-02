CREATE TABLE [dbo].[Insurance_PremiumDetails] (
    [LeadId]        VARCHAR (100) CONSTRAINT [DF_Insurance_LeadId] DEFAULT (newid()) NULL,
    [InsurerId]     VARCHAR (50)  NULL,
    [AddOns]        VARCHAR (10)  NULL,
    [NilDep]        VARCHAR (10)  NULL,
    [CPA]           VARCHAR (10)  NULL,
    [BasicOD]       VARCHAR (100) NULL,
    [BasicTP]       VARCHAR (100) NULL,
    [TotalTP]       VARCHAR (100) NULL,
    [NetPremium]    VARCHAR (100) NULL,
    [GST]           VARCHAR (50)  NULL,
    [GrossPremium]  VARCHAR (100) NULL,
    [GrossDiscount] VARCHAR (100) NULL,
    [CreatedBy]     VARCHAR (100) NULL,
    [CreatedOn]     DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (100) NULL,
    [UpdatedOn]     DATETIME      NULL
);

