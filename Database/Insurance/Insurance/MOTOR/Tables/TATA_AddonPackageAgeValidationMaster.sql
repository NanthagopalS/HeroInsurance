CREATE TABLE [MOTOR].[TATA_AddonPackageAgeValidationMaster] (
    [PackageName]           VARCHAR (50) NULL,
    [PackageFlag]           VARCHAR (50) NULL,
    [IsNCBZero]             BIT          NULL,
    [PackageValidityInDays] INT          NULL,
    [IsActive]              BIT          NULL,
    [CreatedBy]             VARCHAR (50) NULL,
    [CreatedOn]             DATETIME     NULL,
    [UpdatedBy]             VARCHAR (50) NULL,
    [UpdatedOn]             DATETIME     NULL
);

