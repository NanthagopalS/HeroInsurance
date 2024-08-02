using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using System.Net;
using System.Threading;
using ThirdPartyUtilities.Helpers;
using System.Xml.Linq;
using MailKit.Search;
using System.Data;
using System.Collections.Generic;
using ExcelDataReader;
using System.Net.NetworkInformation;
using Admin.Core.Responses;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using MongoDB.Driver.Core.Misc;
using DocumentFormat.OpenXml.Presentation;
using System.Formats.Asn1;
using System.Globalization;
using SharpCompress.Common;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ThirdPartyUtilities.Abstraction;
using System.IO;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using AutoMapper.Configuration.Annotations;
using Admin.Core.Features.Banners.Queries.GetBannerDetail;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Admin.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
	[ServiceFilter(typeof(ResponseCaptureFilter))]
	public class BannersController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly IMongoDBService _mongodbService;
        //IExcelDataReader reader;
        //ServicingContext context;
        public BannersController(IMediator mediatr, IMongoDBService mongodbService)
        {
            _mediatr = mediatr;
            _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        }

        #region - GET Methods -

        #region - Get Banner Details -
        /// <summary>
        /// GetBannerDetail
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBannerDetail")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(GetBannerDetailVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetBannerDetailVm>>> GetBannerDetail(CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new GetBannerDetailQuery(), cancellationToken);
            if (result.Failed)
            {
                var problemDetails = Result.CreateNotFoundError("Data not found");
                return NotFound(problemDetails);
            }
            var res = Result.CreateSuccess(result.Result, (int)HttpStatusCode.OK);
            return Ok(res);
        }
        #endregion

        #endregion


    }
}

