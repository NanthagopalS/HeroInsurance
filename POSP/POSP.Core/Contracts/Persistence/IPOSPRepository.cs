using POSP.Core.Features.POSP.Commands.InsertReferralNewUserDetails;
using POSP.Core.Features.POSP.Commands.InsertUserDeviceDetails;
using POSP.Core.Features.POSP.Queries.GetPanRejectionReasons;
using POSP.Core.Features.POSP.Queries.GetPospLastActivityDetails;
using POSP.Domain.POSP;

namespace POSP.Core.Contracts.Persistence;

public interface IPOSPRepository
{
    Task<IEnumerable<TrainingMaterialDetailModel>> GetTrainingMaterialDetail(string ModuleType, string TrainingId, CancellationToken cancellationToken);
    Task<IEnumerable<ExamInstructionsDetailModel>> GetExamInstructionsDetail(CancellationToken cancellationToken);
    Task<IEnumerable<ExamQuestionStatusMasterModel>> GetExamQuestionStatusMaster(CancellationToken cancellationToken);
    Task<IEnumerable<TrainingInstructionsDetailModel>> GetTrainingInstructionsDetail(CancellationToken cancellationToken);
    Task<bool> InsertExamInstructionsDetail(ExamInstructionsDetailModel examInstructionsDetailModel, CancellationToken cancellationToken);
    Task<bool> DeleteExamInstructionsDetail(string Id, CancellationToken cancellationToken);
    Task<bool> UpdateExamInstructionsDetail(ExamInstructionsDetailModel examInstructionsDetailModel, CancellationToken cancellationToken);
    Task<bool> InsertTrainingInstructionsDetail(TrainingInstructionsDetailModel trainingInstructionsDetailModel, CancellationToken cancellationToken);
    Task<bool> UpdateTrainingInstructionsDetail(TrainingInstructionsDetailModel trainingInstructionsDetailModel, CancellationToken cancellationToken);
    Task<bool> DeleteTrainingInstructionsDetail(string Id, CancellationToken cancellationToken);
    Task<IEnumerable<ExamQuestionPaperMasterModel>> GetPOSPExamQuestionPaperMaster(CancellationToken cancellationToken);
    Task<IEnumerable<ExamLanguageMasterModel>> GetPOSPExamLanguageMaster(CancellationToken cancellationToken);
    Task<IEnumerable<ExamPaperDetailModel>> GetPOSPExamPaperDetail(CancellationToken cancellationToken);
    Task<IEnumerable<ExamQuestionPaperOptionModel>> GetExamQuestionPaperOption(CancellationToken cancellationToken);
    Task<IEnumerable<ExamQuestionNavigatorDetailModel>> GetPOSPExamQuestionNavigatorDetail(string UserId, string ExamId, CancellationToken cancellationToken);
    Task<bool> UpdatePOSPExamQuestionAsweredDetail(ExamQuestionAsweredDetailModel examQuestionAsweredDetailModelModel, CancellationToken cancellationToken);
    Task<ExamParticularQuestionDetailResponseModel> GetPOSPExamParticularQuestionDetail(string UserId, string ExamId, int QuestionNo, CancellationToken cancellationToken);
    Task<ExamDetailResponse> InsertPOSPExamDetail(string UserId, string ExamStatus, CancellationToken cancellationToken);
    Task<IEnumerable<ExamResultDetailModel>> GetExamResultDetail(string Id, string UserId, CancellationToken cancellationToken);
    Task<bool> InsertTrainingMaterialDetail(TrainingMaterialDetailModel trainingMaterialDetailModel, CancellationToken cancellationToken);
    Task<bool> UpdateTrainingMaterialDetail(TrainingMaterialDetailModel trainingMaterialDetailModel, CancellationToken cancellationToken);
    Task<bool> DeleteTrainingMaterialDetail(string Id, CancellationToken cancellationToken);
    Task<bool> InsertPOSPTrainingProgressDetail(TrainingProgressDetailModel trainingProgressDetailModel, CancellationToken cancellationToken);
    Task<POSPResponseTrainingModel> InsertPOSPTrainingDetail(string UserId, string TrainingStatus, string TrainingId, CancellationToken cancellationToken);
    Task<POSPTrainingModel> UpdatePOSPTrainingDetail(POSPTrainingModel trainingModel, CancellationToken cancellationToken);
    Task<IEnumerable<POSPTrainingModel>> GetPOSPTraining(CancellationToken cancellationToken);
    Task<IEnumerable<MessageDetailResponseModel>> GetPOSPMessageDetail(string MessageKey, CancellationToken cancellationToken);


    Task<bool> ExamBannerUpload(ExamBannerDetailModel exambannerdetailmodel);
    Task<IEnumerable<ExamBannerDetailModel>> GetExamBannerDetail(CancellationToken cancellationToken);
    Task<IEnumerable<ExamPaperDetailResponseModel>> GetPOSPExamParticularQuestionStatus(string ExamId, CancellationToken cancellationToken);
    Task<bool> ResetPOSPUserAccountDetail(string UserId, CancellationToken cancellationToken);
    Task<IEnumerable<ButtonResponseModel>> GetPOSPButtonDetail(string UserId, CancellationToken cancellationToken);

    Task<IEnumerable<POSPRatingResponseGetModel>> GetPOSPRating(string UserId, CancellationToken cancellationToken);
    Task<POSPRatingResponseModel> InsertPOSPRating(string UserId, int Rating, string Description, CancellationToken cancellationToken);

    Task<bool> ExamCertificateUpload(ExamCertificateModel examcertificatemodel);
    Task<IEnumerable<ExamCertificateModel>> GetExamCertificatDetail(string UserId, CancellationToken cancellationToken);

    public Task<ExamCertificateModel> GetHtmldocuentId(string configurationKey, CancellationToken cancellationToken);

    Task<ExamCertificateModel> InsertDOcumentaId(ExamCertificateModel exambannerdetailmodel);
    Task<bool> AgreementUpload(AgreementModel agreementmodel);
    Task<IEnumerable<POSPAgreementDocumentModel>> GetPOSPAgreementDocument(string UserId, CancellationToken cancellationToken);
    Task<POSPAgreementDocumentModel> InsertAgreementId(POSPAgreementDocumentModel POSPAgreementDocumentModel);
    Task<IEnumerable<GetProductCategoryModel>> GetProductCategory(CancellationToken cancellationToken);
    Task<GetPospReferralDetailsModel> GetPospReferralDetails(string? UserId, CancellationToken cancellationToken);
    Task<IEnumerable<GetPospLastLogInDetailsVm>> GetPospLastLogInDetails(string? UserId, CancellationToken cancellationToken);
    Task<ReferralNewUserDetailsModel> InsertReferralNewUserDetails(InsertReferralNewUserDetailsCommand insertReferralNewUserDetailsCommand, CancellationToken cancellationToken);
    Task<bool> InsertUserDeviceDetails(InsertUserDeviceDetailsCommand insertUserDeviceDetailsCommand, CancellationToken cancellationToken);
    Task<IEnumerable<GetPOSPTestimonialsResponseModel>> GetTestomonialLists(CancellationToken cancellationToken);
    Task<GetPOSPCardDetailResponseModel> GetPOSPCardDetail(CancellationToken cancellationToken);
    Task<IEnumerable<GetPanRejectionReasonsModel>> GetPanRejectionReasons(GetPanRejectionReasonsQuery getPanRejectionReasonsQuery, CancellationToken cancellationToken);
}
