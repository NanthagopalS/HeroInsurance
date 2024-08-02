using AutoMapper;
using Insurance.Core.Features.Quote.Query.GetPropoalField;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Persistence.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //CreateMap<CreateLeadModel, ProposalFieldMasterModel>()
        //              .ForMember(d => d.FieldKey, o => o.MapFrom((src,dest)  => MapValue(dest.FieldKey,src)));
        
    }

    private static string MapValue(string param, ProposalFieldMasterModel proposalFieldMasterModel)
    {
        return string.Empty;
    }
}
