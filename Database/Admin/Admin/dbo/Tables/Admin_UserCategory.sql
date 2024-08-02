CREATE TABLE [dbo].[Admin_UserCategory] (
    [UserCategoryId]   VARCHAR (100) CONSTRAINT [DF_Admin_UserCategory_UserCategoryId] DEFAULT (newid()) NOT NULL,
    [UserCategoryName] VARCHAR (100) NULL,
    [IsActive]         BIT           CONSTRAINT [DF_Admin_UserCategory_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]        VARCHAR (50)  NULL,
    [CreatedOn]        DATETIME      CONSTRAINT [DF_Admin_UserCategory_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]        VARCHAR (50)  NULL,
    [UpdatedOn]        DATETIME      NULL,
    CONSTRAINT [PK_Admin_UserCategory_UserCategoryId] PRIMARY KEY CLUSTERED ([UserCategoryId] ASC)
);

