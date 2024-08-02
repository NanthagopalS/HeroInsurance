CREATE TABLE [dbo].[Identity_EducationQualificationTypeMaster] (
    [Id]                         VARCHAR (100) CONSTRAINT [DF_Identity_EducationQualificationTypeMaster_Id] DEFAULT (newid()) NOT NULL,
    [EducationQualificationType] VARCHAR (100) NOT NULL,
    [IsActive]                   BIT           CONSTRAINT [DF_Identity_EducationQualificationTypeMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]                  VARCHAR (50)  NULL,
    [CreatedOn]                  DATETIME      CONSTRAINT [DF_Identity_EducationQualificationTypeMaster_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]                  VARCHAR (50)  NULL,
    [UpdatedOn]                  DATETIME      NULL,
    [QualificationID]            INT           NULL,
    CONSTRAINT [PK_EducationQualification_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

