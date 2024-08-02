CREATE TABLE [dbo].[Insurance_Fuel] (
    [FuelId]    VARCHAR (50)  NULL,
    [FuelName]  VARCHAR (100) NULL,
    [CreatedBy] VARCHAR (50)  NULL,
    [CreatedOn] DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50)  NULL,
    [UpdatedOn] DATETIME      NULL
);

