-- =============================================                                        
-- Author:  <Suraj Kumar Gupta>                                        
-- Create date: <14-11-2023>                                        
-- Description: <UpdateheroVariantListWithMappedNotMapped>                                        
-- =============================================                                        
CREATE
  

 PROCEDURE [dbo].[Admin_UpdateHeroVariantListsForIcId] (
  @InsurerId VARCHAR(100) = NULL,
  @UpdateVariantsMappingTableType AS [dbo].UpdateVariantsMappingTableType READONLY,
  @newRecordDataTable AS [dbo].UpdateVariantsMappingTableType READONLY,
  @UpdatedBy VARCHAR(100) = NULL
  )
AS
BEGIN
  DECLARE @UpdatedRecords INT = NULL

  BEGIN TRY
    IF (@InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C') -- TATA                      
    BEGIN
      UPDATE HeroInsurance.MOTOR.TATA_VehicleMaster
      SET VarientId = IC.HeroVariantId,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETDATE(),
        isManuallyMapped = 1
      FROM @UpdateVariantsMappingTableType IC
      JOIN HeroInsurance.MOTOR.TATA_VehicleMaster U ON IC.ICVehicleCode = U.SR_NO
      WHERE ISNULL(IC.ICVehicleCode, '') != ''

      SET @UpdatedRecords = @@ROWCOUNT

      IF EXISTS (
          SELECT *
          FROM @newRecordDataTable
          WHERE ISNULL(HeroVariantId, '') != ''
          )
      BEGIN
        INSERT INTO HeroInsurance.MOTOR.TATA_VehicleMaster (
          SR_NO,
          NUM_MANUFACTURE_CD,
          TXT_MANUFACTURERNAME,
          NUM_MODEL_CODE,
          TXT_MODEL,
          NUM_MODEL_VARIANT_CODE,
          TXT_MODEL_VARIANT,
          NUM_PRODUCT_CODE,
          NUM_PRODUCT_NAME,
          NUM_VEHICLE_CLASS,
          NUM_VEHICLE_SUB_CLASS,
          TXT_BODY_TYPE,
          NUM_CUBIC_CAPACITY,
          TXT_SEGMENT,
          NUM_GROSS_VEHICLE_WEIGHT,
          NUM_SEATING_CAPACITY,
          NUM_CARRYING_CAPACITY,
          TXT_FUEL_TYPE,
          NUM_NO_OF_WHEELS,
          NUM_BODY_TYPE,
          VarientId,
          CreatedBy,
          CreatedOn,
          UpdatedBy,
          UpdatedOn,
          IsManuallyMapped
          )
        SELECT IC.SR_NO,
          IC.NUM_MANUFACTURE_CD,
          IC.TXT_MANUFACTURERNAME,
          IC.NUM_MODEL_CODE,
          IC.TXT_MODEL,
          IC.NUM_MODEL_VARIANT_CODE,
          IC.TXT_MODEL_VARIANT,
          IC.NUM_PRODUCT_CODE,
          IC.NUM_PRODUCT_NAME,
          IC.NUM_VEHICLE_CLASS,
          IC.NUM_VEHICLE_SUB_CLASS,
          IC.TXT_BODY_TYPE,
          IC.NUM_CUBIC_CAPACITY,
          IC.TXT_SEGMENT,
          IC.NUM_GROSS_VEHICLE_WEIGHT,
          IC.NUM_SEATING_CAPACITY,
          IC.NUM_CARRYING_CAPACITY,
          IC.TXT_FUEL_TYPE,
          IC.NUM_NO_OF_WHEELS,
          IC.NUM_BODY_TYPE,
          NEW.HeroVariantId,
          @UpdatedBy,
          GETDATE(),
          IC.UpdatedBy,
          IC.UpdatedOn,
          1
        FROM @newRecordDataTable NEW
        JOIN HeroInsurance.MOTOR.TATA_VehicleMaster IC ON NEW.ICVehicleCode = IC.SR_NO
      END
    END

    IF (@InsurerId = '16413879-6316-4C1E-93A4-FF8318B14D37') -- BAJAJ                      
    BEGIN
      UPDATE HeroInsurance.MOTOR.Bajaj_VehicleMaster
      SET VariantId = IC.HeroVariantId,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETDATE(),
        isManuallyMapped = 1
      FROM @UpdateVariantsMappingTableType IC
      JOIN HeroInsurance.MOTOR.Bajaj_VehicleMaster U ON IC.ICVehicleCode = U.VehicleCode
      WHERE ISNULL(IC.ICVehicleCode, '') != ''

      SET @UpdatedRecords = @@ROWCOUNT

      IF EXISTS (
          SELECT *
          FROM @newRecordDataTable
          WHERE ISNULL(HeroVariantId, '') != ''
          )
      BEGIN
        INSERT INTO HeroInsurance.MOTOR.Bajaj_VehicleMaster (
          VariantId,
          VehicleCode,
          VehicleType,
          VehicleMakeCode,
          VehicleMake,
          VehicleModelCode,
          VehicleModel,
          VehicleSubTypeCode,
          VehicleSubType,
          Fuel,
          CubicCapacity,
          CarryingCapacity,
          Extracol1,
          Extracol2,
          Extracol3,
          Extracol4,
          Extracol5,
          IsManuallyMapped,
          UpdatedBy,
          UpdatedOn
          )
        SELECT NEW.HeroVariantId,
          IC.VehicleCode,
          IC.VehicleType,
          IC.VehicleMakeCode,
          IC.VehicleMake,
          IC.VehicleModelCode,
          IC.VehicleModel,
          IC.VehicleSubTypeCode,
          IC.VehicleSubType,
          IC.Fuel,
          IC.CubicCapacity,
          IC.CarryingCapacity,
          IC.Extracol1,
          IC.Extracol2,
          IC.Extracol3,
          IC.Extracol4,
          IC.Extracol5,
          1,
          IC.UpdatedBy,
          IC.UpdatedOn
        FROM @newRecordDataTable NEW
        JOIN HeroInsurance.MOTOR.Bajaj_VehicleMaster IC ON NEW.ICVehicleCode = IC.VehicleCode
      END
    END

    IF (@InsurerId = '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA') -- CHOLA                      
    BEGIN
      UPDATE HeroInsurance.MOTOR.Chola_VehicleMaster
      SET VarientId = IC.HeroVariantId,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETDATE(),
        isManuallyMapped = 1
      FROM @UpdateVariantsMappingTableType IC
      JOIN HeroInsurance.MOTOR.Chola_VehicleMaster U ON IC.ICVehicleCode = U.ModelCode
      WHERE ISNULL(IC.ICVehicleCode, '') != ''

      SET @UpdatedRecords = @@ROWCOUNT

      IF EXISTS (
          SELECT *
          FROM @newRecordDataTable
          WHERE ISNULL(HeroVariantId, '') != ''
          )
      BEGIN
        DECLARE @VariantAndStateCode AS TABLE (
          Make VARCHAR(100),
          MakeCode VARCHAR(100),
          VehicleModel VARCHAR(100),
          ModelCode VARCHAR(100),
          BodyType VARCHAR(100),
          CubicCapacity VARCHAR(100),
          KiloWatt VARCHAR(100),
          SeatingCapacity VARCHAR(100),
          FuelType VARCHAR(100),
          NumStateCode VARCHAR(100),
          [State] VARCHAR(100),
          StateName VARCHAR(100),
          City VARCHAR(100),
          ExShowRoom VARCHAR(100),
          VehicleTypeId VARCHAR(100),
          VehicleClass VARCHAR(100),
          PolicyTypeId VARCHAR(100)
          )

        INSERT INTO @VariantAndStateCode
        SELECT Make,
          MakeCode,
          VehicleModel,
          ModelCode,
          BodyType,
          CubicCapacity,
          KiloWatt,
          SeatingCapacity,
          FuelType,
          NumStateCode,
          [State],
          StateName,
          City,
          ExShowRoom,
          VehicleTypeId,
          VehicleClass,
          PolicyTypeId
        FROM HeroInsurance.MOTOR.Chola_VehicleMaster
        WHERE ModelCode IN (
            SELECT ICVehicleCode
            FROM @newRecordDataTable
            )
        GROUP BY ModelCode,
          NumStateCode,
          [State],
          StateName,
          Make,
          MakeCode,
          VehicleModel,
          BodyType,
          CubicCapacity,
          KiloWatt,
          SeatingCapacity,
          FuelType,
          City,
          ExShowRoom,
          VehicleTypeId,
          VehicleClass,
          PolicyTypeId

        DECLARE @CholaInsertionCount INT = (
            SELECT COUNT(*)
            FROM @newRecordDataTable
            )
        DECLARE @CholaCurrentCount INT = 1
        DECLARE @CurrentHeroId VARCHAR(50) = NULL

        WHILE (@CholaCurrentCount <= @CholaInsertionCount)
        BEGIN
          SET @CurrentHeroId = (
              SELECT HeroVariantId
              FROM @newRecordDataTable
              ORDER BY HeroVariantId OFFSET @CholaCurrentCount - 1 ROWS FETCH NEXT 1 ROWS ONLY
              )

          INSERT INTO HeroInsurance.MOTOR.Chola_VehicleMaster (
            Make,
            MakeCode,
            VehicleModel,
            ModelCode,
            BodyType,
            CubicCapacity,
            KiloWatt,
            SeatingCapacity,
            FuelType,
            NumStateCode,
            [State],
            StateName,
            City,
            ExShowRoom,
            VehicleTypeId,
            VarientId,
            CreatedOn,
            CreatedBy,
            UpdatedOn,
            UpdatedBy,
            VehicleClass,
            PolicyTypeId,
            IsManuallyMapped
            )
          SELECT NEWCHOLA.Make,
            NEWCHOLA.MakeCode,
            NEWCHOLA.VehicleModel,
            NEWCHOLA.ModelCode,
            NEWCHOLA.BodyType,
            NEWCHOLA.CubicCapacity,
            NEWCHOLA.KiloWatt,
            NEWCHOLA.SeatingCapacity,
            NEWCHOLA.FuelType,
            NEWCHOLA.NumStateCode,
            NEWCHOLA.[State],
            NEWCHOLA.StateName,
            NEWCHOLA.City,
            NEWCHOLA.ExShowRoom,
            NEWCHOLA.VehicleTypeId,
            @CurrentHeroId,
            GETDATE(),
            @UpdatedBy,
            NULL,
            NULL,
            NEWCHOLA.VehicleClass,
            NEWCHOLA.PolicyTypeId,
            1
          FROM @VariantAndStateCode NEWCHOLA
          JOIN @newRecordDataTable NEW ON NEWCHOLA.ModelCode = NEW.IcVehicleCode
          WHERE NEW.HeroVariantId = @CurrentHeroId

          SET @CholaCurrentCount = @CholaCurrentCount + 1;
        END
      END
    END

    IF (@InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621') -- GO DIGIT                      
    BEGIN
      UPDATE HeroInsurance.MOTOR.GoDigit_VehicleMaster
      SET VariantId = IC.HeroVariantId,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETDATE(),
        isManuallyMapped = 1
      FROM @UpdateVariantsMappingTableType IC
      JOIN HeroInsurance.MOTOR.GoDigit_VehicleMaster U ON IC.ICVehicleCode = U.[Vehicle Code]
      WHERE ISNULL(IC.ICVehicleCode, '') != ''

      SET @UpdatedRecords = @@ROWCOUNT

      IF EXISTS (
          SELECT *
          FROM @newRecordDataTable
          WHERE ISNULL(HeroVariantId, '') != ''
          )
      BEGIN
        INSERT INTO HeroInsurance.MOTOR.GoDigit_VehicleMaster (
          [Vehicle Code],
          Make,
          Model,
          Variant,
          VariantId,
          BodyType,
          SeatingCapacity,
          Power,
          CubicCapacity,
          GrosssVehicleWeight,
          FuelType,
          NoOfWheels,
          Abs,
          AirBags,
          Length,
          ExShowroomPrice,
          PriceYear,
          Production,
          Manufacturing,
          VehicleType,
          IsManuallyMapped,
          UpdatedBy,
          UpdatedOn
          )
        SELECT IC.[Vehicle Code],
          IC.Make,
          IC.Model,
          IC.Variant,
          NEW.HeroVariantId,
          IC.BodyType,
          IC.SeatingCapacity,
          IC.Power,
          IC.CubicCapacity,
          IC.GrosssVehicleWeight,
          IC.FuelType,
          IC.NoOfWheels,
          IC.Abs,
          IC.AirBags,
          IC.Length,
          IC.ExShowroomPrice,
          IC.PriceYear,
          IC.Production,
          IC.Manufacturing,
          IC.VehicleType,
          1,
          IC.UpdatedBy,
          IC.UpdatedOn
        FROM @newRecordDataTable NEW
        JOIN HeroInsurance.MOTOR.GoDigit_VehicleMaster IC ON NEW.ICVehicleCode = IC.[Vehicle Code]
      END
    END

    IF (@InsurerId = '0A326B77-AFD5-44DA-9871-1742624CFF16') -- HDFC                      
    BEGIN
      UPDATE HeroInsurance.MOTOR.hdfc_VehicleMaster
      SET VariantId = IC.HeroVariantId,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETDATE(),
        isManuallyMapped = 1
      FROM @UpdateVariantsMappingTableType IC
      JOIN HeroInsurance.MOTOR.hdfc_VehicleMaster U ON IC.ICVehicleCode = U.VEHICLEMODELCODE
      WHERE ISNULL(IC.ICVehicleCode, '') != ''

      SET @UpdatedRecords = @@ROWCOUNT

      IF EXISTS (
          SELECT *
          FROM @newRecordDataTable
          WHERE ISNULL(HeroVariantId, '') != ''
          )
      BEGIN
        INSERT INTO HeroInsurance.MOTOR.HDFC_VehicleMaster (
          MANUFACTURER,
          VEHICLEMODELCODE,
          VEHICLEMODEL,
          NUMBEROFWHEELS,
          CUBICCAPACITY,
          GROSSVEHICLEWEIGHT,
          SEATINGCAPACITY,
          CARRYINGCAPACITY,
          TXT_FUEL,
          TXT_VARIANT,
          VariantId,
          CreatedOn,
          UpdatedBy,
          UpdatedOn,
          IsCommercial,
          IsManuallyMapped
          )
        SELECT IC.MANUFACTURER,
          IC.VEHICLEMODELCODE,
          IC.VEHICLEMODEL,
          IC.NUMBEROFWHEELS,
          IC.CUBICCAPACITY,
          IC.GROSSVEHICLEWEIGHT,
          IC.SEATINGCAPACITY,
          IC.CARRYINGCAPACITY,
          IC.TXT_FUEL,
          IC.TXT_VARIANT,
          NEW.HeroVariantId,
          GETDATE(),
          IC.UpdatedBy,
          IC.UpdatedOn,
          IC.IsCommercial,
          1
        FROM @newRecordDataTable NEW
        JOIN HeroInsurance.MOTOR.HDFC_VehicleMaster IC ON NEW.ICVehicleCode = IC.VEHICLEMODELCODE
      END
    END

    IF (@InsurerId = '372B076C-D9D9-48DC-9526-6EB3D525CAB6') -- Reliance                      
    BEGIN
      UPDATE HeroInsurance.MOTOR.Reliance_VehicleMaster
      SET VarientId = IC.HeroVariantId,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETDATE(),
        isManuallyMapped = 1
      FROM @UpdateVariantsMappingTableType IC
      JOIN HeroInsurance.MOTOR.Reliance_VehicleMaster U ON IC.ICVehicleCode = U.ModelId
      WHERE ISNULL(IC.ICVehicleCode, '') != ''

      SET @UpdatedRecords = @@ROWCOUNT

      IF EXISTS (
          SELECT *
          FROM @newRecordDataTable
          WHERE ISNULL(HeroVariantId, '') != ''
          )
      BEGIN
        INSERT INTO HeroInsurance.MOTOR.Reliance_VehicleMaster (
          MakeID,
          MakeName,
          ModelID,
          ModelName,
          Variance,
          Wheels,
          OperatedBy,
          CC,
          SeatingCapacity,
          CarryingCapacity,
          VehTypeID,
          VehTypeName,
          CreatedBy,
          CreatedOn,
          UpdatedBy,
          UpdatedOn,
          VarientId,
          IsManuallyMapped
          )
        SELECT IC.MakeID,
          IC.MakeName,
          IC.ModelID,
          IC.ModelName,
          IC.Variance,
          IC.Wheels,
          IC.OperatedBy,
          IC.CC,
          IC.SeatingCapacity,
          IC.CarryingCapacity,
          IC.VehTypeID,
          IC.VehTypeName,
          @UpdatedBy,
          GETDATE(),
          IC.UpdatedBy,
          IC.UpdatedOn,
          NEW.HeroVariantId,
          1
        FROM @newRecordDataTable NEW
        JOIN HeroInsurance.MOTOR.Reliance_VehicleMaster IC ON NEW.ICVehicleCode = IC.ModelId
      END
    END

    IF (@InsurerId = 'FD3677E5-7938-46C8-9CD2-FAE188A1782C') -- ICICI                      
    BEGIN
      UPDATE HeroInsurance.MOTOR.ICICI_VehicleMaster
      SET VariantId = IC.HeroVariantId,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETDATE(),
        isManuallyMapped = 1
      FROM @UpdateVariantsMappingTableType IC
      JOIN HeroInsurance.MOTOR.ICICI_VehicleMaster U ON IC.ICVehicleCode = U.VehicleModelCode
      WHERE ISNULL(IC.ICVehicleCode, '') != ''

      SET @UpdatedRecords = @@ROWCOUNT

      IF EXISTS (
          SELECT *
          FROM @newRecordDataTable
          WHERE ISNULL(HeroVariantId, '') != ''
          )
      BEGIN
        INSERT INTO HeroInsurance.MOTOR.ICICI_VehicleMaster (
          VariantId,
          VehicleClassCode,
          VehicleSubClassDesc,
          VehicleSubClassCode,
          VehicleManufactureCode,
          Manufacture,
          VehicleModelCode,
          VehicleModel,
          CubicCapacity,
          SeatingCapacity,
          CarringCapacity,
          VehicleModelStatus,
          ActiveFlag,
          ModelBuild,
          MaxPrice,
          MinimumPrice,
          CarCategory,
          FuelType,
          Segment,
          GVW,
          NumberOfWheels,
          ExShowroomSlab,
          CreatedOn,
          CreatedBy,
          UpdatedOn,
          UpdatedBy,
          IsManuallyMapped
          )
        SELECT NEW.HeroVariantId,
          IC.VehicleClassCode,
          IC.VehicleSubClassDesc,
          IC.VehicleSubClassCode,
          IC.VehicleManufactureCode,
          IC.Manufacture,
          IC.VehicleModelCode,
          IC.VehicleModel,
          IC.CubicCapacity,
          IC.SeatingCapacity,
          IC.CarringCapacity,
          IC.VehicleModelStatus,
          IC.ActiveFlag,
          IC.ModelBuild,
          IC.MaxPrice,
          IC.MinimumPrice,
          IC.CarCategory,
          IC.FuelType,
          IC.Segment,
          IC.GVW,
          IC.NumberOfWheels,
          IC.ExShowroomSlab,
          GETDATE(),
          @UpdatedBy,
          IC.UpdatedOn,
          IC.UpdatedBy,
          1
        FROM @newRecordDataTable NEW
        JOIN HeroInsurance.MOTOR.ICICI_VehicleMaster IC ON NEW.ICVehicleCode = IC.VehicleModelCode
      END
    END

    IF (@InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3') -- Iffco                      
    BEGIN
      UPDATE HeroInsurance.MOTOR.itgi_VehicleMaster
      SET VariantId = IC.HeroVariantId,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETDATE(),
        isManuallyMapped = 1
      FROM @UpdateVariantsMappingTableType IC
      JOIN HeroInsurance.MOTOR.ITGI_VehicleMaster U ON IC.ICVehicleCode = U.MAKE_CODE
      WHERE ISNULL(IC.ICVehicleCode, '') != ''

      SET @UpdatedRecords = @@ROWCOUNT

      IF EXISTS (
          SELECT *
          FROM @newRecordDataTable
          WHERE ISNULL(HeroVariantId, '') != ''
          )
      BEGIN
        INSERT INTO HeroInsurance.MOTOR.ITGI_VehicleMaster (
          MAKE_CODE,
          MANUFACTURE,
          MODEL,
          VARIANT,
          CC,
          SEATING_CAPACITY,
          CONTRACT_TYPE,
          FUEL_TYPE,
          FROM_YEAR,
          TO_YEAR,
          VariantId,
          CreatedBy,
          CreatedOn,
          UpdatedOn,
          UpdatedBy,
          IsCommercial,
          SubClass,
          ExShowroomPrice,
          GVW,
          IsManuallyMapped
          )
        SELECT IC.MAKE_CODE,
          IC.MANUFACTURE,
          IC.MODEL,
          IC.VARIANT,
          IC.CC,
          IC.SEATING_CAPACITY,
          IC.CONTRACT_TYPE,
          IC.FUEL_TYPE,
          IC.FROM_YEAR,
          IC.TO_YEAR,
          NEW.HeroVariantId,
          @UpdatedBy,
          GETDATE(),
          IC.UpdatedOn,
          IC.UpdatedBy,
          IC.IsCommercial,
          IC.SubClass,
          IC.ExShowroomPrice,
          IC.GVW,
          1
        FROM @newRecordDataTable NEW
        JOIN HeroInsurance.MOTOR.ITGI_VehicleMaster IC ON NEW.ICVehicleCode = IC.MAKE_CODE
      END
    END

    IF (@InsurerId = '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9') -- ORIANTAL                      
    BEGIN
      UPDATE HeroInsurance.MOTOR.Oriental_VehicleMaster
      SET VariantId = IC.HeroVariantId,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETDATE(),
        isManuallyMapped = 1
      FROM @UpdateVariantsMappingTableType IC
      JOIN HeroInsurance.MOTOR.Oriental_VehicleMaster U ON IC.ICVehicleCode = U.VEH_MODEL
      WHERE ISNULL(IC.ICVehicleCode, '') != ''

      SET @UpdatedRecords = @@ROWCOUNT

      IF EXISTS (
          SELECT *
          FROM @newRecordDataTable
          WHERE ISNULL(HeroVariantId, '') != ''
          )
      BEGIN
        INSERT INTO HeroInsurance.MOTOR.Oriental_VehicleMaster (
          VEH_MAKE,
          VEH_MAKE_DESC,
          VEH_MODEL,
          VEH_MODEL_DESC,
          POOL_CODE,
          TAC_CODE,
          VEH_BODY,
          VEH_BODY_DESC,
          VEH_CC,
          VEH_FUEL,
          VEH_FUEL_DESC,
          VEH_GVW,
          VEH_SEAT_CAP,
          VEH_NO_DRIVER,
          VAP_PROD_CODE,
          VAP_PROD_CODE_DESC,
          VEH_EFF_FM_DT,
          VEH_EFF_TO_DT,
          DISC_UPTO_5YRS,
          DISC_5_TO_10YRS,
          CreatedBy,
          CreatedOn,
          UpdatedBy,
          UpdatedOn,
          VariantId,
          IsManuallyMapped
          )
        SELECT IC.VEH_MAKE,
          IC.VEH_MAKE_DESC,
          IC.VEH_MODEL,
          IC.VEH_MODEL_DESC,
          IC.POOL_CODE,
          IC.TAC_CODE,
          IC.VEH_BODY,
          IC.VEH_BODY_DESC,
          IC.VEH_CC,
          IC.VEH_FUEL,
          IC.VEH_FUEL_DESC,
          IC.VEH_GVW,
          IC.VEH_SEAT_CAP,
          IC.VEH_NO_DRIVER,
          IC.VAP_PROD_CODE,
          IC.VAP_PROD_CODE_DESC,
          IC.VEH_EFF_FM_DT,
          IC.VEH_EFF_TO_DT,
          IC.DISC_UPTO_5YRS,
          IC.DISC_5_TO_10YRS,
          @UpdatedBy,
          GETDATE(),
          IC.UpdatedBy,
          IC.UpdatedOn,
          NEW.HeroVariantId,
          1
        FROM @newRecordDataTable NEW
        JOIN HeroInsurance.MOTOR.Oriental_VehicleMaster IC ON NEW.ICVehicleCode = IC.Veh_Model
      END
    END

    SELECT 1 isUpdateSuccessFull,
      @UpdatedRecords updatedRecords
  END TRY

  BEGIN CATCH
    DECLARE @StrProcedure_Name VARCHAR(500),
      @ErrorDetail VARCHAR(1000),
      @ParameterList VARCHAR(2000)

    SET @StrProcedure_Name = ERROR_PROCEDURE()
    SET @ErrorDetail = ERROR_MESSAGE()

    EXEC Admin_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name,
      @ErrorDetail = @ErrorDetail,
      @ParameterList = @ParameterList

    SELECT 0 isUpdateSuccessFull,
      0 updatedRecords
  END CATCH
END