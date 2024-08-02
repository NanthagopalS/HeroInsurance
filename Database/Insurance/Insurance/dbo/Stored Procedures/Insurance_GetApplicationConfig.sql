
CREATE Procedure [dbo].[Insurance_GetApplicationConfig]
@Name varchar(500)
as
begin
	select ConfigName, ConfigValue from Insurance_ApplicationConfig WITH(NOLOCK) where ConfigName in (select value from string_split(@Name,','))
end
