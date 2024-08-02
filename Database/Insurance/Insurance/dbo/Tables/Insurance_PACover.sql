CREATE TABLE [dbo].[Insurance_PACover] (
    [PACoverId]     VARCHAR (50)  CONSTRAINT [DF__Insurance__PACov__3552E9B6] DEFAULT (newid()) NOT NULL,
    [CoverName]     VARCHAR (100) NULL,
    [IsActive]      BIT           CONSTRAINT [DF_Insurance_PACover_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     VARCHAR (100) NULL,
    [CreatedOn]     DATETIME      CONSTRAINT [DF_Insurance_PACover_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]     VARCHAR (100) NULL,
    [UpdatedOn]     DATETIME      NULL,
    [PACoverCode]   VARCHAR (50)  NULL,
    [VehicleTypeId] VARCHAR (50)  NULL,
    [PolicyTypeId]  VARCHAR (50)  NULL,
    [IsDefault]     BIT           NULL,
    CONSTRAINT [PK_Insurance_PACover] PRIMARY KEY CLUSTERED ([PACoverId] ASC)
);

