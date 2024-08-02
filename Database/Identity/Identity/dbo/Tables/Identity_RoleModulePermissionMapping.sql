CREATE TABLE [dbo].[Identity_RoleModulePermissionMapping] (
    [RoleModulePermissionID] INT           IDENTITY (1, 1) NOT NULL,
    [RoleID]                 NVARCHAR (50) NULL,
    [ModuleID]               INT           NULL,
    [AddPermission]          BIT           NULL,
    [EditPermission]         BIT           NULL,
    [ViewPermission]         BIT           NULL,
    [DeletePermission]       BIT           NULL,
    [DownloadPermission]     BIT           NULL,
    [RoleTypeID]             INT           NULL,
    [CreatedBy]              NVARCHAR (50) NULL,
    [CreatedOn]              DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]              NVARCHAR (50) NULL,
    [UpdatedOn]              DATETIME      NULL,
    [isActive]               BIT           DEFAULT ((0)) NULL,
    [IdentityRoleId]         INT           NULL,
    CONSTRAINT [PK_RoleModulePermissionID] PRIMARY KEY CLUSTERED ([RoleModulePermissionID] ASC)
);

