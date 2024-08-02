using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;
using System.Data;

namespace Admin.Core.Features.Mmv.UpdateVariantsMapping
{
    public class UpdateVariantsMappingCommand : IRequest<HeroResult<UpdateVariantsMappingCommandHandlerVm>>
    {
        public string InsurerId { get; set; }
        public IEnumerable<HeroToIcVariantMappingRequest> HeroToIcVariants { get; set; }
        public IEnumerable<HeroToIcVariantMappingRequest> HeroToIcDuplicateVariant { get; set; }

    }
    public record HeroToIcVariantMappingRequest
    {
        public string heroVariantId { get; set; }
        public string iCVehicleCode { get; set; }
    }
    public class UpdateVariantsMappingCommandHandler : IRequestHandler<UpdateVariantsMappingCommand, HeroResult<UpdateVariantsMappingCommandHandlerVm>>
    {
        private readonly IMmvRepository _immvRepository;
        private readonly IMapper _mapper;
        public UpdateVariantsMappingCommandHandler(IMmvRepository immvRepository, IMapper mapper)
        {
            _immvRepository = immvRepository ?? throw new ArgumentNullException(nameof(immvRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<HeroResult<UpdateVariantsMappingCommandHandlerVm>> Handle(UpdateVariantsMappingCommand updateVariantsMappingCommand, CancellationToken cancellationToken)
        {
            DataTable updateDataTable = CreatePolicyMigrationDatatable();
            if(updateVariantsMappingCommand.HeroToIcVariants is not null)
            {
                foreach (var item in updateVariantsMappingCommand.HeroToIcVariants)
                {
                    DataRow row = updateDataTable.NewRow();
                    row["HeroVariantId"] = item.heroVariantId.ToString();
                    row["ICVehicleCode"] = item.iCVehicleCode.ToString();
                    updateDataTable.Rows.Add(row);
                }
            }

            DataTable newRecordDataTable = CreatePolicyMigrationDatatable();
            if (updateVariantsMappingCommand.HeroToIcDuplicateVariant is not null)
            {
                foreach (var itemNew in updateVariantsMappingCommand.HeroToIcDuplicateVariant)
                {
                    DataRow rowNew = newRecordDataTable.NewRow();
                    rowNew["HeroVariantId"] = itemNew.heroVariantId.ToString();
                    rowNew["ICVehicleCode"] = itemNew.iCVehicleCode.ToString();
                    newRecordDataTable.Rows.Add(rowNew);
                }
            }

            var HeroVariants = await _immvRepository.UpdateVariantsMapping(updateVariantsMappingCommand, updateDataTable, newRecordDataTable, cancellationToken);
            if (HeroVariants is not null)
            {
                var HeroVariantslist = _mapper.Map<UpdateVariantsMappingCommandHandlerVm>(HeroVariants);
                return HeroResult<UpdateVariantsMappingCommandHandlerVm>.Success(HeroVariantslist);
            }
            return HeroResult<UpdateVariantsMappingCommandHandlerVm>.Fail("No Record Found");
        }

        static DataTable CreatePolicyMigrationDatatable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("HeroVariantId", typeof(string));
            dt.Columns.Add("ICVehicleCode", typeof(string));
            return dt;
        }
    }
}
