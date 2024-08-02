﻿CREATE TABLE [dbo].[Insurance_LeadDetails] (
    [LeadId]                      VARCHAR (50)   DEFAULT (newid()) NOT NULL,
    [VehicleTypeId]               VARCHAR (50)   NULL,
    [VehicleNumber]               VARCHAR (50)   NULL,
    [VariantId]                   VARCHAR (50)   NULL,
    [YearId]                      VARCHAR (50)   NULL,
    [LeadName]                    VARCHAR (100)  NULL,
    [PhoneNumber]                 VARCHAR (20)   NULL,
    [Email]                       VARCHAR (100)  NULL,
    [CreatedBy]                   VARCHAR (100)  NOT NULL,
    [CreatedOn]                   DATETIME       DEFAULT (getdate()) NULL,
    [UpdatedBy]                   VARCHAR (100)  NULL,
    [UpdatedOn]                   DATETIME       NULL,
    [QuoteTransactionID]          VARCHAR (50)   NULL,
    [MakeMonthYear]               VARCHAR (20)   NULL,
    [RegistrationDate]            VARCHAR (20)   NULL,
    [CarOwnedBy]                  VARCHAR (20)   NULL,
    [IsPrevPolicy]                BIT            NULL,
    [PrevPolicyNumber]            VARCHAR (50)   NULL,
    [PrevPolicyExpiryDate]        VARCHAR (20)   NULL,
    [PrevPolicyClaims]            VARCHAR (20)   NULL,
    [PrevPolicyNCB]               VARCHAR (50)   NULL,
    [IsPACover]                   BIT            NULL,
    [Tenure]                      INT            NULL,
    [EngineNumber]                VARCHAR (50)   NULL,
    [ChassisNumber]               VARCHAR (50)   NULL,
    [IsOwnershipChangeIn12Months] BIT            NULL,
    [IsExternalCNGKit]            BIT            NULL,
    [IsCarOnLoan]                 BIT            NULL,
    [LoadProvidedCompany]         VARCHAR (50)   NULL,
    [LoadProvidedCity]            VARCHAR (20)   NULL,
    [CompanyName]                 VARCHAR (50)   NULL,
    [DateOfIncorporation]         VARCHAR (30)   NULL,
    [GSTNo]                       NVARCHAR (50)  NULL,
    [MiddleName]                  VARCHAR (20)   NULL,
    [LastName]                    VARCHAR (20)   NULL,
    [DOB]                         VARCHAR (20)   NULL,
    [Gender]                      VARCHAR (10)   NULL,
    [PANNumber]                   VARCHAR (20)   NULL,
    [AadharNumber]                VARCHAR (20)   NULL,
    [Profession]                  VARCHAR (20)   NULL,
    [MaritalStatus]               VARCHAR (1)    NULL,
    [ProposalRequestBody]         VARCHAR (MAX)  NULL,
    [StageId]                     VARCHAR (50)   NULL,
    [InsuranceTypeId]             VARCHAR (100)  NULL,
    [IsActive]                    BIT            DEFAULT ((1)) NULL,
    [PolicyTypeId]                VARCHAR (50)   NULL,
    [PolicyStartDate]             VARCHAR (20)   NULL,
    [PolicyEndDate]               VARCHAR (20)   NULL,
    [PreviousPolicyNumberSAOD]    VARCHAR (50)   NULL,
    [PreviousPolicyExpirySAOD]    VARCHAR (20)   NULL,
    [IsBrandNew]                  BIT            NULL,
    [IsBreakin]                   BIT            NULL,
    [IsBreakinApproved]           BIT            NULL,
    [PaymentLink]                 NVARCHAR (MAX) NULL,
    [PolicyNumber]                VARCHAR (100)  NULL,
    [TotalPremium]                VARCHAR (50)   NULL,
    [GrossPremium]                VARCHAR (50)   NULL,
    [Tax]                         NVARCHAR (MAX) NULL,
    [NCBPercentage]               VARCHAR (50)   NULL,
    [InsurerId]                   VARCHAR (50)   NULL,
    [IDV]                         VARCHAR (50)   NULL,
    [MinIDV]                      VARCHAR (50)   NULL,
    [MaxIDV]                      VARCHAR (50)   NULL,
    [IsSelfInspection]            BIT            NULL,
    [isPolicyExpired]             BIT            NULL,
    [SelectedIDV]                 VARCHAR (100)  NULL,
    [RTOId]                       VARCHAR (100)  NULL,
    [PreviousSAODInsurer]         VARCHAR (100)  NULL,
    [PreviousSATPInsurer]         VARCHAR (100)  NULL,
    [PrevPolicyTypeId]            VARCHAR (50)   NULL,
    [IsQuoteDeviation]            BIT            NULL,
    [IsApprovalRequired]          BIT            NULL,
    [BreakinId]                   VARCHAR (50)   NULL,
    [PreviousSAODPolicyStartDate] VARCHAR (20)   NULL,
    [PreviousSATPPolicyStartDate] VARCHAR (20)   NULL,
    [RefLeadId]                   VARCHAR (50)   NULL,
    [BreakinInspectionDate]       VARCHAR (50)   NULL,
    [InspectionAgency]            VARCHAR (50)   NULL,
    [BreakinInspectionURL]        NVARCHAR (MAX) NULL,
    [ResponseReferanceFlag]       VARCHAR (10)   NULL,
    [IsManualPolicy]              BIT            DEFAULT ((0)) NULL,
    [PolicySource]                BIT            DEFAULT ((0)) NULL,
    [Salutation]                  VARCHAR (50)   NULL,
    PRIMARY KEY CLUSTERED ([LeadId] ASC)
);









