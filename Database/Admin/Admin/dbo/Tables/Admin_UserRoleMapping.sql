CREATE TABLE [dbo].[Admin_UserRoleMapping] (
    [UserRoleMappingId]       VARCHAR (100) CONSTRAINT [DF_Admin_UserRoleMapping_UserRoleMappingId] DEFAULT (newid()) NOT NULL,
    [UserId]                  VARCHAR (100) NULL,
    [RoleId]                  VARCHAR (100) NULL,
    [ReportingUserId]         VARCHAR (100) NULL,
    [CategoryId]              VARCHAR (100) NULL,
    [BUId]                    VARCHAR (100) NULL,
    [RoleTypeId]              VARCHAR (100) NULL,
    [IdentityRoleId]          VARCHAR (100) NULL,
    [ReportingIdentityRoleId] VARCHAR (100) NULL,
    [IsActive]                BIT           CONSTRAINT [DF_Admin_UserRoleMapping_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]               VARCHAR (50)  NULL,
    [CreatedOn]               DATETIME      CONSTRAINT [DF_Admin_UserRoleMapping_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]               VARCHAR (50)  NULL,
    [UpdatedOn]               DATETIME      NULL,
    CONSTRAINT [PK_Admin_UserRoleMapping_UserRoleMappingId] PRIMARY KEY CLUSTERED ([UserRoleMappingId] ASC)
);

