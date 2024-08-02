CREATE TABLE [dbo].[Insurance_LeadNomineeDetails] (
    [NomineeID]    VARCHAR (50) DEFAULT (newid()) NULL,
    [LeadID]       VARCHAR (50) NULL,
    [FirstName]    VARCHAR (50) NULL,
    [LastName]     VARCHAR (50) NULL,
    [DOB]          VARCHAR (20) NULL,
    [Age]          INT          NULL,
    [Relationship] VARCHAR (20) NULL,
    [CreatedBy]    VARCHAR (50) NULL,
    [CreatedOn]    DATETIME     DEFAULT (getdate()) NULL,
    [UpdatedBy]    VARCHAR (50) NULL,
    [UpdatedOn]    DATETIME     NULL
);

