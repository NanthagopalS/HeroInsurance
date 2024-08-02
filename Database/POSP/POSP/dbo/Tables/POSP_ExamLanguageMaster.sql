CREATE TABLE [dbo].[POSP_ExamLanguageMaster] (
    [Id]           VARCHAR (500) CONSTRAINT [DF_POSP_ExamLanguageMaster_Id] DEFAULT (newid()) NOT NULL,
    [LanguageName] VARCHAR (10)  NOT NULL,
    [IsActive]     BIT           CONSTRAINT [DF_POSP_ExamLanguageMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]    VARCHAR (500) NULL,
    [CreatedOn]    DATETIME      CONSTRAINT [DF_POSP_ExamLanguageMaster_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]    VARCHAR (500) NULL,
    [UpdatedOn]    DATETIME      NULL,
    CONSTRAINT [PK_POSP_ExamLanguageMaster_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

