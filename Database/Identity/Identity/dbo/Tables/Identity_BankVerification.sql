﻿CREATE TABLE [dbo].[Identity_BankVerification] (
    [BankVerificationId] VARCHAR (50)  DEFAULT (newid()) NOT NULL,
    [Task]               VARCHAR (100) NOT NULL,
    [Id]                 VARCHAR (100) NOT NULL,
    [PatronId]           VARCHAR (100) NOT NULL,
    [Active]             VARCHAR (100) NOT NULL,
    [Reason]             VARCHAR (100) NOT NULL,
    [NameMatch]          VARCHAR (100) NOT NULL,
    [MobileMatch]        VARCHAR (100) NOT NULL,
    [SignzyReferenceId]  VARCHAR (100) NOT NULL,
    [NameMatchScore]     VARCHAR (100) NOT NULL,
    [Nature]             VARCHAR (100) NOT NULL,
    [Value]              VARCHAR (100) NOT NULL,
    [Timestamp]          VARCHAR (100) NOT NULL,
    [Response]           VARCHAR (100) NOT NULL,
    [BankRRN]            VARCHAR (100) NOT NULL,
    [BeneName]           VARCHAR (100) NOT NULL,
    [BeneMMID]           VARCHAR (100) NOT NULL,
    [BeneMobile]         VARCHAR (100) NOT NULL,
    [BeneIFSC]           VARCHAR (100) NOT NULL,
    [CreatedBy]          VARCHAR (100) NOT NULL,
    [CreatedOn]          DATETIME      NOT NULL,
    [UpdatedBy]          VARCHAR (100) NULL,
    [UpdatedOn]          DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([BankVerificationId] ASC)
);
