CREATE TABLE [dbo].[Identity_UserAddressDetail] (
    [Id]           VARCHAR (100) CONSTRAINT [DF_Identity_UserAddressDetail_Id] DEFAULT (newid()) NOT NULL,
    [UserId]       VARCHAR (100) NOT NULL,
    [AddressLine1] VARCHAR (200) NULL,
    [AddressLine2] VARCHAR (200) NULL,
    [Pincode]      INT           NULL,
    [CityId]       VARCHAR (100) NULL,
    [StateId]      VARCHAR (100) NULL,
    [IsActive]     BIT           CONSTRAINT [DF_Identity_UserAddressDetail_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]    VARCHAR (50)  NULL,
    [CreatedOn]    DATETIME      CONSTRAINT [DF_Identity_UserAddressDetail_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]    VARCHAR (50)  NULL,
    [UpdatedOn]    DATETIME      NULL,
    CONSTRAINT [PK_Identity_UserAddressDetail_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

