CREATE TABLE [dbo].[Identity_UserBreadcrumStatusDetail] (
    [Id]        VARCHAR (100) CONSTRAINT [DF_Identity_UserBreadcrumStatusDetail_Id] DEFAULT (newid()) NOT NULL,
    [UserId]    VARCHAR (100) NOT NULL,
    [StatusId]  VARCHAR (100) NOT NULL,
    [IsActive]  BIT           CONSTRAINT [DF_Identity_UserBreadcrumStatusDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy] VARCHAR (50)  NULL,
    [CreatedOn] DATETIME      CONSTRAINT [DF_Identity_UserBreadcrumStatusDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50)  NULL,
    [UpdatedOn] DATETIME      NULL,
    CONSTRAINT [PK_Identity_UserBreadcrumStatusDetail_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

