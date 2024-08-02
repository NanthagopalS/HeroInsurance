CREATE TABLE [dbo].[Admin_NotificationBoradcast] (
    [NotificationBoradcastId] VARCHAR (100) CONSTRAINT [DF_Admin_NotificationBoradcast_NotificationBoradcastId] DEFAULT (newid()) NOT NULL,
    [NotificationId]          VARCHAR (100) NULL,
    [RecipientUserId]         VARCHAR (100) NULL,
    [AlertTypeId]             VARCHAR (100) NULL,
    [NotificationCategory]    VARCHAR (100) NULL,
    [NotificationOrigin]      VARCHAR (100) NULL,
    [Title]                   VARCHAR (100) NULL,
    [Description]             VARCHAR (100) NULL,
    [MessageTemplate]         VARCHAR (100) NULL,
    [MobileDeviceId]          VARCHAR (100) NULL,
    [IsDelivered]             VARCHAR (100) CONSTRAINT [DF_Admin_NotificationBoradcast_IsDelivered] DEFAULT ((0)) NULL,
    [IsActive]                BIT           CONSTRAINT [DF_Admin_NotificationBoradcast_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]               VARCHAR (50)  NULL,
    [CreatedOn]               DATETIME      CONSTRAINT [DF_Admin_NotificationBoradcast_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]               VARCHAR (50)  NULL,
    [UpdatedOn]               DATETIME      NULL,
    [IsInQueue]               BIT           CONSTRAINT [df_IsInQueue] DEFAULT ('0') NULL,
    [UserId]                  VARCHAR (500) NULL,
    [IsProccessed]            BIT           DEFAULT ('0') NULL,
    [FirebaseQueueId]         VARCHAR (MAX) NULL,
    [IsSeen]                  BIT           DEFAULT ('0') NULL,
    CONSTRAINT [PK_Admin_NotificationBoradcast_NotificationBoradcastId] PRIMARY KEY CLUSTERED ([NotificationBoradcastId] ASC)
);



