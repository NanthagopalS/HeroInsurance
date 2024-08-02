CREATE TABLE [dbo].[Insurance_ManualLeadDetails] (
    [ManualLeadId]     VARCHAR (50)  CONSTRAINT [DF_Insurance_ManualLeadDetails_ManualLeadId] DEFAULT (newid()) NOT NULL,
    [LeadId]           VARCHAR (50)  NOT NULL,
    [MotorType]        VARCHAR (100) NULL,
    [PolicyType]       VARCHAR (100) NULL,
    [ServiceTax]       FLOAT (53)    NULL,
    [Fuel]             VARCHAR (100) NULL,
    [Variant]          VARCHAR (100) NULL,
    [BusinessType]     VARCHAR (100) NULL,
    [CubicCapacity]    FLOAT (53)    NULL,
    [InsuranceType]    VARCHAR (100) NULL,
    [IsPOSPProduct]    VARCHAR (10)  NULL,
    [GVW]              FLOAT (53)    NULL,
    [SeatingCapacity]  INT           NULL,
    [PolicyCategory]   VARCHAR (100) NULL,
    [CreatedOn]        DATETIME      CONSTRAINT [DF_Insurance_ManualLeadDetails_CreatedOn] DEFAULT (getdate()) NULL,
    [IsActive]         BIT           CONSTRAINT [DF_Insurance_ManualLeadDetails_IsActive] DEFAULT ((1)) NULL,
    [PaymentMethod]    VARCHAR (100) NULL,
    [PaymentTxnNumber] VARCHAR (100) NULL,
    [PaymentMode]      VARCHAR (20)  NULL,
    [Make]             VARCHAR (100) NULL,
    [PhoneNumber]      VARCHAR (50)  NULL
);



