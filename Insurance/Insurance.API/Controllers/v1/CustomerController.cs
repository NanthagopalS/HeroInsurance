using Insurance.Core.Features.Customer.Queries.GetCustomersList;
using Insurance.Core.Features.Customer.Queries.GetParticularCustomerDetailById;
using Insurance.Core.Features.Customer.Queries.GetRenewalDetailsById;
using Insurance.Core.Helpers;
using Insurance.Domain.Customer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;

namespace Insurance.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        public CustomerController(IMediator mediatr, IConfiguration configuration, ISmsService smsServ, IEmailService emailServ)
        {
            _mediatr = mediatr;
            _config = configuration;
            _emailService = emailServ;
            _smsService = smsServ;
        }

        #region - GetCustomersDetail -
        /// <summary>
        /// Get Customers details
        /// </summary>
        /// <param name="reqCustomerListQuery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCustomersDetail")]
        [ProducesResponseType(typeof(GetCustomersListVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetCustomersResponseModel>> GetCustomersDetail(GetCustomersListRequest reqCustomerListQuery, CancellationToken cancellationToken)
        {
            GetCustomersListQuery reqq = new GetCustomersListQuery
            {
                CurrentPageIndex = reqCustomerListQuery.CurrentPageIndex,
                CurrentPageSize = reqCustomerListQuery.CurrentPageSize,
                CustomerType = reqCustomerListQuery.CustomerType,
                PolicyType = reqCustomerListQuery.PolicyType,
                EndDate = reqCustomerListQuery.EndDate,
                SearchText = reqCustomerListQuery.SearchText,
                StartDate = reqCustomerListQuery.StartDate,
                PolicyStatus = reqCustomerListQuery.PolicyStatus,
                PolicyNature = reqCustomerListQuery.PolicyNature,
            };

            int index = 1;
            int reqStarter = reqCustomerListQuery.CurrentPageIndex == 1 ? 0 : Convert.ToInt32(reqCustomerListQuery.CurrentPageIndex - 1) * Convert.ToInt32(reqCustomerListQuery.CurrentPageSize);
            var result = await _mediatr.Send(reqq, cancellationToken);
            if (result.Result != null)
            {
                foreach (var item in result.Result.GetCustomersListModel)
                {
                    item.SerialNumber = index + reqStarter;
                    index++;
                }
            }

            if (result.Failed)
            {
                var errorDetails = Result.CreateNotFoundError(result.Messages);
                return NotFound(errorDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - SendRenewalNotification -
        /// <summary>
        /// InsertNotification
        /// </summary>
        /// <param name="SendRenewalNotification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendRenewalNotification")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<bool>> SendRenewalNotification(GetRenewalDetailsByIdQuery renewalQuery, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(renewalQuery, cancellationToken);
            // Metho call 
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("No Record Found for the Lead.");
                return NotFound(problemDetails);
            }

            var res = Result.CreateSuccess(true, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #region - Get Particular Lead DetailById -
        /// <summary>
        /// Get Particular Lead Detail
        /// </summary>
        /// <param name="POSPId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetParticularLeadDetailById")]
        [ProducesResponseType(typeof(GetParticularLeadDetailByIdVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetParticularLeadDetailByIdVm>> GetParticularLeadDetailById(string? LeadId, CancellationToken cancellationToken)
        {
            var req = new GetParticularLeadDetailByIdQuery()
            {
                LeadId = LeadId
            };
            var result = await _mediatr.Send(req, cancellationToken);
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
