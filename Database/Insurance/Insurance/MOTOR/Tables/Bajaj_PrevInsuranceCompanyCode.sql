CREATE TABLE [MOTOR].[Bajaj_PrevInsuranceCompanyCode] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [CompanyName]       VARCHAR (100) NULL,
    [CompanyCode]       VARCHAR (10)  NULL,
    [IsActive]          VARCHAR (50)  NULL,
    [CreatedOn]         DATETIME      NULL,
    [UpdatedOn]         DATETIME      NULL,
    [PreviousInsurerId] VARCHAR (50)  NULL
);

