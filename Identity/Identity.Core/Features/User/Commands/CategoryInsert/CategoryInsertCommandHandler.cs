using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Commands.BUUpdate;
using Identity.Domain.Roles;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Commands.CategoryInsert
{
    public record CategoryInsertCommand : IRequest<HeroResult<bool>>
    {
        public string UserCategoryName { get; set; }
        public string CreatedBy { get; set; }
    }
    public class CategoryInsertCommandHandler : IRequestHandler<CategoryInsertCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _CategoryRepository;
        private readonly IMapper _mapper;

        public CategoryInsertCommandHandler(IUserRepository catRepository, IMapper mapper)
        {
            _CategoryRepository = catRepository ?? throw new ArgumentNullException(nameof(catRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(CategoryInsertCommand catInsertCommand, CancellationToken cancellationToken)
        {
            var buInputModel = _mapper.Map<CategoryInputModel>(catInsertCommand);
            var result = await _CategoryRepository.CategoryInsert(buInputModel, cancellationToken);

            return HeroResult<bool>.Success(result);
        }
    }
}
