CREATE TABLE [dbo].[Insurance_ManualPolicyNatureTypeMaster] (
    [PolicyNatureTypeId]   VARCHAR (50) CONSTRAINT [DF_Table_1_PolicyNatureType] DEFAULT (newid()) NULL,
    [PolicyNatureTypeName] VARCHAR (50) NULL,
    [IsActive]             BIT          CONSTRAINT [DF_Insurance_ManualPolicyNatureTypeMaster_IsActive] DEFAULT ((1)) NULL,
    [CreatedOn]            VARCHAR (50) CONSTRAINT [DF_Insurance_ManualPolicyNatureTypeMaster_CreatedOn] DEFAULT (getdate()) NULL,
    [CreatedBy]            VARCHAR (50) NULL
);

