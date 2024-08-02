CREATE TABLE [dbo].[Insurance_AddOns] (
    [AddOnId]       VARCHAR (50)  CONSTRAINT [DF__Insurance__AddOn__2DB1C7EE] DEFAULT (newid()) NOT NULL,
    [AddOns]        VARCHAR (100) NULL,
    [IsRecommended] BIT           NULL,
    [Description]   VARCHAR (MAX) NULL,
    [IsActive]      BIT           CONSTRAINT [DF_Insurance_AddOns_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (100) NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_Insurance_AddOns_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (100) NULL,
    [UpdatedOn]     DATETIME      NULL,
    [AddOnCode]     VARCHAR (50)  NULL,
    [VehicleTypeId] VARCHAR (50)  NULL,
    [PolicyTypeId]  VARCHAR (50)  NULL,
    CONSTRAINT [PK_Insurance_AddOns] PRIMARY KEY CLUSTERED ([AddOnId] ASC)
);

