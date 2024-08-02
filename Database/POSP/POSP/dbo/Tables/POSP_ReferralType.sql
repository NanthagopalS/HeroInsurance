CREATE TABLE [dbo].[POSP_ReferralType] (
    [RefererralTypeId] VARCHAR (500) CONSTRAINT [DF_POSP_ReferralType_RefererralTypeId] DEFAULT (newid()) NOT NULL,
    [RefererralType]   VARCHAR (500) NULL,
    [ImageURL]         VARCHAR (500) NULL,
    [ReferralBaseURL]  VARCHAR (500) NULL,
    [PriorityIndex]    INT           NULL,
    [IsActive]         BIT           CONSTRAINT [DF_POSP_ReferralType_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]        VARCHAR (500) NULL,
    [CreatedOn]        DATETIME      CONSTRAINT [DF_POSP_ReferralType_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]        VARCHAR (500) NULL,
    [UpdatedOn]        DATETIME      NULL,
    CONSTRAINT [PK_POSP_ReferralType_RefererralTypeId] PRIMARY KEY CLUSTERED ([RefererralTypeId] ASC)
);

