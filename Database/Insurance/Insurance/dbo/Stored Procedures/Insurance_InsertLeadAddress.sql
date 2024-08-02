Create procedure Insurance_InsertLeadAddress
@LeadID varchar(50),
@ComAddress1 varchar(200), 
@ComAddress2 varchar(200), 
@ComAddress3 varchar(200),
@ComPincode varchar(8),
@PermAddress1 varchar(200), 
@PermAddress2 varchar(200), 
@PermAddress3 varchar(200),
@PermPincode varchar(8)
as
begin

begin try
	
	Merge Insurance_LeadAddressDetails as tar
	using(
		select @LeadID LeadID, @ComAddress1 Address1, @ComAddress2 Address2, @ComAddress3 Address3, @ComPincode Pincode
	) as src
	on tar.LeadID=src.LeadID and tar.AddressType='C'
	when matched then update set
		tar.Address1=src.Address1,
		tar.Address2=src.Address2,
		tar.Address3=src.Address3,
		tar.Pincode=src.Pincode
	when not matched then 
		insert (LeadID,AddressType,Address1,Address2,Address3,Pincode)
		values(LeadID,'C',src.Address1,src.Address2,src.Address3,src.Pincode)
		;

	Merge Insurance_LeadAddressDetails as tar
	using(
		select @LeadID LeadID, @PermAddress1 Address1, @PermAddress2 Address2, @PermAddress3 Address3, @PermPincode Pincode
	) src
	on tar.LeadID=src.LeadID and AddressType='P'
	when matched then update set
		tar.Address1=src.Address1,
		tar.Address2=src.Address2,
		tar.Address3=src.Address3,
		tar.Pincode=src.Pincode
	when not matched then 
		insert (LeadID,AddressType,Address1,Address2,Address3,Pincode)
		values(src.LeadID,'P',src.Address1,src.Address2,src.Address3,src.Pincode)
		;

end try
begin catch
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                          
end catch
end
