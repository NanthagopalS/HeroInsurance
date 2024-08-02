CREATE TABLE [dbo].[Insurance_PolicyMigrationExcelInsurerMapping] (
    [Id]               VARCHAR (50)  CONSTRAINT [DF_Insurance_PolicyMigrationExcelInsurerMapping_Id] DEFAULT (newid()) NULL,
    [excelInsurerName] VARCHAR (150) NULL,
    [DBInsurerId]      VARCHAR (50)  NULL
);

