CREATE TABLE [dbo].[Insurance_GetPreviousCoverMaster] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [InsurerId]     NVARCHAR (50)  NULL,
    [VehicalTypeId] NVARCHAR (50)  NULL,
    [PolicyTypeId]  NVARCHAR (50)  NULL,
    [CoverId]       NVARCHAR (50)  NULL,
    [CoverName]     NVARCHAR (50)  NULL,
    [CoverCode]     NVARCHAR (50)  NULL,
    [CoverText]     NVARCHAR (100) NULL,
    [CoverFlag]     NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



