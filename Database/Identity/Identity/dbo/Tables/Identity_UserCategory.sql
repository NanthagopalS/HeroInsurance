CREATE TABLE [dbo].[Identity_UserCategory] (
    [CategoryID]       INT           IDENTITY (1, 1) NOT NULL,
    [UserCategoryName] NVARCHAR (50) NULL,
    [CreatedBy]        NVARCHAR (50) NULL,
    [CreatedDate]      DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]        NVARCHAR (50) NULL,
    [UpdatedDate]      DATETIME      NULL,
    CONSTRAINT [PK_Identity_UserCategory] PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);

