CREATE TABLE [dbo].[POSP_ReferralDetails] (
    [ReferralId]       VARCHAR (500) CONSTRAINT [DF_POSP_ReferralDetails_ReferralId] DEFAULT (newid()) NOT NULL,
    [RefererralTypeId] VARCHAR (500) NULL,
    [ReferralModeId]   VARCHAR (500) NULL,
    [IsActive]         BIT           CONSTRAINT [DF_POSP_ReferralDetails_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]        VARCHAR (500) NULL,
    [CreatedOn]        DATETIME      CONSTRAINT [DF_POSP_ReferralDetails_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]        VARCHAR (500) NULL,
    [UpdatedOn]        DATETIME      NULL,
    [POSPUserId]       VARCHAR (500) NULL,
    CONSTRAINT [PK_POSP_ReferralDetails_ReferralId] PRIMARY KEY CLUSTERED ([ReferralId] ASC)
);

