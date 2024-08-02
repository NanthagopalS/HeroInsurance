CREATE TABLE [dbo].[Identity_UserBankDetail] (
    [Id]                VARCHAR (100) CONSTRAINT [DF_Identity_UserBankDetail_Id] DEFAULT (newid()) NOT NULL,
    [UserId]            VARCHAR (100) NOT NULL,
    [BankId]            VARCHAR (100) NULL,
    [IFSC]              VARCHAR (100) NULL,
    [AccountHolderName] VARCHAR (100) NOT NULL,
    [AccountNumber]     VARCHAR (100) NULL,
    [IsActive]          BIT           CONSTRAINT [DF_Identity_UserBankDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]         VARCHAR (50)  NULL,
    [CreatedOn]         DATETIME      CONSTRAINT [DF_Identity_UserBankDetail_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]         VARCHAR (50)  NULL,
    [UpdatedOn]         DATETIME      NULL,
    CONSTRAINT [PK_Identity_UserBankDetail_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);



