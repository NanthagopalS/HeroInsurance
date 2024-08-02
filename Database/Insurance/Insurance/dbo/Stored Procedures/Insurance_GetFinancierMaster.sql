CREATE Procedure [dbo].[Insurance_GetFinancierMaster]
as
begin
	select FinancierID, FinancierName from Insurance_Financier WITH(NOLOCK)
end
