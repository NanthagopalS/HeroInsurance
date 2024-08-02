CREATE TABLE [MOTOR].[Chola_PreviousInsurerMaster] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [ShortName]   VARCHAR (100)  NULL,
    [CompanyName] NVARCHAR (MAX) NULL,
    [CityName]    VARCHAR (100)  NULL,
    [DisplayName] NVARCHAR (MAX) NULL,
    [CreatedBy]   VARCHAR (50)   DEFAULT ((1)) NULL,
    [CreatedOn]   DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50)   NULL,
    [UpdatedOn]   DATETIME       NULL,
    [InsurerId]   VARCHAR (50)   NULL
);

