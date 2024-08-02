CREATE TABLE [MOTOR].[Bajaj_RTOCityMaster] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [State]             VARCHAR (100) NULL,
    [RegistractionCity] VARCHAR (100) NULL,
    [Zone]              CHAR (2)      NULL,
    [IsActive]          VARCHAR (50)  NULL,
    [CreatedOn]         DATETIME      NULL,
    [UpdatedOn]         DATETIME      NULL,
    [RTOId]             VARCHAR (50)  NULL
);

