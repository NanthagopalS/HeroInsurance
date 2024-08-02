CREATE TABLE [dbo].[Admin_NotificationEvent] (
    [NotificationEventId] VARCHAR (100) CONSTRAINT [DF_Admin_NotificationEvent_NotificationEventId] DEFAULT (newid()) NOT NULL,
    [EventCode]           VARCHAR (100) NULL,
    [EventTitle]          VARCHAR (100) NULL,
    [EventDescription]    VARCHAR (100) NULL,
    [IsSMS]               BIT           NULL,
    [IsEmail]             BIT           NULL,
    [IsWhatsApp]          BIT           NULL,
    [IsInApp]             BIT           NULL,
    [IsPush]              BIT           NULL,
    [EmailTemplate]       VARCHAR (100) NULL,
    [IsActive]            BIT           CONSTRAINT [DF_Admin_NotificationEvent_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]           VARCHAR (50)  NULL,
    [CreatedOn]           DATETIME      CONSTRAINT [DF_Admin_NotificationEvent_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]           VARCHAR (50)  NULL,
    [UpdatedOn]           DATETIME      NULL,
    CONSTRAINT [PK_Admin_NotificationEvent_NotificationEventId] PRIMARY KEY CLUSTERED ([NotificationEventId] ASC)
);

