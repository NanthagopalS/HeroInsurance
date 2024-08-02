CREATE TABLE [dbo].[Insurance_ProposalDynamicDetails] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [InsurerId]           VARCHAR (50)  NULL,
    [QuoteTransactionId]  VARCHAR (50)  NULL,
    [LeadId]              VARCHAR (50)  NULL,
    [VehicleNumber]       VARCHAR (50)  NULL,
    [VariantId]           VARCHAR (100) NULL,
    [ProposalRequestBody] VARCHAR (MAX) NULL,
    [CreatedBy]           INT           NULL,
    [CreatedOn]           DATETIME      NULL,
    [UpdatedBy]           INT           NULL,
    [UpdatedOn]           DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

