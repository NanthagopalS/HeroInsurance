CREATE TABLE [dbo].[Insurance_QuoteTransaction] (
    [QuoteTransactionId]    VARCHAR (50)    CONSTRAINT [DF__Insurance__Quote__50FB042B] DEFAULT (newid()) NOT NULL,
    [InsurerId]             VARCHAR (50)    NULL,
    [RequestBody]           NVARCHAR (MAX)  NULL,
    [ResponseBody]          NVARCHAR (MAX)  NULL,
    [CommonResponse]        NVARCHAR (MAX)  NULL,
    [CreatedBy]             VARCHAR (100)   NULL,
    [CreatedOn]             DATETIME        CONSTRAINT [DF_Insurance_QuoteTransaction_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]             VARCHAR (100)   NULL,
    [UpdatedOn]             DATETIME        NULL,
    [StageID]               VARCHAR (50)    NULL,
    [LeadId]                NVARCHAR (50)   NULL,
    [MinIDV]                DECIMAL (18, 2) NULL,
    [MaxIDV]                DECIMAL (18, 2) NULL,
    [RecommendedIDV]        DECIMAL (18, 2) NULL,
    [TransactionId]         VARCHAR (100)   NULL,
    [VehicleTypeId]         VARCHAR (100)   NULL,
    [PolicyTypeId]          VARCHAR (100)   NULL,
    [IsBrandNew]            BIT             NULL,
    [RefQuoteTransactionId] VARCHAR (50)    NULL,
    [SelectedIDV]           VARCHAR (100)   NULL,
    [CustomIDV]             VARCHAR (100)   NULL,
    [ProposalId]            VARCHAR (50)    NULL,
    [PolicyId]              VARCHAR (50)    NULL,
    [VehicleNumber]         VARCHAR (20)    NULL,
    [RTOId]                 VARCHAR (50)    NULL,
    CONSTRAINT [PK_Insurance_QuoteTransaction] PRIMARY KEY CLUSTERED ([QuoteTransactionId] ASC)
);










GO
CREATE NONCLUSTERED INDEX [NonClustered_InsurerId_StageId_LeadId]
    ON [dbo].[Insurance_QuoteTransaction]([InsurerId] ASC, [StageID] ASC, [LeadId] ASC);

