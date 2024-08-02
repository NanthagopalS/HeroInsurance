CREATE TABLE [dbo].[Insurance_Gender] (
    [GenderId]    INT          IDENTITY (1, 1) NOT NULL,
    [GenderName]  VARCHAR (10) NULL,
    [GenderValue] VARCHAR (10) NULL,
    [IsActive]    BIT          NULL,
    [CreatedBy]   INT          NULL,
    [CreatedOn]   DATETIME     DEFAULT (getdate()) NULL,
    [UpdatedBy]   INT          NULL,
    [UpdatedOn]   DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([GenderId] ASC)
);

