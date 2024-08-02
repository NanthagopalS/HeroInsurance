CREATE TABLE [MOTOR].[Reliance_PreviousInsurerMaster] (
    [InsuranceCompanyID]   VARCHAR (20)   NULL,
    [InsuranceCompanyName] NVARCHAR (MAX) NULL,
    [CreatedBy]            VARCHAR (50)   DEFAULT ((1)) NULL,
    [CreatedOn]            DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]            VARCHAR (50)   NULL,
    [UpdatedOn]            DATETIME       NULL, 
    [InsurerId]            VARCHAR (50)   NULL
);

