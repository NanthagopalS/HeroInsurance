﻿CREATE TABLE [dbo].[Insurance_VehicleRegistration] (
    [vehicleId]                     VARCHAR (50)   CONSTRAINT [DF_Insurance_VehicleRegistration_vehicleId] DEFAULT (newid()) NOT NULL,
    [regNo]                         VARCHAR (25)   NULL,
    [class]                         VARCHAR (25)   NULL,
    [chassis]                       VARCHAR (30)   NULL,
    [engine]                        VARCHAR (50)   NULL,
    [vehicleManufacturerName]       VARCHAR (100)  NULL,
    [model]                         VARCHAR (25)   NULL,
    [vehicleColour]                 VARCHAR (25)   NULL,
    [type]                          VARCHAR (30)   NULL,
    [normsType]                     VARCHAR (30)   NULL,
    [bodyType]                      VARCHAR (30)   NULL,
    [ownerCount]                    VARCHAR (25)   NULL,
    [owner]                         VARCHAR (30)   NULL,
    [ownerFatherName]               VARCHAR (30)   NULL,
    [mobileNumber]                  VARCHAR (15)   NULL,
    [status]                        VARCHAR (15)   NULL,
    [statusAsOn]                    VARCHAR (30)   NULL,
    [regAuthority]                  VARCHAR (30)   NULL,
    [regDate]                       VARCHAR (15)   NULL,
    [vehicleManufacturingMonthYear] VARCHAR (15)   NULL,
    [rcExpiryDate]                  VARCHAR (15)   NULL,
    [vehicleTaxUpto]                VARCHAR (15)   NULL,
    [vehicleInsuranceCompanyName]   VARCHAR (50)   NULL,
    [vehicleInsuranceUpto]          VARCHAR (15)   NULL,
    [vehicleInsurancePolicyNumber]  VARCHAR (30)   NULL,
    [rcFinancer]                    VARCHAR (50)   NULL,
    [presentAddress]                VARCHAR (500)  NULL,
    [splitPresentAddress]           VARCHAR (2000) NULL,
    [permanentAddress]              VARCHAR (500)  NULL,
    [splitPermanentAddress]         VARCHAR (2000) NULL,
    [vehicleCubicCapacity]          VARCHAR (10)   NULL,
    [grossVehicleWeight]            VARCHAR (10)   NULL,
    [unladenWeight]                 VARCHAR (10)   NULL,
    [vehicleCategory]               VARCHAR (25)   NULL,
    [rcStandardCap]                 VARCHAR (10)   NULL,
    [vehicleCylindersNo]            VARCHAR (10)   NULL,
    [vehicleSeatCapacity]           VARCHAR (10)   NULL,
    [vehicleSleeperCapacity]        VARCHAR (10)   NULL,
    [vehicleStandingCapacity]       VARCHAR (10)   NULL,
    [wheelbase]                     VARCHAR (10)   NULL,
    [vehicleNumber]                 VARCHAR (15)   NULL,
    [puccNumber]                    VARCHAR (15)   NULL,
    [puccUpto]                      VARCHAR (15)   NULL,
    [blacklistStatus]               VARCHAR (15)   NULL,
    [blacklistDetails]              VARCHAR (2000) NULL,
    [challanDetails]                VARCHAR (2000) NULL,
    [permitIssueDate]               VARCHAR (15)   NULL,
    [permitNumber]                  VARCHAR (15)   NULL,
    [permitType]                    VARCHAR (30)   NULL,
    [permitValidFrom]               VARCHAR (50)   NULL,
    [permitValidUpto]               VARCHAR (15)   NULL,
    [nonUseStatus]                  VARCHAR (25)   NULL,
    [nonUseFrom]                    VARCHAR (25)   NULL,
    [nonUseTo]                      VARCHAR (25)   NULL,
    [nationalPermitNumber]          VARCHAR (15)   NULL,
    [nationalPermitUpto]            VARCHAR (15)   NULL,
    [nationalPermitIssuedBy]        VARCHAR (50)   NULL,
    [isCommercial]                  VARCHAR (10)   NULL,
    [nocDetails]                    VARCHAR (50)   NULL,
    [CreatedBy]                     VARCHAR (50)   NULL,
    [CreatedOn]                     DATETIME       NULL,
    [UpdatedBy]                     VARCHAR (50)   NULL,
    [UpdatedOn]                     DATETIME       NULL,
    [VehicleAddUpdateOn]            DATETIME       NULL,
    [VariantId]                     VARCHAR (50)   NULL,
    [VehicleTypeId]                 VARCHAR (100)  NULL,
    CONSTRAINT [PK__Insuranc__5B9D25F2C6F5A63C] PRIMARY KEY CLUSTERED ([vehicleId] ASC)
);

