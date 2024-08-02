
CREATE procedure [dbo].[Insurance_InsertInsuranceICConfig]
@InsurerId VARCHAR(50),    
@PolicyTypeId VARCHAR(50),  
@VehicleTypeId VARCHAR(50),
@NameValue varchar(max) -- Name1=value1,Name2=value2
as
begin
	
	declare @configdata as table(id int identity(1,1), Configdata varchar(max))

	insert into @configdata(Configdata)
	select value from string_split(@namevalue,',')

	declare @cnt as int = (select count(1) from @configdata)
	declare @temp as int = 1
	while(@temp<=@cnt)
	begin
		declare @value varchar(max), @ConfigName varchar(50), @ConfigValue varchar(1000)
		select @value = Configdata from @configdata where id=@temp
		
		set @ConfigName = SUBSTRING(@value, 1, CASE CHARINDEX('=', @value)
			WHEN 0
				THEN LEN(@value)
			ELSE CHARINDEX('=', @value) - 1
			END) 
		set @ConfigValue = SUBSTRING(@value, CASE CHARINDEX('=', @value)
			WHEN 0
				THEN LEN(@value) + 1
			ELSE CHARINDEX('=', @value) + 1
			END, 1000) 

		if exists(select top 1 1 from Insurance_ICConfig WITH(NOLOCK) where InsurerId=@InsurerId and PolicyTypeId=@PolicyTypeId and VehicleTypeId=@VehicleTypeId and ConfigName=@ConfigName)
			update Insurance_ICConfig set ConfigValue=@ConfigValue, UpdatedOn=GETDATE() where InsurerId=@InsurerId and PolicyTypeId=@PolicyTypeId and VehicleTypeId=@VehicleTypeId and ConfigName=@ConfigName
		else
			insert into Insurance_ICConfig(InsurerId,PolicyTypeId,VehicleTypeId,ConfigName,ConfigValue,CreatedBy,CreatedOn) 
			values(@InsurerId, @PolicyTypeId,@VehicleTypeId,@ConfigName,@ConfigValue,1,GETDATE())

		set @temp +=1
	end
end



