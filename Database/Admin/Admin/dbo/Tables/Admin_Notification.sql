CREATE TABLE [dbo].[Admin_Notification] (
    [NotificationId]       VARCHAR (100) CONSTRAINT [DF_Admin_Notification_NotificationId] DEFAULT (newid()) NOT NULL,
    [AlertTypeId]          VARCHAR (100) NULL,
    [RecipientId]          VARCHAR (100) NULL,
    [RecipientUserids]     VARCHAR (MAX) NULL,
    [NotificationCategory] VARCHAR (100) NULL,
    [NotificationOrigin]   VARCHAR (100) NULL,
    [NotificationTitle]    VARCHAR (100) NULL,
    [Description]          VARCHAR (MAX) NULL,
    [NotificationEventId]  VARCHAR (100) NULL,
    [IsPublished]          BIT           CONSTRAINT [DF_Admin_Notification_IsPublished] DEFAULT ((0)) NULL,
    [IsActive]             BIT           CONSTRAINT [DF_Admin_Notification_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]            VARCHAR (50)  NULL,
    [CreatedOn]            DATETIME      CONSTRAINT [DF_Admin_Notification_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]            VARCHAR (50)  NULL,
    [UpdatedOn]            DATETIME      NULL,
    CONSTRAINT [PK_Admin_Notification_NotificationId] PRIMARY KEY CLUSTERED ([NotificationId] ASC)
);

