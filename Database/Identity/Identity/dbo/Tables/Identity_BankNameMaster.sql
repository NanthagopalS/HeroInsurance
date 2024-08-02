CREATE TABLE [dbo].[Identity_BankNameMaster] (
    [Id]        VARCHAR (100) CONSTRAINT [DF_Identity_BankNameMaster_Id] DEFAULT (newid()) NOT NULL,
    [BankName]  VARCHAR (100) NOT NULL,
    [IsActive]  BIT           CONSTRAINT [DF_Identity_BankNameMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy] VARCHAR (50)  NULL,
    [CreatedOn] DATETIME      CONSTRAINT [DF_Identity_BankNameMaster_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy] VARCHAR (50)  NULL,
    [UpdatedOn] DATETIME      NULL,
    CONSTRAINT [PK_BankName_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

