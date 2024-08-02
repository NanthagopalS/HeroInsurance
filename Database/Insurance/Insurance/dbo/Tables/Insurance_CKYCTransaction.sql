CREATE TABLE [dbo].[Insurance_CKYCTransaction] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [InsurerId]          VARCHAR (50)  NULL,
    [QuoteTransactionId] VARCHAR (50)  NULL,
    [CKYCRequestBody]    VARCHAR (MAX) NULL,
    [CKYCResponseBody]   VARCHAR (MAX) NULL,
    [CreatedBy]          VARCHAR (50)  NULL,
    [CreatedOn]          DATETIME      NULL,
    [UpdatedBy]          VARCHAR (50)  NULL,
    [UpdatedOn]          DATETIME      NULL,
    [LeadId]             VARCHAR (50)  NULL,
    [PhotoId]            VARCHAR (MAX) NULL,
    [Stage]              VARCHAR (3)   NULL,
    [KYCId]              VARCHAR (50)  NULL,
    [CKYCNumber]         VARCHAR (50)  NULL,
    [CKYCStatus]         VARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

