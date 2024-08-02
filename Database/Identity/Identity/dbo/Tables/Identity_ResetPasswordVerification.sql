CREATE TABLE [dbo].[Identity_ResetPasswordVerification] (
    [Id]        VARCHAR (100) CONSTRAINT [DF_Identity_ResetPasswordVerification_Id] DEFAULT (newid()) NOT NULL,
    [GuId]      VARCHAR (100) NOT NULL,
    [UserId]    VARCHAR (50)  NULL,
    [EmailId]   VARCHAR (50)  NULL,
    [IsVerify]  BIT           CONSTRAINT [DF_Identity_ResetPasswordVerification_IsVerify] DEFAULT ((0)) NULL,
    [IsActive]  BIT           CONSTRAINT [DF_Identity_ResetPasswordVerification_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy] VARCHAR (100) NULL,
    [CreatedOn] DATETIME      CONSTRAINT [DF_Identity_ResetPasswordVerification_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (100) NULL,
    [UpdatedOn] DATETIME      NULL,
    CONSTRAINT [PK_Identity_ResetPasswordVerification_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

