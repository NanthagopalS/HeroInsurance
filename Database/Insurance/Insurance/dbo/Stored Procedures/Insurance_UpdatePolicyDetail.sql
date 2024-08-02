-- =============================================      
-- Author:  <Author, Ankit>      
-- Create date: <Create Date,10-October-2023>      
-- Description: <Description,[Insurance_UpdatePolicyDetail]>         
-- ============================================= 
Create   procedure Insurance_UpdatePolicyDetail
(
	@MotorType VARCHAR(100)  
	,@PolicyType VARCHAR(100)  
	,@PolicyCategory VARCHAR(100)  
	,@BasicOD VARCHAR(100)  
	,@BasicTP VARCHAR(100)  
	,@TotalPremium VARCHAR(100)  
	,@NetPremium VARCHAR(100)  
	,@GST VARCHAR(100)  
	,@PolicyNumber VARCHAR(100)  
	,@EngineNumber VARCHAR(100)  
	,@ChassisNumber VARCHAR(100) 
	,@VehicleNumber VARCHAR(100)
	,@RegistrationNo VARCHAR(100)  
	,@IDV VARCHAR(100)  
	,@Insurer VARCHAR(100)  
	,@Make VARCHAR(100)  
	,@Fuel VARCHAR(100)  
	,@Variant VARCHAR(100)  
	,@Month VARCHAR(100)  
	,@LeadName VARCHAR(100)  
	,@CreatedOn VARCHAR(100)
	,@InsurerName VARCHAR(100)
	,@PolicyStartDate VARCHAR(100)  
	,@PolicyEndDate VARCHAR(100)  
	,@BusinessType VARCHAR(100)  
	,@NCBPercentage VARCHAR(100)  
	,@PaymentTxnNumber VARCHAR(100)  
	,@PaymentDate VARCHAR(100)  
	,@PaymentMethod VARCHAR(100)  
	,@Email VARCHAR(100)  
	,@CustomerPhoneNumber VARCHAR(100) 
	,@PolicyNatureTypeName VARCHAR(100)
	,@Year VARCHAR(100)  
	,@PrevPolicyNCB VARCHAR(100)  
	,@CubicCapacity VARCHAR(100)  
	,@RTOCode VARCHAR(100)  
	,@PrevPolicyNumber VARCHAR(100)  
	,@CPA VARCHAR(100)  
	,@Tenure VARCHAR(100)  
	,@InsuranceType VARCHAR(100)  
	,@AddOns VARCHAR(100)  
	,@NilDep VARCHAR(100)  
	,@IsPOSPProduct VARCHAR(100)  
	,@Address1 VARCHAR(1000)  
	,@Address2 VARCHAR(1000)  
	,@Address3 VARCHAR(1000)  
	,@STATE VARCHAR(100)  
	,@City VARCHAR(100)  
	,@PhoneNumber VARCHAR(100)  
	,@PinCode VARCHAR(100)  
	,@DOB VARCHAR(100)  
	,@PANNumber VARCHAR(100)  
	,@GrossDiscount VARCHAR(100)  
	,@GrossPremium VARCHAR(100)  
	,@TotalTP VARCHAR(100)  
	,@GVW VARCHAR(100)  
	,@SeatingCapacity VARCHAR(100)  
	,@CreatedBy VARCHAR(100)  
)
AS  
BEGIN  
 BEGIN TRY  
	
	Update Insurance_LeadDetails 
	Set PolicyNumber = @PolicyNumber, EngineNumber = @EngineNumber, ChassisNumber = @ChassisNumber,
		VehicleNumber = @VehicleNumber, IDV = @IDV,PolicyStartDate = @PolicyStartDate, PolicyEndDate = @PolicyEndDate,
		NCBPercentage = @NCBPercentage, Email = @Email, @PhoneNumber = @PhoneNumber,
		PrevPolicyNCB = PrevPolicyNCB, PrevPolicyNumber = @PrevPolicyNumber, Tenure = @Tenure,
		DOB = @DOB, PANNumber = @PANNumber

	Update Insurance_PaymentTransaction 
	Set CreatedOn = @CreatedOn , PaymentDate = @PaymentDate

	Update Insurance_PremiumDetails
	Set BasicOD = @BasicOD, BasicTP = @BasicTP, TotalTP = @TotalTP, NetPremium = @NetPremium,
		GST = @GST, CPA = @CPA, AddOns = @AddOns, NilDep = @NilDep, GrossPremium = @GrossPremium,
		GrossDiscount = @GrossDiscount

	Update Insurance_ManualLeadDetails 
	Set MotorType = @MotorType, PolicyCategory = @PolicyCategory, Make = @Make, Fuel = @Fuel,
		Variant = @Variant, BusinessType = @BusinessType, PaymentTxnNumber = @PaymentTxnNumber,
		IsPOSPProduct = @IsPOSPProduct, PhoneNumber = @PhoneNumber, GVW = @GVW, 
		SeatingCapacity = @SeatingCapacity

	Update Insurance_LeadAddressDetails 
	Set Address1 = @Address1, Address2 =@Address2, Address3 = @Address3, State = @State,
		City = @City, Pincode = @Pincode

	Update Insurance_Insurer
	Set InsurerName = @InsurerName

	Update Insurance_RTO 
	Set RTOCode = @RTOCode

	Update Insurance_ManualPolicyNatureTypeMaster
	Set PolicyNatureTypeName = @PolicyNatureTypeName

  
 END TRY  
  
 BEGIN CATCH  
  DECLARE @StrProcedure_Name VARCHAR(500)  
   ,@ErrorDetail VARCHAR(1000)  
   ,@ParameterList VARCHAR(2000)  
  
  SET @StrProcedure_Name = ERROR_PROCEDURE()  
  SET @ErrorDetail = ERROR_MESSAGE()  
  
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name  
   ,@ErrorDetail = @ErrorDetail  
   ,@ParameterList = @ParameterList  
 END CATCH  
END