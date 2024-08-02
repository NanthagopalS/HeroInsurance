CREATE TABLE [dbo].[Identity_User] (
    [UserId]                  VARCHAR (100) CONSTRAINT [DF_Identity_User_UserId] DEFAULT (newid()) NOT NULL,
    [UserName]                VARCHAR (100) NULL,
    [EmailId]                 VARCHAR (100) NULL,
    [MobileNo]                VARCHAR (10)  NULL,
    [Password]                VARCHAR (MAX) DEFAULT ('Abcd@1234') NULL,
    [RoleId]                  VARCHAR (100) NULL,
    [IsActive]                BIT           CONSTRAINT [DF_Identity_User_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]               VARCHAR (50)  NULL,
    [CreatedOn]               DATETIME      CONSTRAINT [DF_Identity_User_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]               VARCHAR (50)  NULL,
    [UpdatedOn]               DATETIME      NULL,
    [UserProfileStage]        INT           NULL,
    [EmpID]                   VARCHAR (100) NULL,
    [DOB]                     VARCHAR (20)  NULL,
    [Gender]                  VARCHAR (10)  NULL,
    [SignatureDocumentId]     VARCHAR (100) NULL,
    [POSPLeadId]              VARCHAR (50)  NULL,
    [POSPId]                  VARCHAR (50)  NULL,
    [IsAccountLock]           BIT           CONSTRAINT [DF_Identity_User_IsAccountLock] DEFAULT ((0)) NULL,
    [IIBStatus]               VARCHAR (20)  DEFAULT ('Pending') NULL,
    [IIBUploadStatus]         VARCHAR (20)  DEFAULT ('Pending') NULL,
    [CreatedByMode]           VARCHAR (20)  DEFAULT ('Self') NULL,
    [ReferralId]              VARCHAR (500) NULL,
    [LastActivityOn]          DATETIME      NULL,
    [RecipientTypeId]         VARCHAR (100) NULL,
    [IMID]                    VARCHAR (100) NULL,
    [HDFCPospId]              VARCHAR (20)  NULL,
    [IsRegistrationVerified]  BIT           DEFAULT ((0)) NOT NULL,
    [iib_upload_date]         DATETIME      NULL,
    [IsDocQCVerified]         BIT           DEFAULT ((0)) NULL,
    [IsPasswordResetRequired] BIT           DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Identity_User_UserId] PRIMARY KEY CLUSTERED ([UserId] ASC)
);











