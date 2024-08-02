--[Insurance_GetBajajCKYCDetails] '006A646D-F731-4036-9119-5ED604CB36F2','AADHAAR'
CREATE PROCEDURE [dbo].[Insurance_GetBajajCKYCDetails]  
@QuoteTransactionID VARCHAR(50) = null,
@DocumentType VARCHAR(50) = null
AS  
BEGIN  
	DECLARE @QuoteRequestBoby NVARCHAR(MAX), @quoteResponseBody NVARCHAR(MAX), @DocumentCode VARCHAR(50)
  SELECT @QuoteRequestBoby = RequestBody ,@quoteResponseBody = ResponseBody FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionId = @QuoteTransactionID
  SELECT @DocumentCode = DocumentCode FROM Insurance_Documents WHERE DocumentName = @DocumentType AND InsurerId = '16413879-6316-4C1E-93A4-FF8318B14D37'

  SELECT @QuoteRequestBoby QuoteRequestBoby,
  @quoteResponseBody QuoteResponseBody,
  @DocumentCode DocumentCode
END  
