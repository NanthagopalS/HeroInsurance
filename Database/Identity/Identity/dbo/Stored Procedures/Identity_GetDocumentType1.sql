-- =============================================        
-- Author:  <Author,HARSH PATEL>        
-- Create date: <Create Date,03-12-2022>        
-- Description: <Description,GetDocumentType>        
-- =============================================        
CREATE    PROCEDURE [dbo].[Identity_GetDocumentType1]     
(  
 @UserId VARCHAR(100)  
)  
AS        
BEGIN        
 BEGIN TRY        
  --SELECT Id,DocumentType,IsMandatory,ShortDescription FROM Identity_DocumentTypeMaster        
  Declare @isAlias bit , @check VARCHAR(100)  
 SEt @check = (Select AliasName from HeroIdentity.dbo.Identity_UserDetail WITH(NOLOCK) where UserId = @UserId)  
 if (@check != '' or @check != null)  
 BEGIN  
  Set @isAlias = 1  
 END  
 else  
 BEGIN  
  Set @isAlias = 0  
 END  
  
 print @isAlias  
    
 IF OBJECT_ID('#UserDocument') IS NOT NULL  
 DROP TABLE #UserDocument;  
 select * into #UserDocument from [HeroIdentity].[dbo].[Identity_DocumentTypeMaster] WITH(NOLOCK) ;  
 update #UserDocument SET IsMandatory =      
 CASE WHEN (DocumentType= 'POSP Declaration / Affidavit' and @isAlias = 1) THEN 1   
 WHEN (DocumentType = 'POSP Declaration / Affidavit' and @isAlias = 0) THEN 0   
 ELSE IsMandatory  
 END   
 select * from #UserDocument;  
 DROP TABLE #UserDocument;  
  --SELECT * FROM [dbo].[Identity_DocumentTypeMaster] ORDER BY DocumentTypeId ASC  
 --Select isnull(aliasname,0) from HeroIdentity.dbo.Identity_UserDetail where UserId='017776C0-8769-4A4B-BA36-90761B2B8F69'  
   
 END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
END