CREATE TABLE [dbo].[Identity_EmailVerification] (
    [Id]               VARCHAR (100) CONSTRAINT [DF_Identity_EmailVerification_Id] DEFAULT (newid()) NOT NULL,
    [GuId]             VARCHAR (100) NOT NULL,
    [LinkSendDateTime] DATETIME      CONSTRAINT [DF_Identity_EmailVerification_LinkSendDateTime] DEFAULT (getdate()) NULL,
    [UserId]           VARCHAR (50)  NULL,
    [EmailId]          VARCHAR (50)  NULL,
    [IsVerify]         BIT           CONSTRAINT [DF_Identity_EmailVerification_IsVerify] DEFAULT ((0)) NULL,
    [IsActive]         BIT           CONSTRAINT [DF_Identity_EmailVerification_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]        VARCHAR (50)  NULL,
    [CreatedOn]        DATETIME      CONSTRAINT [DF_Identity_EmailVerification_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]        VARCHAR (50)  NULL,
    [UpdatedOn]        DATETIME      NULL,
    CONSTRAINT [PK_Identity_EmailVerification_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

