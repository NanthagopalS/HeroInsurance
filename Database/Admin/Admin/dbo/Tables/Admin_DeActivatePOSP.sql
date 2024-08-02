CREATE TABLE [dbo].[Admin_DeActivatePOSP] (
    [Id]                                       VARCHAR (100) CONSTRAINT [DF_Admin_DeActivatePOSP_Id] DEFAULT (newid()) NOT NULL,
    [DeActivatePospId]                         VARCHAR (100) NULL,
    [RelationshipManagerid]                    VARCHAR (100) NULL,
    [PolicyType]                               VARCHAR (100) NULL,
    [Status]                                   VARCHAR (100) NULL,
    [EmailAttachmentDocumentId]                VARCHAR (100) NULL,
    [BusinessTeamApprovalAttachmentDocumentId] VARCHAR (100) NULL,
    [Remark]                                   VARCHAR (100) NULL,
    [IsNocGenerated]                           BIT           NULL,
    [IsActive]                                 BIT           CONSTRAINT [DF_Admin_DeActivatePOSP_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]                                VARCHAR (50)  NULL,
    [CreatedOn]                                DATETIME      CONSTRAINT [DF_Admin_DeActivatePOSP_CreatedOn] DEFAULT (getdate()) NULL,
    [UpdatedBy]                                VARCHAR (50)  NULL,
    [UpdatedOn]                                DATETIME      NULL,
    CONSTRAINT [PK_Admin_DeActivatePOSP_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

