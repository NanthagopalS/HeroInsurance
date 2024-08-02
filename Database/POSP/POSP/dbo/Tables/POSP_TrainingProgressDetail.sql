CREATE TABLE [dbo].[POSP_TrainingProgressDetail] (
    [Id]                    VARCHAR (500) CONSTRAINT [DF_POSP_TrainingProgressDetail_Id] DEFAULT (newid()) NOT NULL,
    [TrainingId]            VARCHAR (500) NOT NULL,
    [TrainingMaterialId]    VARCHAR (500) NOT NULL,
    [TrainingStartDateTime] DATETIME      CONSTRAINT [DF_POSP_TrainingProgressDetail_TrainingStartDateTime] DEFAULT (getdate()) NOT NULL,
    [IsTrainingCompleted]   BIT           CONSTRAINT [DF_POSP_TrainingProgressDetail_IsTrainingCompleted] DEFAULT ((1)) NULL,
    [IsActive]              BIT           CONSTRAINT [DF_POSP_TrainingProgressDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]             VARCHAR (500) NULL,
    [CreatedOn]             DATETIME      CONSTRAINT [DF_POSP_TrainingProgressDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]             VARCHAR (500) NULL,
    [UpdatedOn]             DATETIME      NULL,
    CONSTRAINT [PK_POSP_TrainingProgressDetail_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

