CREATE FUNCTION dbo.RemoveNonAlphanumeric
(
    @inputString NVARCHAR(MAX)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @outputString NVARCHAR(MAX) = '';
    DECLARE @index INT = 1;

    WHILE @index <= LEN(@inputString)
    BEGIN
        DECLARE @char NCHAR(1) = SUBSTRING(@inputString, @index, 1);

        IF PATINDEX('%[a-zA-Z0-9]%', @char) > 0
        BEGIN
            SET @outputString = @outputString + @char;
        END

        SET @index = @index + 1;
    END

    RETURN @outputString;
END;