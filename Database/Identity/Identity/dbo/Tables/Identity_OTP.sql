CREATE TABLE [dbo].[Identity_OTP] (
    [Id]              VARCHAR (100) CONSTRAINT [DF_Identity_OTP_Id] DEFAULT (newid()) NOT NULL,
    [OTPId]           VARCHAR (100) NOT NULL,
    [OTPNumber]       VARCHAR (4)   NULL,
    [OTPSendDateTime] DATETIME      CONSTRAINT [DF_Identity_OTP_OTPSendDateTime] DEFAULT (getdate()) NULL,
    [UserId]          VARCHAR (50)  NULL,
    [MobileNo]        VARCHAR (10)  NULL,
    [IsVerify]        BIT           CONSTRAINT [DF_Identity_OTP_IsVerify] DEFAULT ((0)) NULL,
    [IsActive]        BIT           CONSTRAINT [DF_Identity_OTP_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]       VARCHAR (50)  NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_Identity_OTP_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)  NULL,
    [UpdatedOn]       DATETIME      NULL,
    [WrongOTPCount]   INT           CONSTRAINT [DF_Identity_OTP_WrongOTPCount] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Identity_OTP_OTPId] PRIMARY KEY CLUSTERED ([Id] ASC)
);

