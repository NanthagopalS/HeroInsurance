
Create procedure Insurance_InsertLeadNominee
@LeadID varchar(50),
@FirstName varchar(50), 
@LastName varchar(50), 
@DOB varchar(20),
@Age int,
@Relationship varchar(20)
as
begin

begin try
	
	Merge Insurance_LeadNomineeDetails as tar
	using(
		select @LeadID LeadID, @FirstName FirstName, @LastName LastName, @DOB DOB, @Age Age, @Relationship Relationship
	) as src
	on tar.LeadID=src.LeadID
	when matched then update set
		tar.FirstName=src.FirstName,
		tar.LastName=src.LastName,
		tar.DOB=src.DOB,
		tar.Age=src.Age,
		tar.Relationship=src.Relationship
	when not matched then 
		insert (LeadID,FirstName,LastName,DOB,Age,Relationship)
		values(src.LeadID,src.FirstName,src.LastName,src.DOB,src.Age,src.Relationship)
		;
end try
begin catch
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                          
end catch
end
