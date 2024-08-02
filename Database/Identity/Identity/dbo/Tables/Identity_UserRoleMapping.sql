CREATE TABLE [dbo].[Identity_UserRoleMapping] (
    [UserRoleID]              INT           IDENTITY (1, 1) NOT NULL,
    [UserID]                  NVARCHAR (50) NULL,
    [RoleID]                  NVARCHAR (50) NULL,
    [ReportingUserID]         NVARCHAR (50) NULL,
    [CategoryID]              INT           NULL,
    [BUID]                    INT           NULL,
    [RoleTypeID]              INT           NULL,
    [IsActive]                BIT           DEFAULT ((0)) NULL,
    [CreatedBy]               NVARCHAR (50) NULL,
    [CreatedOn]               DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]               VARCHAR (50)  NULL,
    [UpdatedOn]               DATETIME      NULL,
    [IdentityRoleId]          INT           NULL,
    [ReportingIdentityRoleId] INT           NULL,
    CONSTRAINT [PK_Identity_UserRoleMapping] PRIMARY KEY CLUSTERED ([UserRoleID] ASC)
);

