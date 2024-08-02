CREATE TABLE [dbo].[POSP_ReferralNewUserDetails] (
    [ReferralNewUserId] VARCHAR (500) CONSTRAINT [DF_POSP_ReferralNewUserDetails_ReferralNewUserId] DEFAULT (newid()) NOT NULL,
    [RefererralMode]    VARCHAR (500) NULL,
    [UserName]          VARCHAR (500) NULL,
    [EmailId]           VARCHAR (500) NULL,
    [PhoneNumber]       VARCHAR (10)  NULL,
    [IsActive]          BIT           CONSTRAINT [DF_POSP_ReferralNewUserDetails_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]         VARCHAR (500) NULL,
    [CreatedOn]         DATETIME      CONSTRAINT [DF_POSP_ReferralNewUserDetails_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]         VARCHAR (500) NULL,
    [UpdatedOn]         DATETIME      NULL,
    [ReferralUserId]    VARCHAR (100) NULL,
    CONSTRAINT [PK_POSP_ReferralNewUserDetails_ReferralNewUserId] PRIMARY KEY CLUSTERED ([ReferralNewUserId] ASC)
);

