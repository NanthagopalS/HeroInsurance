CREATE TABLE [dbo].[POSP_Agreement] (
    [Id]                   VARCHAR (50)  CONSTRAINT [DF_POSP_Agreement_Id] DEFAULT (newid()) NOT NULL,
    [UserId]               VARCHAR (50)  NOT NULL,
    [AgreementId]          VARCHAR (50)  NULL,
    [PreSignedAgreementId] VARCHAR (50)  NULL,
    [IsActive]             BIT           CONSTRAINT [DF_POSP_Agreement_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]            VARCHAR (50)  NULL,
    [CreatedOn]            DATETIME      CONSTRAINT [DF_POSP_Agreement_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]            VARCHAR (50)  NULL,
    [UpdatedOn]            DATETIME      NULL,
    [StampId]              VARCHAR (100) CONSTRAINT [DF_POSP_Agreement_StampId] DEFAULT (NULL) NULL,
    [IsRevoked]            BIT           NULL,
    CONSTRAINT [PK_POSP_Agreement_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

