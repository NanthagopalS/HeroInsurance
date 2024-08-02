CREATE TABLE [MOTOR].[Reliance_Salutation] (
    [Code]        VARCHAR (10) NULL,
    [Description] VARCHAR (10) NULL,
    [CreatedBy]   VARCHAR (50) DEFAULT ((1)) NULL,
    [CreatedOn]   DATETIME     DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50) NULL,
    [UpdatedOn]   DATETIME     NULL
);

