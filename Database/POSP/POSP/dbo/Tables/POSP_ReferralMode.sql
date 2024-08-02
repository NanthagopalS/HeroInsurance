CREATE TABLE [dbo].[POSP_ReferralMode] (
    [ReferralModeId]   VARCHAR (500) CONSTRAINT [DF_POSP_ReferralMode_ReferralModeId] DEFAULT (newid()) NOT NULL,
    [ReferralModeType] VARCHAR (500) NULL,
    [ImageURL]         VARCHAR (500) NULL,
    [PriorityIndex]    INT           NULL,
    [IsActive]         BIT           CONSTRAINT [DF_POSP_ReferralMode_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]        VARCHAR (500) NULL,
    [CreatedOn]        DATETIME      CONSTRAINT [DF_POSP_ReferralMode_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]        VARCHAR (500) NULL,
    [UpdatedOn]        DATETIME      NULL,
    CONSTRAINT [PK_POSP_ReferralMode_ReferralModeId] PRIMARY KEY CLUSTERED ([ReferralModeId] ASC)
);

