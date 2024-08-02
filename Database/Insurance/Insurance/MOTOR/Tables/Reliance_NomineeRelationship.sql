CREATE TABLE [MOTOR].[Reliance_NomineeRelationship] (
    [Id]        VARCHAR (20) NULL,
    [Text]      VARCHAR (20) NULL,
    [CreatedBy] VARCHAR (50) DEFAULT ((1)) NULL,
    [CreatedOn] DATETIME     DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50) NULL,
    [UpdatedOn] DATETIME     NULL
);

