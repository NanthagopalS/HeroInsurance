CREATE TABLE [dbo].[Admin_RequestForEditProfile] (
    [Id]                    VARCHAR (100) CONSTRAINT [DF_Admin_RequestForEditProfile_Id] DEFAULT (newid()) NOT NULL,
    [UserId]                VARCHAR (100) NOT NULL,
    [RequestType]           VARCHAR (100) NULL,
    [NewRequestTypeContent] VARCHAR (100) NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Admin_RequestForEditProfile_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]             VARCHAR (100) NULL,
    [CreatedOn]             DATETIME      CONSTRAINT [DF_Admin_RequestForEditProfile_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]             VARCHAR (100) NULL,
    [UpdatedOn]             DATETIME      NULL,
    CONSTRAINT [PK_Admin_RequestForEditProfile_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

