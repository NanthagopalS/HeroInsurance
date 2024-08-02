CREATE TABLE [dbo].[Admin_RoleLevel] (
    [RoleLevelId]   VARCHAR (100) CONSTRAINT [DF_Admin_RoleLevel_RoleLevelId] DEFAULT (newid()) NOT NULL,
    [RoleLevelName] VARCHAR (20)  NULL,
    [PriorityIndex] INT           NULL,
    [IsActive]      BIT           CONSTRAINT [DF_Admin_RoleLevel_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (50)  NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_Admin_RoleLevel_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (50)  NULL,
    [UpdatedOn]     DATETIME      NULL,
    CONSTRAINT [PK_Admin_RoleLevel_RoleLevelId] PRIMARY KEY CLUSTERED ([RoleLevelId] ASC)
);

