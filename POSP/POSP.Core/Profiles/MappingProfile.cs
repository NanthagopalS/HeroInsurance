using AutoMapper;
using POSP.Core.Features.Command.ImportPOSP;
using POSP.Core.Features.POSP.Commands.DeleteExamInstructionsDetail;
using POSP.Core.Features.POSP.Commands.DeleteTrainingInstructionsDetail;
using POSP.Core.Features.POSP.Commands.DeleteTrainingMaterialDetail;
using POSP.Core.Features.POSP.Commands.InsertAgreement;
using POSP.Core.Features.POSP.Commands.InsertAgreementId;
using POSP.Core.Features.POSP.Commands.InsertCertificateDocumentId;
using POSP.Core.Features.POSP.Commands.InsertExamBanner;
using POSP.Core.Features.POSP.Commands.InsertExamCertificate;
using POSP.Core.Features.POSP.Commands.InsertExamDetail;
using POSP.Core.Features.POSP.Commands.InsertExamInstructionsDetail;
using POSP.Core.Features.POSP.Commands.InsertPOSPRating;
using POSP.Core.Features.POSP.Commands.InsertPOSPTrainingDetail;
using POSP.Core.Features.POSP.Commands.InsertPOSPTrainingProgressDetail;
using POSP.Core.Features.POSP.Commands.InsertReferralNewUserDetails;
using POSP.Core.Features.POSP.Commands.InsertTrainingInstructionsDetail;
using POSP.Core.Features.POSP.Commands.InsertTrainingMaterialDetail;
using POSP.Core.Features.POSP.Commands.InsertUserDeviceDetails;
using POSP.Core.Features.POSP.Commands.UpdateExamInstructionsDetail;
using POSP.Core.Features.POSP.Commands.UpdatePOSPExamQuestionAsweredDetail;
using POSP.Core.Features.POSP.Commands.UpdatePOSPTrainingDetail;
using POSP.Core.Features.POSP.Commands.UpdateTrainingInstructionsDetail;
using POSP.Core.Features.POSP.Commands.UpdateTrainingMaterialDetail;
using POSP.Core.Features.POSP.Queries.GetExamBannerDetail;
using POSP.Core.Features.POSP.Queries.GetExamCertificatedetail;
using POSP.Core.Features.POSP.Queries.GetExamDetail;
using POSP.Core.Features.POSP.Queries.GetExamInstructionsDetail;
using POSP.Core.Features.POSP.Queries.GetExamLanguageMaster;
using POSP.Core.Features.POSP.Queries.GetExamPaperDetail;
using POSP.Core.Features.POSP.Queries.GetExamParticularQuestionDetail;
using POSP.Core.Features.POSP.Queries.GetExamParticularQuestionStatus;
using POSP.Core.Features.POSP.Queries.GetExamQuestionPaperMaster;
using POSP.Core.Features.POSP.Queries.GetExamQuestionPaperOption;
using POSP.Core.Features.POSP.Queries.GetExamQuestionStatusMaster;
using POSP.Core.Features.POSP.Queries.GetFeedbackList;
using POSP.Core.Features.POSP.Queries.GetHtmlDocumentId;
using POSP.Core.Features.POSP.Queries.GetManagePOSPTraining;
using POSP.Core.Features.POSP.Queries.GetPanRejectionReasons;
using POSP.Core.Features.POSP.Queries.GetPOSPAgreementDocument;
using POSP.Core.Features.POSP.Queries.GetPOSPButtonDetail;
using POSP.Core.Features.POSP.Queries.GetPOSPCardDetail;
using POSP.Core.Features.POSP.Queries.GetPOSPExamQuestionNavigatorDetail;
using POSP.Core.Features.POSP.Queries.GetPospLastActivityDetails;
using POSP.Core.Features.POSP.Queries.GetPOSPMessageDetail;
using POSP.Core.Features.POSP.Queries.GetPOSPRating;
using POSP.Core.Features.POSP.Queries.GetPospReferralDetails;
using POSP.Core.Features.POSP.Queries.GetProductCategory;
using POSP.Core.Features.POSP.Queries.GetTrainingInstructionsDetail;
using POSP.Core.Features.POSP.Queries.GetTrainingMaterial;
using POSP.Core.Features.Reports;
using POSP.Domain.Migration;
using POSP.Domain.POSP;
using POSP.Domain.Reports;

namespace POSP.Core.Profiles;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TrainingMaterialDetailModel, GetPOSPTrainingMaterialDetailVm>();
        CreateMap<POSPTrainingModel, GetPOSPTrainingVm>();
        CreateMap<ExamInstructionsDetailModel, GetExamInstructionsDetailVm>();
        CreateMap<ExamQuestionStatusMasterModel, GetExamQuestionStatusMasterVm>();
        CreateMap<TrainingInstructionsDetailModel, GetTrainingInstructionsDetailVm>();
        CreateMap<InsertExamInstructionsDetailCommand, ExamInstructionsDetailModel>();
        CreateMap<DeleteExamInstructionsDetailCommand, ExamInstructionsDetailModel>();
        CreateMap<UpdateExamInstructionsDetailCommand, ExamInstructionsDetailModel>();
        CreateMap<ExamBannerUploadCommand, ExamBannerDetailModel>();
        CreateMap<ExamCertificateUploadCommand, ExamCertificateModel>();
        CreateMap<CertificacateCommand, ExamCertificateModel>();
        CreateMap<InsertExamDetailCommand, ExamDetailResponse>();
        CreateMap<InsertTrainingInstructionsDetailCommand, TrainingInstructionsDetailModel>();
        CreateMap<DeleteTrainingInstructionsDetailCommand, TrainingInstructionsDetailModel>();
        CreateMap<UpdateTrainingInstructionsDetailCommand, TrainingInstructionsDetailModel>();
        CreateMap<ExamQuestionPaperOptionModel, GetExamQuestionPaperOptionMasterVm>();
        CreateMap<ExamResultDetailModel, GetExamDetailVm>();
        CreateMap<ExamDetailResponse, GetExamDetailVm>();
        CreateMap<ExamBannerDetailModel, GetExamBannerDetailVm>();
        CreateMap<ExamCertificateModel, GetExamCertificateDetailVm>();
        CreateMap<ExamCertificateModel, GetHtmlDocumentDetailVm>();

        CreateMap<ExamQuestionPaperMasterModel, GetPOSPExamQuestionPaperMasterVm>();
        CreateMap<ExamLanguageMasterModel, GetPOSPExamLanguageMasterVm>();
        CreateMap<ExamPaperDetailModel, GetPOSPExamPaperDetailVm>();
        CreateMap<ExamQuestionNavigatorDetailModel, GetPOSPExamQuestionNavigatorDetailVm>();
        CreateMap<UpdatePOSPExamQuestionAsweredDetailCommand, ExamQuestionAsweredDetailModel>();

        CreateMap<InsertTrainingMaterialDetailCommand, TrainingMaterialDetailModel>();
        CreateMap<UpdateTrainingMaterialDetailCommand, TrainingMaterialDetailModel>();
        CreateMap<DeleteTrainingMaterialDetailCommand, TrainingMaterialDetailModel>();
        CreateMap<InsertPOSPTrainingProgressDetailCommand, TrainingProgressDetailModel>();

        CreateMap<InsertPOSPTrainingDetailCommand, POSPResponseTrainingModel>();
        CreateMap<UpdatePOSPTrainingDetailCommand, POSPTrainingModel>();
        CreateMap<ExamParticularQuestionDetailResponseModel, GetPOSPExamParticularQuestionDetailVm>();
        CreateMap<MessageDetailResponseModel, GetPOSPMessageDetailVm>();
        CreateMap<ExamPaperDetailResponseModel, GetPOSPExamParticularQuestionStatusVm>();
        CreateMap<ButtonResponseModel, GetPOSPButtonDetailVm>();
        CreateMap<POSPRatingResponseGetModel, GetPOSPRatingVm>();
        CreateMap<InsertPOSPRatingCommand, POSPRatingResponseModel>();

        CreateMap<AgreementUploadCommand, AgreementModel>();

        CreateMap<POSPAgreementDocumentModel, GetPOSPAgreementDocumentVm>();
        CreateMap<InsertAgreementIdCommand, POSPAgreementDocumentModel>();
        CreateMap<GetProductCategoryModel, GetProductCategoryVm>();

        CreateMap<GetProductCategoryModel, GetProductCategoryVm>();
        CreateMap<GetPospReferralDetailsModel, GetPospReferralDetailsVm>();

        CreateMap<PospLastActivityDetailsModel, GetPospLastLogInDetailsQuery>();

        CreateMap<InsertReferralNewUserDetailsCommand, ReferralNewUserDetailsModel>();
        CreateMap<InsertUserDeviceDetailsCommand, InsertUserDeviceDetailsModel>();
        CreateMap<NewAndOldPOSPReportResponceModel, NewAndOldPOSPReportVm>();
        CreateMap<POSPMigrationResponceModal, POSPMigrationResponceModalVm>();
        CreateMap<GetPOSPTestimonialsResponseModel, GetPOSPTestimonialsVm>();
        CreateMap<GetPOSPCardDetailResponseModel, GetPOSPCardDetailVm>();
        CreateMap<GetPanRejectionReasonsModel, GetPanRejectionReasonsVm>();
    }

}
