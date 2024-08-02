CREATE TABLE [MOTOR].[HDFC_VehicleMaster] (
    [MANUFACTURER]       VARCHAR (100) NULL,
    [VEHICLEMODELCODE]   VARCHAR (100) NULL,
    [VEHICLEMODEL]       VARCHAR (100) NULL,
    [NUMBEROFWHEELS]     VARCHAR (10)  NULL,
    [CUBICCAPACITY]      VARCHAR (10)  NULL,
    [GROSSVEHICLEWEIGHT] VARCHAR (10)  NULL,
    [SEATINGCAPACITY]    VARCHAR (10)  NULL,
    [CARRYINGCAPACITY]   VARCHAR (10)  NULL,
    [TXT_FUEL]           VARCHAR (10)  NULL,
    [TXT_VARIANT]        VARCHAR (100) NULL,
    [VariantId]          VARCHAR (50)  NULL,
    [CreatedOn]          DATETIME      NULL,
    [UpdatedBy]          VARCHAR (50)  NULL,
    [UpdatedOn]          DATETIME      NULL,
    [IsCommercial]       BIT           CONSTRAINT [VM_IsCom] DEFAULT ((0)) NULL,
    [IsManuallyMapped]   BIT           NULL
);







