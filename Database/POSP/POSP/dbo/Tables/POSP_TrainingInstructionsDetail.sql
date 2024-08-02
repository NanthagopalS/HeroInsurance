CREATE TABLE [dbo].[POSP_TrainingInstructionsDetail] (
    [Id]                VARCHAR (500) CONSTRAINT [DF_POSP_TrainingInstructionsDetail_Id] DEFAULT (newid()) NOT NULL,
    [InstructionDetail] VARCHAR (MAX) NULL,
    [PriorityIndex]     INT           NULL,
    [IsActive]          BIT           CONSTRAINT [DF_POSP_TrainingInstructionsDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]         VARCHAR (500) NULL,
    [CreatedOn]         DATETIME      CONSTRAINT [DF_POSP_TrainingInstructionsDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]         VARCHAR (500) NULL,
    [UpdatedOn]         DATETIME      NULL,
    CONSTRAINT [PK_POSP_TrainingInstructionsDetail_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

