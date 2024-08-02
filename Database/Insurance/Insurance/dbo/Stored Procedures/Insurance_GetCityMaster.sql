CREATE Procedure [dbo].[Insurance_GetCityMaster]
as
begin
	select CityId, CityName from Insurance_City WITH(NOLOCK)
end
