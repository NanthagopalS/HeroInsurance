using Admin.Core.Features.Mmv.GetHeroVariantLists;
using Admin.Core.Features.Mmv.ResetMvvMappingForIcVariant;
using Admin.Core.Features.Mmv.UpdateVariantsMapping;
using Admin.Core.Features.Mmv.VariantMappingStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Helpers;

namespace Admin.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class MmvController : ControllerBase
    {
        private readonly IMediator _mediatr;
        public MmvController(IMediator mediatr)
        {
            _mediatr = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
        }
        #region - GetHeroVariantLists -
        /// <summary>
        /// GetHeroVariantLists
        /// </summary>
        /// <param name="getHeroVariantListsQuery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetHeroVariantLists")]
        [ProducesResponseType(typeof(GetHeroVariantListsQueryVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetHeroVariantListsQueryVm>> GetAllPOSPCountDetail(GetHeroVariantListsQuery getHeroVariantListsQuery, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(getHeroVariantListsQuery, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError(result.Messages);
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - UpdateVariantsMapping -
        /// <summary>
        /// UpdateVariantsMapping
        /// </summary>
        /// <param name="updateVariantsMappingCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateVariantsMapping")]
        [ProducesResponseType(typeof(UpdateVariantsMappingCommandHandlerVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<UpdateVariantsMappingCommandHandlerVm>> UpdateVariantsMapping(UpdateVariantsMappingCommand updateVariantsMappingCommand, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(updateVariantsMappingCommand, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError(result.Messages);
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - GetCustomMmvSearch -
        /// <summary>
        /// GetCustomMmvSearch
        /// </summary>
        /// <param name="getCustomMmvSearchQuery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
		[Route("GetCustomMmvSearch")]
		[ProducesResponseType(typeof(GetCustomMmvSearchVm), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
		public async Task<ActionResult<IEnumerable<GetCustomMmvSearchVm>>> GetCustomMmvSearch(GetCustomMmvSearchQuery getCustomMmvSearchQuery, CancellationToken cancellationToken)
		{
			var result = await _mediatr.Send(getCustomMmvSearchQuery, cancellationToken);
			if (result.Failed)
			{
				var errorDetails = Result.CreateNotFoundError(result.Messages);
				return NotFound(errorDetails);
			}
			var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
			return Ok(res);
		}
        #endregion

        #region - ResetMvvMappingForIcVariant -
        /// <summary>
        /// ResetMvvMappingForIcVariant
        /// </summary>
        /// <param name="getCustomMmvSearchQuery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ResetMvvMappingForIcVariant")]
        [ProducesResponseType(typeof(GetCustomMmvSearchVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetCustomMmvSearchVm>>> ResetMvvMappingForIcVariant(ResetMvvMappingForIcVariantCommand getCustomMmvSearchQuery, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(getCustomMmvSearchQuery, cancellationToken);
            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError(result.Messages);
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion
    }
}
