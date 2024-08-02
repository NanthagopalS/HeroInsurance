CREATE TABLE [dbo].[Admin_BannerDetail] (
    [BannerId]          VARCHAR (100) CONSTRAINT [DF_Admin_BannerDetail_BannerId] DEFAULT (newid()) NOT NULL,
    [ProductCategoryId] VARCHAR (100) NULL,
    [DocumentId]        VARCHAR (100) NULL,
    [IsActive]          BIT           CONSTRAINT [DF_Admin_BannerDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]         VARCHAR (50)  NULL,
    [CreatedOn]         DATETIME      CONSTRAINT [DF_Admin_BannerDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]         VARCHAR (50)  NULL,
    [UpdatedOn]         DATETIME      NULL,
    CONSTRAINT [PK_Admin_BannerDetail_BannerId] PRIMARY KEY CLUSTERED ([BannerId] ASC)
);

