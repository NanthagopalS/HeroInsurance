CREATE TABLE [dbo].[Identity_PanVerification] (
    [PanVerificationId]   VARCHAR (50)  DEFAULT (newid()) NOT NULL,
    [PanNumber]           VARCHAR (100) NOT NULL,
    [Name]                VARCHAR (100) NOT NULL,
    [FatherName]          VARCHAR (100) NOT NULL,
    [DOB]                 VARCHAR (100) NOT NULL,
    [InstanceId]          VARCHAR (100) NOT NULL,
    [InstanceCallbackUrl] VARCHAR (100) NOT NULL,
    [CreatedBy]           VARCHAR (100) NOT NULL,
    [CreatedOn]           DATETIME      NOT NULL,
    [UpdatedBy]           VARCHAR (100) NULL,
    [UpdatedOn]           DATETIME      NULL,
    [PanAddUpdateOn]      DATETIME      NULL,
    [UserId]              VARCHAR (100) NULL,
    [IsActive]            BIT           CONSTRAINT [df_IsActive] DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([PanVerificationId] ASC)
);



