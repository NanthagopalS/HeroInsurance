using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using POSP.Core.Contracts.Common;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Commands.InsertReferralNewUserDetails;
using POSP.Core.Features.POSP.Commands.InsertUserDeviceDetails;
using POSP.Core.Features.POSP.Queries.GetPanRejectionReasons;
using POSP.Core.Features.POSP.Queries.GetPospLastActivityDetails;
using POSP.Domain.POSP;
using POSP.Persistence.Configuration;
using System.Data;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Models.JWT;

namespace POSP.Persistence.Repository;

public class POSPRepository : IPOSPRepository
{
    private readonly ApplicationDBContext _context;
    private readonly IMongoDBService _mongodbService;
    private readonly IEmailService _emailService;
    private readonly ISmsService _sMSService;
    private readonly IConfiguration _config;
    private readonly IApplicationClaims _applicationClaims;

    public POSPRepository(ApplicationDBContext context, ISmsService sMSService, IEmailService emailService, IMongoDBService mongodbService, IOptions<TokenSettings> tokenSettings, IApplicationClaims applicationClaims, IConfiguration config)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _sMSService = sMSService ?? throw new ArgumentNullException(nameof(sMSService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));

    }


    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public POSPRepository(ApplicationDBContext context, IMongoDBService mongodbService, IEmailService emailService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

    }

    public async Task<IEnumerable<TrainingMaterialDetailModel>> GetTrainingMaterialDetail(string ModuleType, string TrainingId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("ModuleType", ModuleType, DbType.String, ParameterDirection.Input);
        parameters.Add("TrainingId", TrainingId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<TrainingMaterialDetailModel>("[dbo].[POSP_GetTrainingMaterialDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        foreach (var item in result)
        {
            //item.Image64 = await _mongodbService.MongoDownload("63b670462b67c64123803a07");
            item.Image64 = await _mongodbService.MongoDownload(item.DocumentId);
        }

        return result;
    }

    public async Task<IEnumerable<POSPTrainingModel>> GetPOSPTraining(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<POSPTrainingModel>("[dbo].[POSP_GetTraining]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// GetExamInstructionsDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<ExamInstructionsDetailModel>> GetExamInstructionsDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<ExamInstructionsDetailModel>("[dbo].[POSP_GetExamInstructionsDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// GetExamQuestionStatusMaster
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<ExamQuestionStatusMasterModel>> GetExamQuestionStatusMaster(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<ExamQuestionStatusMasterModel>("[dbo].[POSP_GetExamQuestionStatusMaster]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// GetTrainingInstructionsDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TrainingInstructionsDetailModel>> GetTrainingInstructionsDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<TrainingInstructionsDetailModel>("[dbo].[POSP_GetTrainingInstructionsDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }
    /// <summary>
    /// InsertExamInstructionsDetail
    /// </summary>
    /// <param name="examInstructionsDetailModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> InsertExamInstructionsDetail(ExamInstructionsDetailModel examInstructionsDetailModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("InstructionDetail", examInstructionsDetailModel.InstructionDetail, DbType.String, ParameterDirection.Input);
        parameters.Add("PriorityIndex", examInstructionsDetailModel.PriorityIndex, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[POSP_InsertExamInstructionsDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }
    /// <summary>
    /// DeleteExamInstructionsDetail
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteExamInstructionsDetail(string Id, CancellationToken cancellationToken)
    {
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", Id, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[POSP_DeleteExamInstructionsDetail]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);

        }
    }

    /// <summary>
    /// UpdateExamInstructionsDetail
    /// </summary>
    /// <param name="examInstructionsDetailModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> UpdateExamInstructionsDetail(ExamInstructionsDetailModel examInstructionsDetailModel, CancellationToken cancellationToken)
    {

        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", examInstructionsDetailModel.Id, DbType.String, ParameterDirection.Input);
            parameters.Add("InstructionDetail", examInstructionsDetailModel.InstructionDetail, DbType.String, ParameterDirection.Input);
            parameters.Add("PriorityIndex", examInstructionsDetailModel.PriorityIndex, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[POSP_UpdateExamInstructionsDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return (result > 0);

        }
    }


    /// <summary>
    /// Insert TrainingInstructionsDetail
    /// </summary>
    /// <param name="trainingInstructionsDetailModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> InsertTrainingInstructionsDetail(TrainingInstructionsDetailModel trainingInstructionsDetailModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("InstructionDetail", trainingInstructionsDetailModel.InstructionDetail, DbType.String, ParameterDirection.Input);
        parameters.Add("PriorityIndex", trainingInstructionsDetailModel.PriorityIndex, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[POSP_InsertTrainingInstructionsDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }
    /// <summary>
    /// DeleteExamInstructionsDetail
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteTrainingInstructionsDetail(string Id, CancellationToken cancellationToken)
    {
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", Id, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[POSP_DeleteTrainingInstructionsDetail]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);

        }
    }

    /// <summary>
    /// Update TrainingInstructionsDetail
    /// </summary>
    /// <param name="trainingInstructionsDetailModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> UpdateTrainingInstructionsDetail(TrainingInstructionsDetailModel trainingInstructionsDetailModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("Id", trainingInstructionsDetailModel.Id, DbType.String, ParameterDirection.Input);
        parameters.Add("InstructionDetail", trainingInstructionsDetailModel.InstructionDetail, DbType.String, ParameterDirection.Input);
        parameters.Add("PriorityIndex", trainingInstructionsDetailModel.PriorityIndex, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[POSP_UpdateTrainingInstructionsDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }



    public async Task<IEnumerable<ExamQuestionPaperOptionModel>> GetExamQuestionPaperOption(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<ExamQuestionPaperOptionModel>("[dbo].[POSP_GetExamQuestionPaperOptionMaster]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }

    public async Task<IEnumerable<ExamQuestionPaperMasterModel>> GetPOSPExamQuestionPaperMaster(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<ExamQuestionPaperMasterModel>("[dbo].[POSP_GetExamQuestionPaperMaster]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }


    public async Task<IEnumerable<ExamLanguageMasterModel>> GetPOSPExamLanguageMaster(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<ExamLanguageMasterModel>("[dbo].[POSP_GetExamLanguageMaster]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }


    public async Task<IEnumerable<ExamPaperDetailModel>> GetPOSPExamPaperDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<ExamPaperDetailModel>("[dbo].[POSP_GetExamPaperDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }


    /// <summary>
    /// UpdatePOSPTrainingDetail
    /// </summary>
    /// <param name="examInstructionsDetailModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<POSPTrainingModel> UpdatePOSPTrainingDetail(POSPTrainingModel trainingModel, CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", trainingModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("TrainingModuleType", trainingModel.TrainingModuleType, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<POSPTrainingModel>("[dbo].[POSP_UpdatePOSPTrainingDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        if (result.Any())
        {

            return result.FirstOrDefault();
        }
        return default;
    }

    public async Task<IEnumerable<ExamResultDetailModel>> GetExamResultDetail(string Id, string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("Id", Id, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ExamResultDetailModel>("[dbo].[POSP_GetExamDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<ExamDetailResponse> InsertPOSPExamDetail(string UserId, string ExamStatus, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("ExamStatus", ExamStatus, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<ExamDetailResponse>("[dbo].[POSP_InsertPOSPExamDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        if (result.Any())
        {

            return result.FirstOrDefault();
        }

        return default;
    }

    /// <summary>
    /// InsertTrainingMaterialDetail
    /// </summary>
    /// <param name="examInstructionsDetailModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> InsertTrainingMaterialDetail(TrainingMaterialDetailModel trainingMaterialDetailModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("TrainingModuleType", trainingMaterialDetailModel.TrainingModuleType, DbType.String, ParameterDirection.Input);
        parameters.Add("MaterialFormatType", trainingMaterialDetailModel.MaterialFormatType, DbType.String, ParameterDirection.Input);
        parameters.Add("VideoDuration", trainingMaterialDetailModel.VideoDuration, DbType.String, ParameterDirection.Input);
        parameters.Add("LessonNumber", trainingMaterialDetailModel.LessonNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("LessonTitle", trainingMaterialDetailModel.LessonTitle, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentFileName", trainingMaterialDetailModel.DocumentFileName, DbType.String, ParameterDirection.Input);
        parameters.Add("PriorityIndex", trainingMaterialDetailModel.PriorityIndex, DbType.String, ParameterDirection.Input);


        var result = await connection.ExecuteAsync("[dbo].[POSP_InsertTrainingMaterialDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    /// <summary>
    /// UpdateTrainingMaterialDetail
    /// </summary>
    /// <param name="examInstructionsDetailModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> UpdateTrainingMaterialDetail(TrainingMaterialDetailModel trainingMaterialDetailModel, CancellationToken cancellationToken)
    {

        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", trainingMaterialDetailModel.Id, DbType.String, ParameterDirection.Input);
            parameters.Add("TrainingModuleType", trainingMaterialDetailModel.TrainingModuleType, DbType.String, ParameterDirection.Input);
            parameters.Add("MaterialFormatType", trainingMaterialDetailModel.MaterialFormatType, DbType.String, ParameterDirection.Input);
            parameters.Add("VideoDuration", trainingMaterialDetailModel.VideoDuration, DbType.String, ParameterDirection.Input);
            parameters.Add("LessonNumber", trainingMaterialDetailModel.LessonNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("LessonTitle", trainingMaterialDetailModel.LessonTitle, DbType.String, ParameterDirection.Input);
            parameters.Add("DocumentFileName", trainingMaterialDetailModel.DocumentFileName, DbType.String, ParameterDirection.Input);
            parameters.Add("PriorityIndex", trainingMaterialDetailModel.PriorityIndex, DbType.String, ParameterDirection.Input);

            var result = await connection.ExecuteAsync("[dbo].[POSP_UpdateTrainingMaterialDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return (result > 0);

        }
    }

    /// <summary>
    /// DeleteTrainingMaterialDetail
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteTrainingMaterialDetail(string Id, CancellationToken cancellationToken)
    {
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", Id, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[POSP_DeleteTrainingMaterialDetail]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);

        }
    }

    /// <summary>
    /// InsertPOSPTrainingProgressDetail
    /// </summary>
    /// <param name="trainingProgressDetailModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> InsertPOSPTrainingProgressDetail(TrainingProgressDetailModel trainingProgressDetailModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", trainingProgressDetailModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("TrainingMaterialId", trainingProgressDetailModel.TrainingMaterialId, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[POSP_InsertPOSPTrainingProgressDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }


    public async Task<bool> ResetPOSPUserAccountDetail(string UserId, CancellationToken cancellationToken)
    {
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[POSP_ResetPOSPDatabaseAccount]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);

        }
    }



    public async Task<ExamParticularQuestionDetailResponseModel> GetPOSPExamParticularQuestionDetail(string UserId, string ExamId, int QuestionNo, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("ExamId", ExamId, DbType.String, ParameterDirection.Input);
        parameters.Add("QuestionNo", QuestionNo, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[POSP_GetPOSPExamParticularQuestionDetail]", parameters,
           commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        ExamParticularQuestionDetailResponseModel response = new()
        {
            ExamParticularDetailslist = result.Read<ExamParticularDetail>(),
            OptionResponsesList = result.Read<OptionResponse>(),
        };

        return response;

    }
    public async Task<IEnumerable<ExamQuestionNavigatorDetailModel>> GetPOSPExamQuestionNavigatorDetail(string UserId, string ExamId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("ExamId", ExamId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ExamQuestionNavigatorDetailModel>("[dbo].[POSP_GetPOSPExamQuetionNavigatorDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<bool> UpdatePOSPExamQuestionAsweredDetail(ExamQuestionAsweredDetailModel examQuestionAsweredDetailModelModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", examQuestionAsweredDetailModelModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("ExamId", examQuestionAsweredDetailModelModel.ExamId, DbType.String, ParameterDirection.Input);
        parameters.Add("QuestionNo", examQuestionAsweredDetailModelModel.QuestionNo, DbType.String, ParameterDirection.Input);
        parameters.Add("QuestionId", examQuestionAsweredDetailModelModel.QuestionId, DbType.String, ParameterDirection.Input);
        parameters.Add("AnswerOptionId", examQuestionAsweredDetailModelModel.AnswerOptionId, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[POSP_UpdatePOSPExamQuestionAsweredDetail]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    public async Task<bool> ExamBannerUpload(ExamBannerDetailModel bannerdetailmodel)
    {
        Stream stream = new MemoryStream(bannerdetailmodel.BannnerImage);
        var id = await _mongodbService.MongoUpload(bannerdetailmodel.BannerFileName, stream, bannerdetailmodel.BannnerImage);
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("BannerFileName", bannerdetailmodel.BannerFileName, DbType.String, ParameterDirection.Input);
        parameters.Add("BannerStoragePath", bannerdetailmodel.BannerStoragePath, DbType.String, ParameterDirection.Input);
        parameters.Add("BannerType", bannerdetailmodel.BannerType, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentId", id, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[POSP_InsertExamBannerDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return (result > 0);
    }

    public async Task<IEnumerable<ExamBannerDetailModel>> GetExamBannerDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<ExamBannerDetailModel>("[dbo].[POSP_GetExamBannerDetail]", commandType: CommandType.StoredProcedure);
        foreach (var item in result)
        {
            //item.Image64 = await _mongodbService.MongoDownload("63b670462b67c64123803a07");
            item.Image64 = await _mongodbService.MongoDownload(item.DocumentId);
        }

        return result;
    }
    public async Task<IEnumerable<MessageDetailResponseModel>> GetPOSPMessageDetail(string MessageKey, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("MessageKey", MessageKey, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<MessageDetailResponseModel>("[dbo].[POSP_GetPOSPMessageDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<IEnumerable<ExamPaperDetailResponseModel>> GetPOSPExamParticularQuestionStatus(string ExamId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("ExamId", ExamId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ExamPaperDetailResponseModel>("[dbo].[POSP_GetPOSPExamParticularQuestionStatus]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<POSPResponseTrainingModel> InsertPOSPTrainingDetail(string UserId, string TrainingStatus, string TrainingId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("TrainingStatus", TrainingStatus, DbType.String, ParameterDirection.Input);
        parameters.Add("TrainingId", TrainingId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<POSPResponseTrainingModel>("[dbo].[POSP_InsertPOSPTrainingDetail]", parameters,
                  commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        if (result.Any())
        {

            return result.FirstOrDefault();
        }

        return default;
    }

    public async Task<IEnumerable<ButtonResponseModel>> GetPOSPButtonDetail(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ButtonResponseModel>("[dbo].[POSP_GetPOSPButtonDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<IEnumerable<POSPRatingResponseGetModel>> GetPOSPRating(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<POSPRatingResponseGetModel>("[dbo].[POSP_GetPOSPRating]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<POSPRatingResponseModel> InsertPOSPRating(string UserId, int Rating, string Description, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("Rating", Rating, DbType.Int64, ParameterDirection.Input);
        parameters.Add("Description", Description, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<POSPRatingResponseModel>("[dbo].[POSP_InsertPOSPRating]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        if (result.Any())
        {

            return result.FirstOrDefault();
        }

        return default;
    }

    public async Task<bool> ExamCertificateUpload(ExamCertificateModel examcertificatemodel)
    {
        Stream stream = new MemoryStream(examcertificatemodel.BannnerImage);
        var id = await _mongodbService.MongoCertificateUpload(examcertificatemodel.BannerFileName, stream, examcertificatemodel.BannnerImage);
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("ConfigurationKey", "ExamCertificate", DbType.String, ParameterDirection.Input);
        parameters.Add("ConfigurationValue", id, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[POSP_InsertCertifictaeDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return (result > 0);
    }

    public async Task<IEnumerable<ExamCertificateModel>> GetExamCertificatDetail(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ExamCertificateModel>("[dbo].[POSP_GetUserCertificateDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }
    //Add parameter ConfigurationKey => ExamCertificate /  POSPAgreementFormat
    public async Task<ExamCertificateModel> GetHtmldocuentId(string configurationKey, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("ConfigurationKey", configurationKey, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ExamCertificateModel>("[dbo].[POSP_GetHtmlDocumentId]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        if (result.Any())
        {

            return result.FirstOrDefault();
        }

        return default;
    }

    public async Task<ExamCertificateModel> InsertDOcumentaId(ExamCertificateModel exambannerdetailmodel)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", exambannerdetailmodel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentId", exambannerdetailmodel.DocumentId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ExamCertificateModel>("[dbo].[POSP_InsertCertificateDocument]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result.FirstOrDefault();
    }

    public async Task<bool> AgreementUpload(AgreementModel agreementModel)
    {
        try
        {
            Stream stream = new MemoryStream(agreementModel.SignatureImage);
            var id = await _mongodbService.MongoSignatureUpload("POSP-Signature", stream, agreementModel.SignatureImage); // Agreement => POSP-Signature
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("UserId", agreementModel.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("SignatureId", id, DbType.String, ParameterDirection.Input);
            parameters.Add("ProcessType", "Signature", DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[POSP_InsertAgreementDetail]", parameters,
                            commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            var userParameters = new DynamicParameters();
            userParameters.Add("UserId", agreementModel.UserId, DbType.String, ParameterDirection.Input);
            var userDetails = await connection.QueryAsync<AgreementModel>("[dbo].[POSP_GetUserDetail]", userParameters,
                        commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            foreach (var item in userDetails)
            {
                await SendEmail(item.UserId, item.EmailId, item.UserName, item.POSPId, item.MobileNo);
            }
            return result > 0;
        }
        catch (Exception)
        {

            throw;
        }

        //Stream stream = new MemoryStream(agreementModel.SignatureImage);
        //var id = await _mongodbService.MongoSignatureUpload("POSP-Signature", stream, agreementModel.SignatureImage); // Agreement => POSP-Signature
        //using var connection = _context.CreateConnection();
        //var parameters = new DynamicParameters();
        //parameters.Add("UserId", agreementModel.UserId, DbType.String, ParameterDirection.Input);
        //parameters.Add("SignatureId", id, DbType.String, ParameterDirection.Input);
        //parameters.Add("ProcessType", "Signature", DbType.String, ParameterDirection.Input);
        //var result = await connection.ExecuteAsync("[dbo].[POSP_InsertAgreementDetail]", parameters,
        //                commandType: CommandType.StoredProcedure).ConfigureAwait(false);


        //await SendEmail(agreementModel.UserId, agreementModel.EmailId, agreementModel.UserName, agreementModel.POSPId, agreementModel.MobileNo).ConfigureAwait(false);

        //return result > 0;




        // return (result > 0);
        //var userDetail = result > 0;

        //if (result > 0)
        //{
        //    await SendEmail(agreementModel.UserId, agreementModel.EmailId, agreementModel.UserName, agreementModel.POSPId, agreementModel.MobileNo).ConfigureAwait(false);

        //    return true;
        //}
        //else
        //    return false;

    }



    private async Task SendEmail(string UserId, string EmailId, string UserName, string POSPId, string MobileNo)
    {
        await _emailService.SendWelcomeMail(UserId, EmailId, UserName, POSPId, MobileNo, CancellationToken.None);
    }

    public async Task<IEnumerable<POSPAgreementDocumentModel>> GetPOSPAgreementDocument(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<POSPAgreementDocumentModel>("[dbo].[POSP_GetPOSPAgreementDocument]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<POSPAgreementDocumentModel> InsertAgreementId(POSPAgreementDocumentModel POSPAgreementDocumentModel)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", POSPAgreementDocumentModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("AgreementId", POSPAgreementDocumentModel.AgreementId, DbType.String, ParameterDirection.Input);
        parameters.Add("ProcessType", POSPAgreementDocumentModel.ProcessType, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<POSPAgreementDocumentModel>("[dbo].[POSP_InsertAgreementDocument]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result.FirstOrDefault();
    }

    //public async Task<POSPAgreementDocumentModel> InsertPreSignedAgreementId(POSPAgreementDocumentModel POSPAgreementDocumentModel)
    //{
    //    using var connection = _context.CreateConnection();
    //    var parameters = new DynamicParameters();
    //    parameters.Add("UserId", POSPAgreementDocumentModel.UserId, DbType.String, ParameterDirection.Input);
    //    parameters.Add("AgreementId", POSPAgreementDocumentModel.AgreementId, DbType.String, ParameterDirection.Input);
    //    parameters.Add("ProcessType", "PreSignAgreement", DbType.String, ParameterDirection.Input);
    //    var result = await connection.QueryAsync<POSPAgreementDocumentModel>("[dbo].[POSP_InsertAgreementDocument]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
    //    return result.FirstOrDefault();
    //}

    /// <summary>
    /// GetProductCategory
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<GetProductCategoryModel>> GetProductCategory(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<GetProductCategoryModel>("[dbo].[POSP_GetProductCategory]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }


    public async Task<GetPospReferralDetailsModel> GetPospReferralDetails(string? UserId, CancellationToken cancellationToken)
    {
        string baseUrl = _config.GetSection("WebUrl").GetSection("prefixUrl").Value;
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);


        var result = await connection.QueryMultipleAsync("[dbo].[POSP_GetPospReferralDetails]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        GetPospReferralDetailsModel response = new()
        {
            ReferralTypeModel = result.Read<ReferralTypeModel>(),
            RefferalMode = result.Read<RefferalMode>(),
            RefferalDetails = result.Read<RefferalDetails>()
        };
        foreach (var referraltypesingle in response.ReferralTypeModel)
        {
            referraltypesingle.ReferralBaseURL = baseUrl + referraltypesingle.ReferralBaseURL;
            referraltypesingle.ImageURL = baseUrl + referraltypesingle.ImageURL;
        }
        foreach (var referelSingle in response.RefferalMode)
        {
            referelSingle.ImageUrl = baseUrl + referelSingle.ImageUrl;
        }

        return response;
    }

    public async Task<IEnumerable<GetPospLastLogInDetailsVm>> GetPospLastLogInDetails(string? UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetPospLastLogInDetailsVm>("[dbo].[POSP_GetPospLastLogInDetails]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<ReferralNewUserDetailsModel> InsertReferralNewUserDetails(InsertReferralNewUserDetailsCommand referralNewUserDetailsModel, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("ReferralUserId", referralNewUserDetailsModel.ReferralUserId, DbType.String, ParameterDirection.Input);
            parameters.Add("ReferralMode", referralNewUserDetailsModel.ReferralMode, DbType.String, ParameterDirection.Input);
            parameters.Add("UserName", referralNewUserDetailsModel.UserName, DbType.String, ParameterDirection.Input);
            parameters.Add("EmailId", referralNewUserDetailsModel.EmailId, DbType.String, ParameterDirection.Input);
            parameters.Add("PhoneNumber", referralNewUserDetailsModel.PhoneNumber, DbType.String, ParameterDirection.Input);

            var response = await connection.QueryAsync<ReferralNewUserDetailsModel>("[dbo].[POSP_InsertReferralNewUserDetails]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(response.FirstOrDefault().ReferralId))
            {
                if (referralNewUserDetailsModel.ReferralMode == "EMAIL")
                {
                    await _emailService.SendEmailToReferralUser(referralNewUserDetailsModel.EmailId,
                       referralNewUserDetailsModel.ReferralUserId, referralNewUserDetailsModel.UserName, response.FirstOrDefault().POSPName, CancellationToken.None).ConfigureAwait(false);
                }
                else if (referralNewUserDetailsModel.ReferralMode == "SMS")
                {
                    await _sMSService.SendSMSToReferralUser(referralNewUserDetailsModel.PhoneNumber, referralNewUserDetailsModel.UserName, response.FirstOrDefault().POSPName, referralNewUserDetailsModel.ReferralUserId,
                       CancellationToken.None).ConfigureAwait(false);
                }
            }
            return response.FirstOrDefault();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> InsertUserDeviceDetails(InsertUserDeviceDetailsCommand insertUserDeviceDetailsCommand, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", insertUserDeviceDetailsCommand.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@MobileDeviceId", insertUserDeviceDetailsCommand.MobileDeviceId, DbType.String, ParameterDirection.Input);
        parameters.Add("@BrowserId", insertUserDeviceDetailsCommand.BrowserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@GfcToken", insertUserDeviceDetailsCommand.GfcToken, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[POSP_InsertUserDeviceDetails]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result > 0;
    }
    /// <summary>
    /// GetTestomonialLists
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
	public async Task<IEnumerable<GetPOSPTestimonialsResponseModel>> GetTestomonialLists(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<GetPOSPTestimonialsResponseModel>("[dbo].[POSP_GetTestimonialsProfiles]", parameters, commandType: CommandType.StoredProcedure);
        return result;
    }


    public async Task<GetPOSPCardDetailResponseModel> GetPOSPCardDetail(CancellationToken cancellationToken)
    {
        var userid = _applicationClaims.GetUserId();
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetPOSPCardDetailResponseModel>("[dbo].[POSP_GetCardDetails]", parameters, commandType: CommandType.StoredProcedure);

        return result.First();
    }

    public async Task<IEnumerable<GetPanRejectionReasonsModel>> GetPanRejectionReasons(GetPanRejectionReasonsQuery getPanRejectionReasonsQuery ,CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<GetPanRejectionReasonsModel>("[dbo].[POSP_GetPanRejectionReasons]", parameters, commandType: CommandType.StoredProcedure);
        if(result.Any())
        {
            return result;
        }
        return default;
    }
}
