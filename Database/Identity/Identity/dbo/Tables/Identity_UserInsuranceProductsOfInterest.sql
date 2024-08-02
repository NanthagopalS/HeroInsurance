CREATE TABLE [dbo].[Identity_UserInsuranceProductsOfInterest] (
    [Id]                            VARCHAR (100) CONSTRAINT [DF_Identity_UserInsuranceProductsOfInterest_Id] DEFAULT (newid()) NOT NULL,
    [UserId]                        VARCHAR (100) NULL,
    [InsuranceProductsOfInterestId] VARCHAR (100) NULL,
    [IsActive]                      BIT           CONSTRAINT [DF_Identity_UserInsuranceProductsOfInterest_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]                     VARCHAR (50)  NULL,
    [CreatedOn]                     DATETIME      CONSTRAINT [DF_Identity_UserInsuranceProductsOfInterest_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]                     VARCHAR (50)  NULL,
    [UpdatedOn]                     DATETIME      NULL,
    CONSTRAINT [PK_Identity_UserInsuranceProductsOfInterest_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

