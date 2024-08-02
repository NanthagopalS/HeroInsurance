CREATE TABLE [dbo].[Insurance_VehicleType] (
    [VehicleTypeId]   VARCHAR (50)  NULL,
    [VehicleType]     VARCHAR (100) NULL,
    [CreatedBy]       VARCHAR (50)  NULL,
    [CreatedOn]       DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]       VARCHAR (50)  NULL,
    [UpdatedOn]       DATETIME      NULL,
    [InsuranceTypeId] VARCHAR (50)  NULL
);

