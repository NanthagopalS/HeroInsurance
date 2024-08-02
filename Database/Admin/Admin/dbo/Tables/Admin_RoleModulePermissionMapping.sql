CREATE TABLE [dbo].[Admin_RoleModulePermissionMapping] (
    [RoleModulePermissionId] VARCHAR (100) CONSTRAINT [DF_Admin_RoleModulePermissionMapping_RoleModulePermissionId] DEFAULT (newid()) NOT NULL,
    [RoleTypeId]             VARCHAR (100) NULL,
    [RoleId]                 VARCHAR (100) NULL,
    [ModuleId]               VARCHAR (100) NULL,
    [AddPermission]          BIT           NULL,
    [EditPermission]         BIT           NULL,
    [ViewPermission]         BIT           NULL,
    [DeletePermission]       BIT           NULL,
    [DownloadPermission]     BIT           NULL,
    [IdentityRoleId]         VARCHAR (100) NULL,
    [IsActive]               BIT           CONSTRAINT [DF_Admin_RoleModulePermissionMapping_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]              VARCHAR (50)  NULL,
    [CreatedOn]              DATETIME      CONSTRAINT [DF_Admin_RoleModulePermissionMapping_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]              VARCHAR (50)  NULL,
    [UpdatedOn]              DATETIME      NULL,
    CONSTRAINT [PK_Admin_RoleModulePermissionMapping_RoleModulePermissionId] PRIMARY KEY CLUSTERED ([RoleModulePermissionId] ASC)
);

