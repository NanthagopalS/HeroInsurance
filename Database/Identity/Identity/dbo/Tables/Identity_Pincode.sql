CREATE TABLE [dbo].[Identity_Pincode] (
    [PincodeId] VARCHAR (50) DEFAULT (newid()) NOT NULL,
    [Pincode]   VARCHAR (10) NOT NULL,
    [StateName] VARCHAR (50) NULL,
    [StateId]   VARCHAR (50) NULL,
    [CreatedBy] VARCHAR (50) NULL,
    [CreatedOn] DATETIME     DEFAULT (getdate()) NULL,
    [UpdatedBy] VARCHAR (50) NULL,
    [UpdatedOn] DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([PincodeId] ASC)
);

