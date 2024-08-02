CREATE TABLE [dbo].[Insurance_PreviousPolicyType] (
    [PreviousPolicyTypeId] VARCHAR (50) DEFAULT (newid()) NOT NULL,
    [PreviousPolicyType]   VARCHAR (80) NULL,
    [CreatedBy]            VARCHAR (50) NULL,
    [CreatedOn]            DATETIME     DEFAULT (getdate()) NULL,
    [UpdatedBy]            VARCHAR (50) NULL,
    [UpdatedOn]            DATETIME     NULL,
    [IsActive]             BIT          NULL,
    PRIMARY KEY CLUSTERED ([PreviousPolicyTypeId] ASC)
);

