CREATE TABLE [dbo].[Insurance_OTP] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [OTPId]           VARCHAR (100) NOT NULL,
    [OTPNumber]       VARCHAR (4)   NULL,
    [OTPSendDateTime] DATETIME      CONSTRAINT [DF_Insurance_OTP_OTPSendDateTime] DEFAULT (getdate()) NULL,
    [LeadId]          VARCHAR (50)  NULL,
    [UserId]          VARCHAR (50)  NULL,
    [MobileNo]        VARCHAR (10)  NULL,
    [IsVerify]        BIT           CONSTRAINT [DF_Insurance_OTP_IsVerify] DEFAULT ((0)) NULL,
    [IsActive]        BIT           CONSTRAINT [DF_Insurance_OTP_IsActive] DEFAULT ((1)) NULL,
    [CreatedOn]       DATETIME      CONSTRAINT [DF_Insurance_OTP_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedOn]       DATETIME      NULL,
    [WrongOTPCount]   INT           CONSTRAINT [DF_Insurance_OTP_WrongOTPCount] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Identity_OTP_OTPId] PRIMARY KEY CLUSTERED ([Id] ASC)
);

