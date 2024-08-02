using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster;

/// <summary>
/// GetMakeModelQuery
/// CVCategoryId
/// </summary>
public record GetMakeModelQuery : IRequest<HeroResult<IEnumerable<MakeModelVm>>>
{
    public string VehicleType { get; set; }
    public string CVCategoryId { get; set; }
}

public class GetMakeModelQueryHandler : IRequestHandler<GetMakeModelQuery, HeroResult<IEnumerable<MakeModelVm>>>
{
    private readonly IInsuranceMasterRepository _quoteRepository;

    /// <summary>
    /// Initialize
    /// </summary>
    public GetMakeModelQueryHandler(IInsuranceMasterRepository quoteRepository)
    {
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
    }

    public async Task<HeroResult<IEnumerable<MakeModelVm>>> Handle(GetMakeModelQuery request, CancellationToken cancellationToken)
    {
        var makeModelFuelResult = await _quoteRepository.GetMakeModel(request, cancellationToken).ConfigureAwait(false);

        if (makeModelFuelResult is not null)
        {
            var lstMakeModel = new List<MakeModelVm>();
            foreach (var item in makeModelFuelResult.MakeList)
            {
                var lstModels = makeModelFuelResult.ModelList
                    .Where(x => x.MakeId.Equals(item.MakeId))
                    .Select(d => new ModelVm
                    {
                        ModelId = d.ModelId,
                        ModelName = d.ModelName,
                        IsPopularModel = d.IsPopularModel
                    }).ToList();

                lstMakeModel.Add(new MakeModelVm()
                {
                    MakeId = item.MakeId,
                    MakeName = item.MakeName,
                    ImageURL = item.ImageURL,
                    ModelVms = lstModels
                });
            }
            return HeroResult<IEnumerable<MakeModelVm>>.Success(lstMakeModel);
        }

        return HeroResult<IEnumerable<MakeModelVm>>.Fail("No record found");
    }
}
