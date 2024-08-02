CREATE TABLE [dbo].[Admin_RoleType] (
    [RoleTypeID]    VARCHAR (100) CONSTRAINT [DF_Admin_RoleType_RoleTypeID] DEFAULT (newid()) NOT NULL,
    [RoleTypeName]  VARCHAR (20)  NULL,
    [PriorityIndex] INT           NULL,
    [IsActive]      BIT           CONSTRAINT [DF_Admin_RoleType_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (50)  NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_Admin_RoleType_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (50)  NULL,
    [UpdatedOn]     DATETIME      NULL,
    CONSTRAINT [PK_Admin_RoleType_RoleTypeID] PRIMARY KEY CLUSTERED ([RoleTypeID] ASC)
);

