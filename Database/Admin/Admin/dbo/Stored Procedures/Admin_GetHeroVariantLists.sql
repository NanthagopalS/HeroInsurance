-- =============================================                      
-- Author:  <Suraj Kumar Gupta>                      
-- Create date: <09-11-2023>                      
-- Description: <getheroVariantListWithMappedNotMapped>                      
/*      
EXEC Admin_GetHeroVariantLists '16413879-6316-4C1E-93A4-FF8318B14D37','3B6BAEF1-BD57-43E4-A220-9267A78628D5','','',0  
*/  
-- =============================================                      
CREATE     PROCEDURE [dbo].[Admin_GetHeroVariantLists] (  
    @InsurerId VARCHAR(50) = NULL,  
    @ModelId VARCHAR(50) = NULL,  
    @VariantIds VARCHAR(MAX) = NULL,  
    @FuelTypes VARCHAR(MAX) = NULL  
    )  
AS  
BEGIN  
    DECLARE @CurrentVId VARCHAR(100) = NULL  
    DECLARE @TEMP_TABLE_FOR_HERO_VARIANT AS TABLE (  
        ICName VARCHAR(100),  
        MMVScore float,  
        HeroVarientID VARCHAR(50),  
        ICVehicleCode VARCHAR(100),  
        HEROMake VARCHAR(100),  
        HeroModel VARCHAR(150),  
        HeroVarient VARCHAR(100),  
        HeroSC VARCHAR(100),  
        HCC VARCHAR(100),  
        HeroFuel VARCHAR(100),  
        ICMake VARCHAR(100),  
        ICModel VARCHAR(100),  
        ICVarient VARCHAR(100),  
        ICSC VARCHAR(100),  
        CCC VARCHAR(100),  
        ICFuel VARCHAR(100),  
  HeroGVW VARCHAR(100),  
  ICGVW VARCHAR(100),  
  IsManuallyMapped bit  
        )  
    DECLARE @MappingTableRecords AS TABLE (  
        ICName VARCHAR(100),  
        ScoreRank VARCHAR(100),  
        MScore VARCHAR(100),  
        VScore VARCHAR(100),  
        MoScore VARCHAR(100),  
        MMVScore VARCHAR(100),  
        HeroVarientID VARCHAR(100),  
        ICVehicleCode VARCHAR(100),  
        HEROMake VARCHAR(100),  
        HeroModel VARCHAR(100),  
        HeroVarient VARCHAR(100),  
        HeroSC VARCHAR(100),  
        HCC VARCHAR(100),  
        HeroFuel VARCHAR(100),  
        ICMake VARCHAR(100),  
        ICModel VARCHAR(100),  
        ICVarient VARCHAR(100),  
        ICSC VARCHAR(100),  
        CCC VARCHAR(100),  
        ICFuel VARCHAR(100),  
  HeroGVW VARCHAR(100),  
  ICGVW VARCHAR(100),  
  IsManuallyMapped bit  
        )  
  
    INSERT @TEMP_TABLE_FOR_HERO_VARIANT  
    SELECT '' ICName,  
        '' MMVScore,  
        VARIANT.VariantId HeroVarientID,  
        '' ICVehicleCode,  
        MAKE.MakeName HEROMake,  
        MODEL.ModelName HeroModel,  
        VARIANT.VariantName HeroVarient,  
        VARIANT.SeatingCapacity HeroSC,  
        VARIANT.CubicCapacity HCC,  
        FUEL.FuelName HeroFuel,  
        '' ICMake,  
        '' ICModel,  
        '' ICVarient,  
        '' ICSC,  
        '' CCC,  
        '' ICFuel,  
  VARIANT.GVW HeroGVW,  
  '' ICGVW,  
  0 IsManuallyMapped   
    FROM HeroInsurance.dbo.INSURANCE_VARIANT VARIANT WITH (NOLOCK)  
    JOIN HeroInsurance.dbo.Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId  
    JOIN HeroInsurance.dbo.Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId  
    JOIN HeroInsurance.dbo.Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId  
    WHERE MODEL.ModelId = @ModelId  
 AND (FUEL.FuelId IN (SELECT value FROM STRING_SPLIT(@FuelTypes,',')) OR ISNULL(@FuelTypes,'')='')  
 AND (VARIANT.VariantId IN (SELECT value FROM STRING_SPLIT(@VariantIds,',')) OR ISNULL(@VariantIds,'')='')  
    ORDER BY VARIANT.VariantId  
  
    DECLARE @TOtalVariant INT = (  
            SELECT COUNT(*)  
            FROM @TEMP_TABLE_FOR_HERO_VARIANT  
            ),  
        @CurrentCount INT = 1  
  
    PRINT (@TOtalVariant)  
  
    WHILE (@CurrentCount <= @TOtalVariant)  
    BEGIN  
        SET @CurrentVId = (  
                SELECT HeroVarientID  
                FROM @TEMP_TABLE_FOR_HERO_VARIANT  
                ORDER BY HeroVarientID OFFSET @CurrentCount - 1 ROWS FETCH NEXT 1 ROWS ONLY  
                )  
  
        PRINT (@CurrentVId)  
  
        IF (@InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C') -- TATA    
        BEGIN  
   PRINT('tata')  
            DELETE  
            FROM @MappingTableRecords  
  
            INSERT INTO @MappingTableRecords  
            SELECT *  
            FROM HeroInsurance.dbo.[Insurance_GetVariantSuggestionForTATA](@CurrentVId)  
            UPDATE @TEMP_TABLE_FOR_HERO_VARIANT  
            SET CCC = U.CCC,  
                MMVScore = U.MMVScore,  
                ICVehicleCode = U.ICVehicleCode,  
                ICMake = U.ICMake,  
                ICModel = U.ICModel,  
                ICVarient = U.ICVarient,  
                ICSC = U.ICSC,  
                ICFuel = U.ICFuel,  
    ICName = U.ICName,  
    ICGVW = U.ICGVW,  
    IsManuallyMapped = U.IsManuallyMapped  
            FROM @TEMP_TABLE_FOR_HERO_VARIANT c  
            JOIN @MappingTableRecords U ON c.HeroVarientID = U.HeroVarientID  
            WHERE c.HeroVarientID = @CurrentVId;  
        END  
  
        IF (@InsurerId = '16413879-6316-4C1E-93A4-FF8318B14D37') -- BAJAJ    
        BEGIN  
            DELETE  
            FROM @MappingTableRecords  
  
            INSERT INTO @MappingTableRecords  
            SELECT *  
            FROM HeroInsurance.dbo.[Insurance_GetVariantSuggestionForBajaj](@CurrentVId)  
  
            UPDATE @TEMP_TABLE_FOR_HERO_VARIANT  
            SET CCC = U.CCC,  
                MMVScore = U.MMVScore,  
                ICVehicleCode = U.ICVehicleCode,  
                ICMake = U.ICMake,  
                ICModel = U.ICModel,  
                ICVarient = U.ICVarient,  
                ICSC = U.ICSC,  
                ICFuel = U.ICFuel,  
    ICName = U.ICName,  
    ICGVW = U.ICGVW,  
    IsManuallyMapped = U.IsManuallyMapped  
            FROM @TEMP_TABLE_FOR_HERO_VARIANT c  
            JOIN @MappingTableRecords U ON c.HeroVarientID = U.HeroVarientID  
            WHERE c.HeroVarientID = @CurrentVId;  
        END  
  
        IF (@InsurerId = '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA') -- CHOLA    
        BEGIN  
            DELETE  
            FROM @MappingTableRecords  
  
            INSERT INTO @MappingTableRecords  
            SELECT *  
            FROM HeroInsurance.dbo.[Insurance_GetVariantSuggestionForChola](@CurrentVId)  
  
            UPDATE @TEMP_TABLE_FOR_HERO_VARIANT  
            SET CCC = U.CCC,  
                MMVScore = U.MMVScore,  
                ICVehicleCode = U.ICVehicleCode,  
                ICMake = U.ICMake,  
                ICModel = U.ICModel,  
                ICVarient = U.ICVarient,  
                ICSC = U.ICSC,  
                ICFuel = U.ICFuel,  
    ICName = U.ICName,  
    ICGVW = U.ICGVW,  
    IsManuallyMapped = U.IsManuallyMapped  
            FROM @TEMP_TABLE_FOR_HERO_VARIANT c  
            JOIN @MappingTableRecords U ON c.HeroVarientID = U.HeroVarientID  
            WHERE c.HeroVarientID = @CurrentVId;  
        END  
  
        IF (@InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621') -- GO DIGIT    
        BEGIN  
            DELETE  
            FROM @MappingTableRecords  
  
            INSERT INTO @MappingTableRecords  
            SELECT *  
            FROM HeroInsurance.dbo.[Insurance_GetVariantSuggestionForGoDigit](@CurrentVId)  
  
            UPDATE @TEMP_TABLE_FOR_HERO_VARIANT  
            SET CCC = U.CCC,  
                MMVScore = U.MMVScore,  
                ICVehicleCode = U.ICVehicleCode,  
                ICMake = U.ICMake,  
                ICModel = U.ICModel,  
                ICVarient = U.ICVarient,  
                ICSC = U.ICSC,  
                ICFuel = U.ICFuel,  
    ICName = U.ICName,  
    ICGVW = U.ICGVW,  
    IsManuallyMapped = U.IsManuallyMapped  
            FROM @TEMP_TABLE_FOR_HERO_VARIANT c  
            JOIN @MappingTableRecords U ON c.HeroVarientID = U.HeroVarientID  
            WHERE c.HeroVarientID = @CurrentVId;  
        END  
  
        IF (@InsurerId = '0A326B77-AFD5-44DA-9871-1742624CFF16') -- HDFC    
        BEGIN  
            DELETE  
            FROM @MappingTableRecords  
  
            INSERT INTO @MappingTableRecords  
            SELECT *  
            FROM HeroInsurance.dbo.[Insurance_GetVariantSuggestionForHDFC](@CurrentVId)  
  
            UPDATE @TEMP_TABLE_FOR_HERO_VARIANT  
            SET CCC = U.CCC,  
         MMVScore = U.MMVScore,  
                ICVehicleCode = U.ICVehicleCode,  
                ICMake = U.ICMake,  
                ICModel = U.ICModel,  
                ICVarient = U.ICVarient,  
                ICSC = U.ICSC,  
                ICFuel = U.ICFuel,  
    ICName = U.ICName,  
    ICGVW = U.ICGVW,  
    IsManuallyMapped = U.IsManuallyMapped  
            FROM @TEMP_TABLE_FOR_HERO_VARIANT c  
            JOIN @MappingTableRecords U ON c.HeroVarientID = U.HeroVarientID  
            WHERE c.HeroVarientID = @CurrentVId;  
        END  
  
        IF (@InsurerId = '372B076C-D9D9-48DC-9526-6EB3D525CAB6') -- Reliance    
        BEGIN  
            DELETE  
            FROM @MappingTableRecords  
  
            INSERT INTO @MappingTableRecords  
            SELECT *  
            FROM HeroInsurance.dbo.[Insurance_GetVariantSuggestionForReliance](@CurrentVId)  
  
            UPDATE @TEMP_TABLE_FOR_HERO_VARIANT  
            SET CCC = U.CCC,  
                MMVScore = U.MMVScore,  
                ICVehicleCode = U.ICVehicleCode,  
                ICMake = U.ICMake,  
                ICModel = U.ICModel,  
                ICVarient = U.ICVarient,  
                ICSC = U.ICSC,  
                ICFuel = U.ICFuel,  
    ICName = U.ICName,  
    ICGVW = U.ICGVW,  
    IsManuallyMapped = U.IsManuallyMapped  
            FROM @TEMP_TABLE_FOR_HERO_VARIANT c  
            JOIN @MappingTableRecords U ON c.HeroVarientID = U.HeroVarientID  
            WHERE c.HeroVarientID = @CurrentVId;  
        END  
  
        IF (@InsurerId = 'FD3677E5-7938-46C8-9CD2-FAE188A1782C') -- ICICI    
        BEGIN  
            DELETE  
            FROM @MappingTableRecords  
  
            INSERT INTO @MappingTableRecords  
            SELECT *  
            FROM HeroInsurance.dbo.[Insurance_GetVariantSuggestionForICICI](@CurrentVId)  
  
            UPDATE @TEMP_TABLE_FOR_HERO_VARIANT  
            SET CCC = U.CCC,  
                MMVScore = U.MMVScore,  
                ICVehicleCode = U.ICVehicleCode,  
                ICMake = U.ICMake,  
                ICModel = U.ICModel,  
                ICVarient = U.ICVarient,  
                ICSC = U.ICSC,  
                ICFuel = U.ICFuel,  
    ICName = U.ICName,  
    ICGVW = U.ICGVW,  
    IsManuallyMapped = U.IsManuallyMapped  
            FROM @TEMP_TABLE_FOR_HERO_VARIANT c  
            JOIN @MappingTableRecords U ON c.HeroVarientID = U.HeroVarientID  
            WHERE c.HeroVarientID = @CurrentVId;  
        END  
  
        IF (@InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3') -- Iffco    
        BEGIN  
            DELETE  
            FROM @MappingTableRecords  
  
            INSERT INTO @MappingTableRecords  
            SELECT *  
            FROM HeroInsurance.dbo.[Insurance_GetVariantSuggestionForIFFCO](@CurrentVId)  
  
            UPDATE @TEMP_TABLE_FOR_HERO_VARIANT  
            SET CCC = U.CCC,  
                MMVScore = U.MMVScore,  
                ICVehicleCode = U.ICVehicleCode,  
                ICMake = U.ICMake,  
                ICModel = U.ICModel,  
                ICVarient = U.ICVarient,  
                ICSC = U.ICSC,  
                ICFuel = U.ICFuel,  
    ICName = U.ICName,  
    ICGVW = U.ICGVW,  
    IsManuallyMapped = U.IsManuallyMapped  
            FROM @TEMP_TABLE_FOR_HERO_VARIANT c  
            JOIN @MappingTableRecords U ON c.HeroVarientID = U.HeroVarientID  
            WHERE c.HeroVarientID = @CurrentVId;  
        END  
  
        IF (@InsurerId = '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9') -- ORIANTAL    
        BEGIN  
            DELETE  
            FROM @MappingTableRecords  
  
            INSERT INTO @MappingTableRecords  
            SELECT *  
            FROM HeroInsurance.dbo.[Insurance_GetVariantSuggestionForOriental](@CurrentVId)  
  
            UPDATE @TEMP_TABLE_FOR_HERO_VARIANT  
            SET CCC = U.CCC,  
                MMVScore = U.MMVScore,  
                ICVehicleCode = U.ICVehicleCode,  
                ICMake = U.ICMake,  
                ICModel = U.ICModel,  
                ICVarient = U.ICVarient,  
               ICSC = U.ICSC,  
                ICFuel = U.ICFuel,  
    ICName = U.ICName,  
    ICGVW = U.ICGVW,  
    IsManuallyMapped = U.IsManuallyMapped  
            FROM @TEMP_TABLE_FOR_HERO_VARIANT c  
            JOIN @MappingTableRecords U ON c.HeroVarientID = U.HeroVarientID  
            WHERE c.HeroVarientID = @CurrentVId;  
        END  
  
        SET @CurrentCount = @CurrentCount + 1  
    END  
  
    SELECT *  
    FROM @TEMP_TABLE_FOR_HERO_VARIANT  ORDER BY HeroVarient,IsManuallyMapped
  
 select * from  HeroInsurance.dbo.[Insurance_GetAllVariantForModel] (@InsurerId,@ModelId)  
  
END