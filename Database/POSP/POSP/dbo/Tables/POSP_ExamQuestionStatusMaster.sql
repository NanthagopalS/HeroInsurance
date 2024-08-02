CREATE TABLE [dbo].[POSP_ExamQuestionStatusMaster] (
    [Id]            VARCHAR (500) CONSTRAINT [DF_POSP_ExamQuestionStatusMaster_Id] DEFAULT (newid()) NOT NULL,
    [StatusValue]   VARCHAR (500) NOT NULL,
    [PriorityIndex] INT           NULL,
    [IsActive]      BIT           CONSTRAINT [DF_POSP_ExamQuestionStatusMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (500) NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_POSP_ExamQuestionStatusMaster_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (500) NULL,
    [UpdatedOn]     DATETIME      NULL,
    CONSTRAINT [PK_POSP_ExamQuestionStatusMaster_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

