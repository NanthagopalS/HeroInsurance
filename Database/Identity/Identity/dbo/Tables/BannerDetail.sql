CREATE TABLE [dbo].[BannerDetail] (
    [Id]                VARCHAR (100) CONSTRAINT [DF_BannerDetail_Id] DEFAULT (newid()) NOT NULL,
    [BannerFileName]    VARCHAR (100) NOT NULL,
    [BannerStoragePath] VARCHAR (200) NULL,
    [IsActive]          BIT           CONSTRAINT [DF_BannerDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]         VARCHAR (50)  NULL,
    [CreatedOn]         DATETIME      CONSTRAINT [DF_BannerDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]         VARCHAR (50)  NULL,
    [UpdatedOn]         DATETIME      NULL,
    [DocumentId]        VARCHAR (100) NULL,
    [BannerType]        VARCHAR (10)  NULL,
    CONSTRAINT [PK_BannerDetail_OTPId] PRIMARY KEY CLUSTERED ([Id] ASC)
);

