CREATE TABLE [dbo].[Identity_UserInquiry] (
    [Id]                 VARCHAR (100)  CONSTRAINT [DF_Identity_UserInquiry_Id] DEFAULT (newid()) NOT NULL,
    [UserName]           VARCHAR (100)  NOT NULL,
    [PhoneNumber]        VARCHAR (10)   NOT NULL,
    [InquiryDescription] VARCHAR (1000) NOT NULL,
    [IsActive]           BIT            CONSTRAINT [DF_Identity_UserInquiry_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]          VARCHAR (50)   NULL,
    [CreatedOn]          DATETIME       CONSTRAINT [DF_Identity_UserInquiry_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]          VARCHAR (50)   NULL,
    [UpdatedOn]          DATETIME       NULL,
    CONSTRAINT [PK_Identity_UserInquiry_UserId] PRIMARY KEY CLUSTERED ([Id] ASC)
);

