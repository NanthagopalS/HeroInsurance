﻿CREATE TYPE [dbo].[POSPMigrationDumpTableType] AS TABLE (
    [posp_code]                     VARCHAR (50)  NULL,
    [name]                          VARCHAR (50)  NULL,
    [alias_name]                    VARCHAR (50)  NULL,
    [email]                         VARCHAR (50)  NULL,
    [mobile]                        VARCHAR (10)  NULL,
    [pan_number]                    VARCHAR (10)  NULL,
    [dob]                           VARCHAR (50)  NULL,
    [father_name]                   VARCHAR (50)  NULL,
    [gender]                        VARCHAR (10)  NULL,
    [adhar_no]                      VARCHAR (50)  NULL,
    [alternate_mobile]              VARCHAR (15)  NULL,
    [address1]                      VARCHAR (200) NULL,
    [address2]                      VARCHAR (200) NULL,
    [pincode]                       VARCHAR (10)  NULL,
    [state]                         VARCHAR (50)  NULL,
    [city]                          VARCHAR (50)  NULL,
    [product_type]                  VARCHAR (50)  NULL,
    [noc_available]                 VARCHAR (10)  NULL,
    [bank_name]                     VARCHAR (100) NULL,
    [ifsc_code]                     VARCHAR (15)  NULL,
    [account_holder_name]           VARCHAR (50)  NULL,
    [account_number]                VARCHAR (50)  NULL,
    [educational_qualification]     VARCHAR (50)  NULL,
    [select_average_premium]        VARCHAR (50)  NULL,
    [POSPBU]                        VARCHAR (50)  NULL,
    [created_date]                  VARCHAR (50)  NULL,
    [created_by]                    VARCHAR (50)  NULL,
    [sourced_by]                    VARCHAR (50)  NULL,
    [serviced_by]                   VARCHAR (50)  NULL,
    [posp_source]                   VARCHAR (50)  NULL,
    [general_training_start]        VARCHAR (50)  NULL,
    [general_training_end]          VARCHAR (50)  NULL,
    [life_insurance_training_start] VARCHAR (50)  NULL,
    [life_insurance_training_end]   VARCHAR (50)  NULL,
    [exam_status]                   VARCHAR (50)  NULL,
    [exam_start]                    VARCHAR (50)  NULL,
    [exam_end]                      VARCHAR (50)  NULL,
    [IIBUploadStatus]               VARCHAR (50)  NULL,
    [iib_upload_date]               VARCHAR (50)  NULL,
    [AgreementStatus]               VARCHAR (50)  NULL);

