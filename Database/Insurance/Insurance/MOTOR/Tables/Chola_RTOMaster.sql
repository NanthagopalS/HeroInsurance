CREATE TABLE [MOTOR].[Chola_RTOMaster] (
    [NumCountryCode]           VARCHAR (10)  NULL,
    [NumStateCode]             VARCHAR (10)  NULL,
    [StateName]                VARCHAR (50)  NULL,
    [NumCityDistrictCode]      VARCHAR (10)  NULL,
    [DistrictName]             VARCHAR (100) NULL,
    [TXTRTOLocationCode]       VARCHAR (20)  NULL,
    [Alternate_RTO]            VARCHAR (20)  NULL,
    [TXTRTOLocationDesc]       VARCHAR (100) NULL,
    [TXTRegistrationStateCode] VARCHAR (10)  NULL,
    [NumRegistrationRTOCode]   VARCHAR (10)  NULL,
    [NumVehicleClassCode]      VARCHAR (10)  NULL,
    [TxtRegistrationZone]      VARCHAR (5)   NULL,
    [TXTStatus]                VARCHAR (20)  NULL,
    [ActiveFlag]               VARCHAR (5)   NULL,
    [RTOId]                    VARCHAR (50)  NULL,
    [CreatedBy]                VARCHAR (50)  DEFAULT ((1)) NULL,
    [CreatedOn]                DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]                VARCHAR (50)  NULL,
    [UpdatedOn]                DATETIME      NULL,
    [RTOCode]                  VARCHAR (50)  NULL
);



