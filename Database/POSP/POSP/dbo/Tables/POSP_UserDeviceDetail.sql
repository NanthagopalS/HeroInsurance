CREATE TABLE [dbo].[POSP_UserDeviceDetail] (
    [UserDeviceId]   VARCHAR (100) CONSTRAINT [DF_POSP_UserDeviceDetail_UserDeviceId] DEFAULT (newid()) NOT NULL,
    [UserId]         VARCHAR (100) NULL,
    [MobileDeviceId] VARCHAR (200) NULL,
    [BrowserId]      VARCHAR (200) NULL,
    [GfcToken]       VARCHAR (100) NULL,
    [IsActive]       BIT           CONSTRAINT [DF_POSP_UserDeviceDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]      VARCHAR (50)  NULL,
    [CreatedOn]      DATETIME      CONSTRAINT [DF_POSP_UserDeviceDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]      VARCHAR (50)  NULL,
    [UpdatedOn]      DATETIME      NULL,
    CONSTRAINT [PK_POSP_UserDeviceDetail_UserDeviceId] PRIMARY KEY CLUSTERED ([UserDeviceId] ASC)
);



