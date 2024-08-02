CREATE TABLE [dbo].[Insurance_LeadAddressDetails] (
    [AddressID]   VARCHAR (50)  DEFAULT (newid()) NULL,
    [LeadID]      VARCHAR (50)  NULL,
    [AddressType] VARCHAR (50)  NULL,
    [Address1]    VARCHAR (200) NULL,
    [Address2]    VARCHAR (200) NULL,
    [Address3]    VARCHAR (200) NULL,
    [Pincode]     VARCHAR (8)   NULL,
    [CreatedBy]   VARCHAR (50)  NULL,
    [CreatedOn]   DATETIME      DEFAULT (getdate()) NULL,
    [UpdatedBy]   VARCHAR (50)  NULL,
    [UpdatedOn]   DATETIME      NULL,
    [City]        VARCHAR (100) NULL,
    [State]       VARCHAR (100) NULL,
    [Country]     VARCHAR (100) NULL
);

