CREATE TABLE [dbo].[Tata_AddonPackageMaster] (
    [Package_Addon_Id] INT          IDENTITY (1, 1) NOT NULL,
    [Package_Id]       VARCHAR (50) NULL,
    [Addon_Id]         VARCHAR (50) NULL,
    [Addon_Code]       VARCHAR (50) NULL,
    [PolicyTypeId]     VARCHAR (50) NULL,
    [IsBrandNew]       BIT          NULL
);

