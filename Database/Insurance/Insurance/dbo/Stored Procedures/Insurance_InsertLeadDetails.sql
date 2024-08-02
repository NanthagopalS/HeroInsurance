CREATE      
 PROCEDURE [dbo].[Insurance_InsertLeadDetails] @VehicleTypeId VARCHAR(50) = NULL,    
   @VehicleNumber VARCHAR(50) = NULL,    
   @VariantId VARCHAR(50) = NULL,    
   @YearId VARCHAR(50) = NULL,    
   @LeadName VARCHAR(100) = NULL,    
   @PhoneNumber VARCHAR(12) = NULL,    
   @Email VARCHAR(100) = NULL,    
   @PrevPolicyTypeId VARCHAR(50) = NULL,    
   @RTOId VARCHAR(50) = NULL,    
   @LeadId VARCHAR(50) = NULL,    
   @IsBrandNew BIT = NULL,    
   @CreatedBy VARCHAR(100) = NULL,    
   @RefLeadId VARCHAR(50) = NULL,    
   @CarrierType INT = NULL,    
   @UsageNatureId INT = NULL,    
   @VehicleBodyId INT = NULL,    
   @HazardousVehicleUse INT = NULL,    
   @IfTrailer INT = NULL,    
   @TrailerIDV VARCHAR(100) = NULL,    
   @CategoryId INT = NULL,    
   @SubCategoryId INT = NULL,  
   @UsageTypeId INT = NULL,  
   @PCVVehicleCategoryId INT = NULL  
AS    
BEGIN    
   BEGIN TRY    
      IF (ISNULL(@RefLeadId, '') = '')    
      BEGIN    
         IF EXISTS (    
               SELECT 1    
               FROM Insurance_LeadDetails WITH (NOLOCK)    
               WHERE LeadId = @LeadId    
               )    
         BEGIN    
            IF @VehicleTypeId = '88a807b3-90e4-484b-b5d2-65059a8e1a91'    
            BEGIN    
               UPDATE Insurance_CommercialLeadDetail    
               SET CarrierTypeId = @CarrierType,    
                  UsageNatureId = @UsageNatureId,    
                  VehicleBodyId = @VehicleBodyId,    
                  IsHazardousVehicleUse = @HazardousVehicleUse,    
                  IsTrailer = @IfTrailer,    
                  TrailerIDV = @TrailerIDV,    
                  VehicleCategoryId = @CategoryId,    
                  VehicleSubCategoryId = @SubCategoryId,   
      UsageTypeId = @UsageTypeId,  
      PCVVehicleCategoryId = @PCVVehicleCategoryId,  
                  UpdatedOn = GETDATE()    
               WHERE LeadId = @LeadId    
            END    
    
            UPDATE Insurance_LeadDetails    
            SET VehicleTypeId = @VehicleTypeId,    
               VehicleNumber = @VehicleNumber,    
               VariantId = @VariantId,    
               YearId = @YearId,    
               PrevPolicyTypeId = @PrevPolicyTypeId,    
               RTOId = @RTOId,    
               IsBrandNew = @IsBrandNew,    
               UpdatedOn = GETDATE()    
            WHERE LeadId = @LeadId    
         END    
         ELSE    
         BEGIN    
            SELECT @LeadId = 'HERO/ENQ/' + CAST(NEXT VALUE FOR [dbo].[SEQ_LeadTransactionId] AS VARCHAR(10))    
    
            IF @VehicleTypeId = '88a807b3-90e4-484b-b5d2-65059a8e1a91'    
            BEGIN    
               INSERT INTO Insurance_CommercialLeadDetail (    
                  LeadId,    
                  CarrierTypeId,    
                  UsageNatureId,    
                  VehicleBodyId,    
                  IsHazardousVehicleUse,    
                  IsTrailer,    
                  TrailerIDV,    
                  VehicleCategoryId,    
                  VehicleSubCategoryId,  
      UsageTypeId,  
      PCVVehicleCategoryId  
                  )    
               VALUES (    
                  @LeadId,    
                  @CarrierType,    
                  @UsageNatureId,    
                  @VehicleBodyId,    
                  @HazardousVehicleUse,    
                  @IfTrailer,    
                  @TrailerIDV,    
                  @CategoryId,    
                  @SubCategoryId,  
      @UsageTypeId,  
      @PCVVehicleCategoryId  
                  );    
            END    
    
            INSERT INTO Insurance_LeadDetails (    
               LeadId,    
               VehicleTypeId,    
               VehicleNumber,    
               VariantId,    
               YearId,    
               LeadName,    
               PhoneNumber,    
               Email,    
               CreatedBy,    
               PrevPolicyTypeId,    
               StageId,    
               RTOId,    
               IsBrandNew    
               )    
            VALUES (    
     @LeadId,    
               @VehicleTypeId,    
               @VehicleNumber,    
               @VariantId,    
               @YearId,    
               @LeadName,    
               @PhoneNumber,    
               @Email,    
               @CreatedBy,    
               @PrevPolicyTypeId,    
               'AB4FA6D2-2C04-431A-8E6F-692359BAC662',    
               @RTOId,    
               @IsBrandNew    
               );-- StageID Set as PreQuote once create a new lead        
         END    
      END    
      ELSE    
      BEGIN    
         SELECT @LeadId = 'HERO/ENQ/' + CAST(NEXT VALUE FOR [dbo].[SEQ_LeadTransactionId] AS VARCHAR(10))    
    
         IF @VehicleTypeId = '88a807b3-90e4-484b-b5d2-65059a8e1a91'    
         BEGIN    
            INSERT INTO Insurance_CommercialLeadDetail (    
               LeadId,    
               CarrierTypeId,    
               UsageNatureId,    
               VehicleBodyId,    
               IsHazardousVehicleUse,    
               IsTrailer,    
               TrailerIDV,    
               VehicleCategoryId,    
               VehicleSubCategoryId,  
      UsageTypeId,  
      PCVVehicleCategoryId  
               )    
            VALUES (    
               @LeadId,    
               @CarrierType,    
               @UsageNatureId,    
               @VehicleBodyId,    
               @HazardousVehicleUse,    
               @IfTrailer,    
               @TrailerIDV,    
               @CategoryId,    
               @SubCategoryId,  
      @UsageTypeId,  
      @PCVVehicleCategoryId  
               );    
         END    
    
         INSERT INTO Insurance_LeadDetails (    
            LeadId,    
            VehicleTypeId,    
            VehicleNumber,    
            VariantId,    
            YearId,    
            LeadName,    
            PhoneNumber,    
            Email,    
            CreatedBy,    
            PrevPolicyTypeId,    
            StageId,    
            RTOId,    
            IsBrandNew,    
            RefLeadId    
            )    
         VALUES (    
            @LeadId,    
            @VehicleTypeId,    
            @VehicleNumber,    
            @VariantId,    
            @YearId,    
            @LeadName,    
            @PhoneNumber,    
            @Email,    
            @CreatedBy,    
            @PrevPolicyTypeId,    
            'AB4FA6D2-2C04-431A-8E6F-692359BAC662',    
            @RTOId,    
            @IsBrandNew,    
            @RefLeadId    
            );-- StageID Set as PreQuote once create a new lead         
      END    
    
      SELECT @LeadId LeadId    
   END TRY    
    
   BEGIN CATCH    
      DECLARE @StrProcedure_Name VARCHAR(500),    
         @ErrorDetail VARCHAR(1000),    
         @ParameterList VARCHAR(2000)    
    
      SET @StrProcedure_Name = ERROR_PROCEDURE()    
      SET @ErrorDetail = ERROR_MESSAGE()    
    
      EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name,    
         @ErrorDetail = @ErrorDetail,    
         @ParameterList = @ParameterList    
   END CATCH    
END 