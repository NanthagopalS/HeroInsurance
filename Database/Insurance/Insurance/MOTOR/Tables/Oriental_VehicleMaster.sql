CREATE TABLE [MOTOR].[Oriental_VehicleMaster] (
    [VEH_MAKE]           VARCHAR (20)  NULL,
    [VEH_MAKE_DESC]      VARCHAR (20)  NULL,
    [VEH_MODEL]          VARCHAR (20)  NULL,
    [VEH_MODEL_DESC]     VARCHAR (20)  NULL,
    [POOL_CODE]          VARCHAR (20)  NULL,
    [TAC_CODE]           VARCHAR (20)  NULL,
    [VEH_BODY]           VARCHAR (10)  NULL,
    [VEH_BODY_DESC]      VARCHAR (20)  NULL,
    [VEH_CC]             VARCHAR (20)  NULL,
    [VEH_FUEL]           VARCHAR (10)  NULL,
    [VEH_FUEL_DESC]      VARCHAR (20)  NULL,
    [VEH_GVW]            VARCHAR (20)  NULL,
    [VEH_SEAT_CAP]       VARCHAR (10)  NULL,
    [VEH_NO_DRIVER]      VARCHAR (10)  NULL,
    [VAP_PROD_CODE]      VARCHAR (20)  NULL,
    [VAP_PROD_CODE_DESC] VARCHAR (100) NULL,
    [VEH_EFF_FM_DT]      VARCHAR (20)  NULL,
    [VEH_EFF_TO_DT]      VARCHAR (20)  NULL,
    [DISC_UPTO_5YRS]     VARCHAR (10)  NULL,
    [DISC_5_TO_10YRS]    VARCHAR (10)  NULL,
    [CreatedBy]          VARCHAR (50)  NULL,
    [CreatedOn]          DATETIME      NULL,
    [UpdatedBy]          VARCHAR (50)  NULL,
    [UpdatedOn]          DATETIME      NULL,
    [VariantId]          VARCHAR (50)  NULL,
    [IsManuallyMapped]   BIT           NULL
);



