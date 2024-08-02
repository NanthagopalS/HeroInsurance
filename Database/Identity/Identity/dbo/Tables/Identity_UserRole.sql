CREATE TABLE [dbo].[Identity_UserRole] (
    [IdentityRoleId] INT           IDENTITY (1, 1) NOT NULL,
    [RoleTypeID]     VARCHAR (100) NULL,
    [RoleTitleName]  VARCHAR (80)  NULL,
    [BUID]           VARCHAR (100) NULL,
    [RoleLevelID]    VARCHAR (100) NULL,
    [CreatedBy]      VARCHAR (50)  NULL,
    [CreatedOn]      DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]      VARCHAR (50)  NULL,
    [UpdatedOn]      DATETIME      NULL,
    CONSTRAINT [PK__Identity__IdentityRoleId] PRIMARY KEY CLUSTERED ([IdentityRoleId] ASC)
);

