CREATE TABLE [dbo].[Identity_ErrorCode] (
    [Id]           VARCHAR (50)  CONSTRAINT [DF_Identity_ErrorCode_Id] DEFAULT (newid()) NOT NULL,
    [ErrorType]    VARCHAR (10)  NOT NULL,
    [ErrorCode]    VARCHAR (10)  NOT NULL,
    [ErrorKey]     VARCHAR (50)  NOT NULL,
    [ErrorMessage] VARCHAR (200) NOT NULL,
    [IsActive]     BIT           CONSTRAINT [DF_Identity_ErrorCode_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]    VARCHAR (500) NULL,
    [CreatedOn]    DATETIME      CONSTRAINT [DF_Identity_ErrorCode_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]    VARCHAR (500) NULL,
    [UpdatedOn]    DATETIME      NULL,
    CONSTRAINT [PK_Identity_ErrorCode_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

