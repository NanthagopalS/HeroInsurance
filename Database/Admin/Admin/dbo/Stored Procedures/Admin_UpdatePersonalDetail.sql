            
-- =============================================            
-- Author:  <Author,Ankit Ghosh>            
-- Create date: <Create Date,2- March-2023>            
-- Description: <Description,,UPDATE PERSONAL DETAIL>            
-- =============================================            
CREATE     PROCEDURE [dbo].[Admin_UpdatePersonalDetail]             
(         
 @UserId Varchar (100),      
 @AlternateNumber Varchar(10),      
 @AddressLine1 VARCHAR(400),      
 @AddressLine2 VARCHAR(400),      
 @Pincode Int,      
 @City VARCHAR(100),      
 @State VARCHAR(100),      
 @EducationalQualification VARCHAR(100),      
 @InsuranceProductsofInterestTypeId VARCHAR(100),      
 @ProductCategoryId Varchar(100),      
 @POSPSource bit,      
 @SellingExperience VARCHAR(100),      
 @ICName VARCHAR(100),      
 @PremiumSold VARCHAR(100),      
 @IsSelling VARCHAR(10),      
 @NOCAvailable bit      
)            
AS         
--Declare @BankName1 VARCHAR(100) = @BankName,       
       --@EducationQualifications VARCHAR(100) = @EducationalQualification      
      
BEGIN            
 BEGIN TRY                  
 --SET @BankName1 = (Select IBN.BankName from [HeroIdentity].[dbo].[Identity_BankNameMaster] as IBN      
      --Inner join [HeroIdentity].[dbo].[Identity_UserBankDetail] IUB on IUB.BankId = IBN.Id )      
      
 --SET @EducationQualifications = (Select IBM.EducationQualificationType from [HeroIdentity].[dbo].[Identity_EducationQualificationTypeMaster] as IBM      
      --Inner join [HeroIdentity].[dbo].[Identity_UserDetail] IUD on IUD.EducationQualificationTypeId = IBM.Id)           
            
      
         
  BEGIN            
   Update [HeroIdentity].[dbo].[Identity_UserAddressDetail]      
   Set AddressLine1 = @AddressLine1, AddressLine2 = @AddressLine2, Pincode = @Pincode, CityId = @City, StateId = @State , CreatedOn= GETDATE()     
   where UserId = @UserId      
      
   Update [HeroIdentity].[dbo].[Identity_UserDetail]      
   Set  EducationQualificationTypeId = @EducationalQualification, ServicedByUserId = @ICName, IsSelling = @IsSelling,      
   InsuranceSellingExperienceRangeId = @SellingExperience, AlternateContactNo = @AlternateNumber, NOCAvailable = @NOCAvailable , CreatedOn= GETDATE()    
   where UserId = @UserId      
   
 Update [HeroAdmin].[dbo].[Admin_UserRoleMapping]      
 set CategoryId = @ProductCategoryId, CreatedOn= GETDATE()      
 where UserId = @UserId      
      
  END            
        
          
 END TRY                            
 BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList            
              
 END CATCH            
            
END 
