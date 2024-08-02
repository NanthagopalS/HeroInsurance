CREATE TABLE [dbo].[Insurance_PaymentTransaction] (
    [PaymentTransactionId]     VARCHAR (50)    DEFAULT (newid()) NOT NULL,
    [QuoteTransactionId]       VARCHAR (50)    NULL,
    [InsurerId]                VARCHAR (50)    NULL,
    [ApplicationId]            VARCHAR (200)   NULL,
    [ProposalNumber]           VARCHAR (50)    NULL,
    [Status]                   VARCHAR (50)    NULL,
    [CreatedBy]                VARCHAR (100)   NULL,
    [CreatedOn]                DATETIME        DEFAULT (getdate()) NULL,
    [UpdatedBy]                VARCHAR (100)   NULL,
    [UpdatedOn]                DATETIME        NULL,
    [LeadId]                   VARCHAR (20)    NULL,
    [Amount]                   DECIMAL (18, 2) NULL,
    [PaymentTransactionNumber] VARCHAR (MAX)   NULL,
    [CKYCStatus]               VARCHAR (50)    NULL,
    [CKYCLink]                 VARCHAR (200)   NULL,
    [CKYCFailReason]           VARCHAR (50)    NULL,
    [PolicyDocumentLink]       VARCHAR (500)   NULL,
    [DocumentId]               VARCHAR (100)   NULL,
    [PolicyNumber]             NVARCHAR (MAX)  NULL,
    [CustomerId]               VARCHAR (100)   NULL,
    [IsTP]                     BIT             NULL,
    [BankName]                 VARCHAR (100)   NULL,
    [BankPaymentRefNum]        VARCHAR (100)   NULL,
    [PaymentDate]              VARCHAR (50)    NULL,
    [PaymentCorrelationId]     VARCHAR (50)    NULL
);









