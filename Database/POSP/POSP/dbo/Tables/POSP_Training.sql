CREATE TABLE [dbo].[POSP_Training] (
    [Id]                               VARCHAR (500) CONSTRAINT [DF_POSP_Training_Id] DEFAULT (newid()) NOT NULL,
    [UserId]                           VARCHAR (500) NOT NULL,
    [GeneralInsuranceStartDateTime]    DATETIME      CONSTRAINT [DF_POSP_Training_GeneralInsuranceStartDateTime] DEFAULT (getdate()) NOT NULL,
    [GeneralInsuranceEndDateTime]      DATETIME      NULL,
    [LifeInsuranceStartDateTime]       DATETIME      NULL,
    [LifeInsuranceEndDateTime]         DATETIME      NULL,
    [IsGeneralInsuranceCompleted]      BIT           CONSTRAINT [DF_POSP_Training_IsGeneralInsuranceCompleted] DEFAULT ((0)) NULL,
    [IsLifeInsuranceCompleted]         BIT           CONSTRAINT [DF_POSP_Training_IsLifeInsuranceCompleted] DEFAULT ((0)) NULL,
    [IsTrainingCompleted]              BIT           CONSTRAINT [DF_POSP_Training_IsTrainingCompleted] DEFAULT ((0)) NULL,
    [IsActive]                         BIT           CONSTRAINT [DF_POSP_Training_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]                        VARCHAR (500) NULL,
    [CreatedOn]                        DATETIME      CONSTRAINT [DF_POSP_Training_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]                        VARCHAR (500) NULL,
    [UpdatedOn]                        DATETIME      NULL,
    [GeneralInsuranceIdealEndDateTime] DATETIME      NULL,
    [LifeInsuranceIdealEndDateTime]    DATETIME      NULL,
    CONSTRAINT [PK_POSP_Training_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

