using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.UpdateBUStatus;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UpdatePersonalDetails
{
    public class UpdatePersonalDetailsCommand : IRequest<HeroResult<bool>>
    {
        public string? UserId { get; set; }
        public string? AlternateNumber { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public int? Pincode { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? EducationalQualification { get; set; }
        public bool POSPSourceMode { get; set; }
        public string? InsuranceSellingExperience { get; set; }
        public string? ICName { get; set; }
        public string? PremiumSold { get; set; }
        public string? IsSelling { get; set; }
        public string NOCAvailable { get; set; }
        public string? ProductCategoryId { get; set; }

    }
    public class UpdatePersonalDetailsCommandHandler : IRequestHandler<UpdatePersonalDetailsCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateBUDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdatePersonalDetailsCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="updateBUDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdatePersonalDetailsCommand updateCommand, CancellationToken cancellationToken)
        {
            //var updateBUStatus = _mapper.Map<UpdateBUStatusResonse>(updateBUStatusCommand);
            var result = await _userRepository.UpdatePersonalDetails(updateCommand, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
