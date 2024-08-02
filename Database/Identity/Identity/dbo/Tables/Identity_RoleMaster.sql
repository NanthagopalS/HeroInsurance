CREATE TABLE [dbo].[Identity_RoleMaster] (
    [RoleId]      VARCHAR (50) DEFAULT (newid()) NOT NULL,
    [RoleName]    VARCHAR (80) NULL,
    [CreatedBy]   VARCHAR (20) NULL,
    [CreatedOn]   DATETIME     DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (20) NULL,
    [UpdatedOn]   DATETIME     NULL,
    [RoleTypeID]  INT          NULL,
    [RoleLevelID] INT          NULL,
    CONSTRAINT [PK__Identity_Roleid] PRIMARY KEY CLUSTERED ([RoleId] ASC)
);

