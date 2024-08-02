
 CREATE PROCEDURE [dbo].[Identity_GetCardDetails]   
    
  @UserId VARCHAR(100)  
   
AS  
  
BEGIN  
 BEGIN TRY  
	Declare @PremiumCollection int,
			@Card Varchar(50),
			@POSPId Varchar(100)

	Set @PremiumCollection = (select sum(convert(int, TotalPremium)) from [HeroInsurance].[dbo].[Insurance_LeadDetails] 
	where CreatedBy = @UserId
	group by CreatedBy)

	if @PremiumCollection <= 500000
	BEGIN
		Set @Card = 'Bronze'
	END

	else if (500000 < @PremiumCollection AND @PremiumCollection <= 999999)
	BEGIN
		Set @Card = 'Silver'
	END
	
	else if (1000000 < @PremiumCollection AND @PremiumCollection <= 4999999)
	BEGIN
		Set @Card = 'Gold'
	END

	else
	BEGIN
		Set @Card = 'Platinum'
	END

	Set @POSPId = (Select POSPId from Identity_User where UserId = @UserId)

	Select @PremiumCollection, @Card, @POSPId

 END TRY                  
 BEGIN CATCH    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END