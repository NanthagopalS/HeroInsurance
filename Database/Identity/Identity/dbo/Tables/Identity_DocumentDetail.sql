CREATE TABLE [dbo].[Identity_DocumentDetail] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_Identity_DocumentDetail_Id] DEFAULT (newid()) NOT NULL,
    [UserId]           VARCHAR (100)    NOT NULL,
    [DocumentTypeId]   VARCHAR (100)    NOT NULL,
    [DocumentFileName] VARCHAR (500)    NULL,
    [VerifyOn]         DATETIME         NULL,
    [VerifyByUserId]   NVARCHAR (450)   NOT NULL,
    [IsVerify]         BIT              CONSTRAINT [DF_Identity_DocumentDetail_IsVerify] DEFAULT ((0)) NULL,
    [IsApprove]        BIT              NULL,
    [IsActive]         BIT              CONSTRAINT [DF_Identity_DocumentDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]        VARCHAR (50)     NULL,
    [CreatedOn]        DATETIME         CONSTRAINT [DF_Identity_DocumentDetail_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]        VARCHAR (50)     NULL,
    [UpdatedOn]        DATETIME         NULL,
    [DocumentId]       VARCHAR (100)    NULL,
    [FileSize]         VARCHAR (255)    NULL,
    [BackOfficeRemark] VARCHAR (100)    NULL,
    CONSTRAINT [PK_Identity_DocumentDetail_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

