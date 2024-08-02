using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Core.Features.Quote.Query.GetPolicyDatesCommercial
{
    public class GetPolicyDatesQueryCommercial : IRequest<HeroResult<PolicyDatesResponse>>
    {
        public string RegistrationYear { get; set; }
        public string VehicleType { get; set; }
        public bool IsPreviousPolicy { get; set; }
        public string ODPolicyExpiry { get; set; }
        public string TPPolicyExpiry { get; set; }
        public string PreviousPolicyTypeId { get; set; }
        public bool IsBrandNewVehicle { get; set; }
        public string ManufacturingDate { get; set; }
    }
    public class GetPolicyDatesQueryCommercialHandler : IRequestHandler<GetPolicyDatesQueryCommercial, HeroResult<PolicyDatesResponse>>
    {
        private readonly PolicyTypeConfig PolicyType;
        private readonly VehicleTypeConfig VehicleType;
        public GetPolicyDatesQueryCommercialHandler(IOptions<PolicyTypeConfig> policyType, IOptions<VehicleTypeConfig> vehicleType)
        {
            PolicyType = policyType?.Value;
            VehicleType = vehicleType?.Value;
        }

        public Task<HeroResult<PolicyDatesResponse>> Handle(GetPolicyDatesQueryCommercial request, CancellationToken cancellationToken)
        {
            var dateTime = DateTime.Today;
            int vehicleTenure = 1;
            int vehicleTPTenure = 1;
            string registrationDate = string.Empty;
            string manufacturingDate = string.Empty;
            var firstDay = new DateTime(dateTime.Year, dateTime.Month, 1);
            var response = new PolicyDatesResponse();

            response.IsCommercial = true;
          
            if (request.RegistrationYear?.ToCharArray().Length == 4)
            {
                registrationDate = $"{firstDay:dd}-{dateTime:MMM}-{request.RegistrationYear}";
                response.ManufacturingDate = registrationDate;
            }
            else
            {
                registrationDate = request.RegistrationYear;
                //var date = Convert.ToDateTime(request.ManufacturingDate);
                //var getdate = new DateTime(date.Year, date.Month, 1);
                //manufacturingDate = $"{firstDay:dd}-{getdate:MM}-{getdate:yyyy}";
                //response.ManufacturingDate = Convert.ToDateTime(manufacturingDate).ToString("dd-MM-yyyy");
                response.ManufacturingDate = Convert.ToDateTime(request.ManufacturingDate).ToString("dd-MM-yyyy");
            }

            if (request.IsBrandNewVehicle)
            {
                response.PolicyStartDate = dateTime.ToString("dd-MMM-yyyy");
                response.PolicyEndDate = dateTime.AddYears(vehicleTenure).AddDays(-1).ToString("dd-MMM-yyyy");

                if (request.PreviousPolicyTypeId == PolicyType.PackageComprehensive)
                {
                    response.VehicleTPTenure = 1;
                    response.VehicleODTenure = 1;
                    response.PreviousPolicyTypeId = PolicyType.ComprehensiveBundle;
                }
                else if (request.PreviousPolicyTypeId == PolicyType.SATP)
                {
                    response.VehicleTPTenure = 1;
                    response.VehicleODTenure = 0;
                    response.PreviousPolicyTypeId = PolicyType.SATP;
                }
            }
            else
            {
                if (!request.IsPreviousPolicy)
                {
                    response.TPPolicyStartDate = Convert.ToDateTime(registrationDate).ToString("dd-MMM-yyyy");
                    response.TPPolicyEndDate = Convert.ToDateTime(response.TPPolicyStartDate).AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");

                    response.PolicyStartDate = dateTime.AddDays(1).ToString("dd-MMM-yyyy");
                    response.PolicyEndDate = dateTime.AddYears(1).ToString("dd-MMM-yyyy");

                    response.PreviousPolicyTypeId = PolicyType.PackageComprehensive;
                    response.VehicleTPTenure = 1;
                    response.VehicleODTenure = 0;
                }
                else
                {
                    if (!string.IsNullOrEmpty((request.ODPolicyExpiry)))
                    {
                        response.ODPolicyStartDate = Convert.ToDateTime(request.ODPolicyExpiry).AddYears(-1).AddDays(1).ToString("dd-MMM-yyyy");
                        response.ODPolicyEndDate = Convert.ToDateTime(request.ODPolicyExpiry).ToString("dd-MMM-yyyy");
                    }
                    if (!string.IsNullOrEmpty(request.TPPolicyExpiry))//TP
                    {
                        response.TPPolicyStartDate = Convert.ToDateTime(request.TPPolicyExpiry).AddYears(-1).AddDays(1).ToString("dd-MMM-yyyy");
                        response.TPPolicyEndDate = Convert.ToDateTime(request.TPPolicyExpiry).ToString("dd-MMM-yyyy");
                    }

                    if (!string.IsNullOrEmpty(request.PreviousPolicyTypeId) && request.PreviousPolicyTypeId == PolicyType.PackageComprehensive)
                    {
                        response.PreviousPolicyTypeId = PolicyType.PackageComprehensive;
                        if (dateTime.Compare(Convert.ToDateTime(request.ODPolicyExpiry), DateTimeInterval.Days) > 45)
                        {
                            return Task.FromResult(HeroResult<PolicyDatesResponse>.Fail("OD Policy expiry date is greater than 45 days in future, I won't be allowing to issue the policy."));
                        }
                        else
                        {
                            response.VehicleTPTenure = 1;
                            response.VehicleODTenure = 1;
                            if (!string.IsNullOrWhiteSpace(request.ODPolicyExpiry) && Convert.ToDateTime(request.ODPolicyExpiry) <= dateTime)
                            {
                                response.PolicyStartDate = dateTime.AddDays(1).ToString("dd-MMM-yyyy");
                                response.PolicyEndDate = dateTime.AddYears(1).ToString("dd-MMM-yyyy");
                            }
                            else
                            {
                                response.PolicyStartDate = Convert.ToDateTime(request.ODPolicyExpiry).AddDays(1).ToString("dd-MMM-yyyy");
                                response.PolicyEndDate = Convert.ToDateTime(response.PolicyStartDate).AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(request.PreviousPolicyTypeId) && request.PreviousPolicyTypeId == PolicyType.SATP)
                    {
                        response.PreviousPolicyTypeId = PolicyType.SATP;
                        if (dateTime.Compare(Convert.ToDateTime(request.TPPolicyExpiry), DateTimeInterval.Days) > 45)
                        {
                            return Task.FromResult(HeroResult<PolicyDatesResponse>.Fail("TP Policy expiry date is greater than 45 days in future, I won't be allowing to issue the policy."));
                        }
                        else
                        {
                            response.VehicleTPTenure = 1;
                            response.VehicleODTenure = 0;
                            if (!string.IsNullOrWhiteSpace(request.TPPolicyExpiry) && Convert.ToDateTime(request.TPPolicyExpiry) <= dateTime)
                            {
                                response.PolicyStartDate = dateTime.AddDays(1).ToString("dd-MMM-yyyy");
                                response.PolicyEndDate = dateTime.AddYears(1).ToString("dd-MMM-yyyy");
                            }
                            else
                            {
                                response.PolicyStartDate = Convert.ToDateTime(request.TPPolicyExpiry).AddDays(1).ToString("dd-MMM-yyyy");
                                response.PolicyEndDate = Convert.ToDateTime(response.PolicyStartDate).AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
                            }
                        }
                    }
                }
            }

            response.RegistrationDate = registrationDate;
            return Task.FromResult(HeroResult<PolicyDatesResponse>.Success(response));
        }
    }
}
