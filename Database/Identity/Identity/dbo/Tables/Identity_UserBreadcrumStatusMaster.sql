CREATE TABLE [dbo].[Identity_UserBreadcrumStatusMaster] (
    [Id]            VARCHAR (100) CONSTRAINT [DF_Identity_UserBreadcrumStatusMaster_Id] DEFAULT (newid()) NOT NULL,
    [StatusName]    VARCHAR (100) NULL,
    [StatusValue]   VARCHAR (20)  NULL,
    [PriorityIndex] INT           NULL,
    [IsActive]      BIT           CONSTRAINT [DF_Identity_UserBreadcrumStatusMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (50)  NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_Identity_UserBreadcrumStatusMaster_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (50)  NULL,
    [UpdatedOn]     DATETIME      NULL,
    CONSTRAINT [PK_Identity_UserBreadcrumStatusMaster_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

