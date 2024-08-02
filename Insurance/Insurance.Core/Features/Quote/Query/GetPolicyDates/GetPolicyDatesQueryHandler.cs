using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails
{
    public class GetPolicyDatesQuery : IRequest<HeroResult<PolicyDatesResponse>>
    {
        public string RegistrationYear { get; set; }
        public string VehicleType { get; set; }
        public bool IsPreviousPolicy { get; set; }
        public string ODPolicyExpiry { get; set; }
        public string TPPolicyExpiry { get; set; }
        public string PreviousPolicyTypeId { get; set; }
        public bool IsBrandNewVehicle { get; set; }
        public string ManufacturingDate { get; set; }
        public List<PACoverList> PACoverList { get; set; }
    }
    public class GetPolicyDatesQueryHandler : IRequestHandler<GetPolicyDatesQuery, HeroResult<PolicyDatesResponse>>
    {
        private readonly PolicyTypeConfig PolicyType;
        private readonly VehicleTypeConfig VehicleType;
        public GetPolicyDatesQueryHandler(IOptions<PolicyTypeConfig> policyType, IOptions<VehicleTypeConfig> vehicleType)
        {
            PolicyType = policyType?.Value;
            VehicleType = vehicleType?.Value;
        }

        public Task<HeroResult<PolicyDatesResponse>> Handle(GetPolicyDatesQuery request, CancellationToken cancellationToken)
        {
            var response = new PolicyDatesResponse();
            var dateTime = DateTime.Today;
            var firstDay = new DateTime(dateTime.Year, dateTime.Month, 1);
            string registrationDate = string.Empty;
            int years = 0;
            //string manufacturingDate = string.Empty;

            if (request.PACoverList != null)
            {
                bool isOwnerDriver = request.PACoverList.Exists(x => x.PACoverId == "5FFF8E39-0253-42FD-801E-FFB4A91DB8E6");
                response.IsDefaultPACoverRequired = isOwnerDriver;
            }

            if (request.RegistrationYear?.ToCharArray().Length == 4)
            {
                registrationDate = $"{firstDay:dd}-{dateTime:MMM}-{request.RegistrationYear}";
                years = Convert.ToDateTime(registrationDate).Compare(dateTime, DateTimeInterval.Years);
                response.ManufacturingDate = registrationDate;
            }
            else
            {
                registrationDate = Convert.ToDateTime(request.RegistrationYear).ToString("dd-MMM-yyyy");
                years = Convert.ToDateTime(registrationDate).Compare(dateTime, DateTimeInterval.Years);
                //var date = Convert.ToDateTime(request.ManufacturingDate);
                //var getdate = new DateTime(date.Year, date.Month, 1);
                //manufacturingDate = $"{firstDay:dd}-{getdate:MM}-{getdate:yyyy}";
                //response.ManufacturingDate = Convert.ToDateTime(manufacturingDate).ToString("dd-MM-yyyy");
                response.ManufacturingDate = Convert.ToDateTime(request.ManufacturingDate).ToString("dd-MMM-yyyy");
            }
            
            int vehicleTenure = 0;
            int vehicleTPTenure = 0;

            if (request.VehicleType == VehicleType.FourWheeler)
            {
                vehicleTenure = 3;
                vehicleTPTenure = 3;
                response.IsFourWheeler = true;
                response.IsTwoWheeler = false;
            }
            else if (request.VehicleType == VehicleType.TwoWheeler)
            {
                vehicleTenure = 5;
                vehicleTPTenure = 5;
                response.IsTwoWheeler = true;
                response.IsFourWheeler = false;
            }
            if (request.IsBrandNewVehicle)
            {
                response.PolicyStartDate = dateTime.ToString("dd-MMM-yyyy");
                response.PolicyEndDate = dateTime.AddYears(vehicleTenure).AddDays(-1).ToString("dd-MMM-yyyy");
                response.VehicleODTenure = 1;
                response.VehicleTPTenure = vehicleTPTenure;
                response.PreviousPolicyTypeId = PolicyType.ComprehensiveBundle;
            }
            else
            {
                if (years < vehicleTenure)
                {
                    if (!request.IsPreviousPolicy)
                    {
                        response.ODPolicyStartDate = Convert.ToDateTime(registrationDate).ToString("dd-MMM-yyyy");
                        response.ODPolicyEndDate = Convert.ToDateTime(response.ODPolicyStartDate).AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
                        response.TPPolicyStartDate = Convert.ToDateTime(registrationDate).ToString("dd-MMM-yyyy");
                        response.TPPolicyEndDate = Convert.ToDateTime(response.TPPolicyStartDate).AddYears(vehicleTenure).AddDays(-1).ToString("dd-MMM-yyyy");
                        response.PolicyStartDate = dateTime.AddDays(1).ToString("dd-MMM-yyyy");
                        response.PolicyEndDate = dateTime.AddYears(1).ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.ODPolicyExpiry))
                        {
                            response.ODPolicyStartDate = Convert.ToDateTime(request.ODPolicyExpiry).AddYears(-1).AddDays(1).ToString("dd-MMM-yyyy");
                            response.ODPolicyEndDate = Convert.ToDateTime(request.ODPolicyExpiry).ToString("dd-MMM-yyyy");

                            if (!string.IsNullOrEmpty(request.TPPolicyExpiry) && (Convert.ToDateTime(request.ODPolicyExpiry) - Convert.ToDateTime(request.TPPolicyExpiry)).TotalDays != 0)
                            {
                                response.TPPolicyStartDate = Convert.ToDateTime(request.TPPolicyExpiry).AddYears(-vehicleTPTenure).AddDays(1).ToString("dd-MMM-yyyy");
                                response.TPPolicyEndDate = Convert.ToDateTime(request.TPPolicyExpiry).ToString("dd-MMM-yyyy");
                            }
                            else
                            {
                                response.TPPolicyStartDate = Convert.ToDateTime(registrationDate).ToString("dd-MMM-yyyy");
                                response.TPPolicyEndDate = Convert.ToDateTime(response.TPPolicyStartDate).AddYears(vehicleTenure).AddDays(-1).ToString("dd-MMM-yyyy");
                            }
                        }
                        if (Convert.ToDateTime(response.TPPolicyEndDate) < (Convert.ToDateTime(response.ODPolicyEndDate)))
                        {
                            response.TPPolicyEndDate = response.ODPolicyEndDate;
                            response.TPPolicyStartDate = Convert.ToDateTime(response.TPPolicyEndDate).AddYears(-vehicleTenure).ToString("dd-MMM-yyyy");
                        }
                        if (Convert.ToDateTime(request.ODPolicyExpiry) <= dateTime)
                        {
                            response.PolicyStartDate = dateTime.AddDays(1).ToString("dd-MMM-yyyy");
                            response.PolicyEndDate = dateTime.AddYears(1).ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            if (dateTime.Compare(Convert.ToDateTime(request.ODPolicyExpiry), DateTimeInterval.Days) > 45)
                            {
                                return Task.FromResult(HeroResult<PolicyDatesResponse>.Fail("OD Policy expiry date is greater than 45 days in future, I won't be allowing to issue the policy."));
                            }

                            response.PolicyStartDate = Convert.ToDateTime(request.ODPolicyExpiry).AddDays(1).ToString("dd-MMM-yyyy");
                            response.PolicyEndDate = Convert.ToDateTime(response.PolicyStartDate).AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
                        }
                        request.PreviousPolicyTypeId = PolicyType.SAOD;
                    }
                    SetODTPTenure(request.PreviousPolicyTypeId, response);
                }
                else if (years > vehicleTenure)
                {
                    if (!request.IsPreviousPolicy)
                    {
                        response.ODPolicyStartDate = dateTime.AddYears(-1).AddDays(-91).ToString("dd-MMM-yyyy");
                        response.ODPolicyEndDate = dateTime.AddDays(-92).ToString("dd-MMM-yyyy");
                        response.TPPolicyStartDate = Convert.ToDateTime(registrationDate).ToString("dd-MMM-yyyy");
                        response.TPPolicyEndDate = Convert.ToDateTime(response.TPPolicyStartDate).AddYears(vehicleTenure).AddDays(-1).ToString("dd-MMM-yyyy");

                        response.PolicyStartDate = dateTime.AddDays(1).ToString("dd-MMM-yyyy");
                        response.PolicyEndDate = dateTime.AddYears(1).ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.TPPolicyExpiry))//if TP
                        {
                            response.TPPolicyStartDate = Convert.ToDateTime(request.TPPolicyExpiry).AddYears(-1).AddDays(1).ToString("dd-MMM-yyyy");
                            response.TPPolicyEndDate = Convert.ToDateTime(request.TPPolicyExpiry).ToString("dd-MMM-yyyy");
                        }

                        if (!string.IsNullOrEmpty(request.ODPolicyExpiry))//if package comprehensive
                        {
                            response.ODPolicyStartDate = Convert.ToDateTime(request.ODPolicyExpiry).AddYears(-1).AddDays(1).ToString("dd-MMM-yyyy");
                            response.ODPolicyEndDate = Convert.ToDateTime(request.ODPolicyExpiry).ToString("dd-MMM-yyyy");
                        }

                        if (string.IsNullOrEmpty(request.ODPolicyExpiry))
                        {
                            response.ODPolicyStartDate = Convert.ToDateTime(request.TPPolicyExpiry).AddYears(-1).AddDays(1).ToString("dd-MMM-yyyy");
                            response.ODPolicyEndDate = Convert.ToDateTime(request.TPPolicyExpiry).ToString("dd-MMM-yyyy");
                        }

                        if (!string.IsNullOrEmpty(request.PreviousPolicyTypeId) && request.PreviousPolicyTypeId == PolicyType.PackageComprehensive)
                        {
                            if (dateTime.Compare(Convert.ToDateTime(request.ODPolicyExpiry), DateTimeInterval.Days) > 45)
                            {
                                return Task.FromResult(HeroResult<PolicyDatesResponse>.Fail("OD Policy expiry date is greater than 45 days in future, I won't be allowing to issue the policy."));
                            }

                            if (!string.IsNullOrWhiteSpace(request.ODPolicyExpiry) && Convert.ToDateTime(request.ODPolicyExpiry) >= dateTime)
                            {
                                response.PolicyStartDate = Convert.ToDateTime(request.ODPolicyExpiry).AddDays(1).ToString("dd-MMM-yyyy");
                                response.PolicyEndDate = Convert.ToDateTime(response.PolicyStartDate).AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
                            }
                            else
                            {
                                response.PolicyStartDate = dateTime.AddDays(1).ToString("dd-MMM-yyyy");
                                response.PolicyEndDate = dateTime.AddYears(1).ToString("dd-MMM-yyyy");
                                //response.PolicyStartDate = Convert.ToDateTime(request.ODPolicyExpiry).AddDays(1).ToString("dd-MMM-yyyy");
                                //response.PolicyEndDate = Convert.ToDateTime(response.PolicyStartDate).AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
                            }
                        }
                        else
                        {
                            if (dateTime.Compare(Convert.ToDateTime(request.TPPolicyExpiry), DateTimeInterval.Days) > 45)
                            {
                                return Task.FromResult(HeroResult<PolicyDatesResponse>.Fail("OD Policy expiry date is greater than 45 days in future, I won't be allowing to issue the policy."));
                            }

                            if (!string.IsNullOrWhiteSpace(request.TPPolicyExpiry) && Convert.ToDateTime(request.TPPolicyExpiry) >= dateTime)
                            {
                                response.PolicyStartDate = Convert.ToDateTime(request.TPPolicyExpiry).AddDays(1).ToString("dd-MMM-yyyy");
                                response.PolicyEndDate = Convert.ToDateTime(response.PolicyStartDate).AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
                            }
                            else if (!string.IsNullOrWhiteSpace(request.ODPolicyExpiry) && Convert.ToDateTime(request.ODPolicyExpiry) >= dateTime && string.IsNullOrWhiteSpace(request.TPPolicyExpiry))
                            {
                                response.PolicyStartDate = Convert.ToDateTime(request.ODPolicyExpiry).AddDays(1).ToString("dd-MMM-yyyy");
                                response.PolicyEndDate = Convert.ToDateTime(response.PolicyStartDate).AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
                            }
                            else
                            {
                                response.PolicyStartDate = dateTime.AddDays(1).ToString("dd-MMM-yyyy");
                                response.PolicyEndDate = dateTime.AddYears(1).ToString("dd-MMM-yyyy");
                            }
                        }
                    }
                    SetODTPTenure(request.PreviousPolicyTypeId, response);
                }
                else
                {
                    if (!request.IsPreviousPolicy)
                    {
                        response.ODPolicyStartDate = dateTime.AddYears(-1).AddDays(-91).ToString("dd-MMM-yyyy");
                        response.ODPolicyEndDate = dateTime.AddDays(-92).ToString("dd-MMM-yyyy");
                        response.TPPolicyStartDate = Convert.ToDateTime(registrationDate).ToString("dd-MMM-yyyy");
                        response.TPPolicyEndDate = Convert.ToDateTime(response.TPPolicyStartDate).AddYears(3).AddDays(-1).ToString("dd-MMM-yyyy");

                        response.PolicyStartDate = dateTime.AddDays(1).ToString("dd-MMM-yyyy");
                        response.PolicyEndDate = dateTime.AddYears(1).ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.ODPolicyExpiry))
                        {
                            response.ODPolicyStartDate = Convert.ToDateTime(request.ODPolicyExpiry).AddYears(-1).AddDays(1).ToString("dd-MMM-yyyy");
                            response.ODPolicyEndDate = Convert.ToDateTime(request.ODPolicyExpiry).ToString("dd-MMM-yyyy");
                        }
                        if (!string.IsNullOrEmpty(request.TPPolicyExpiry))//TP
                        {
                            response.TPPolicyStartDate = Convert.ToDateTime(request.TPPolicyExpiry).AddYears(-vehicleTPTenure).AddDays(1).ToString("dd-MMM-yyyy");
                            response.TPPolicyEndDate = Convert.ToDateTime(request.TPPolicyExpiry).ToString("dd-MMM-yyyy");
                        }

                        if (!string.IsNullOrEmpty(request.PreviousPolicyTypeId) && request.PreviousPolicyTypeId == PolicyType.PackageComprehensive)
                        {
                            if (dateTime.Compare(Convert.ToDateTime(request.ODPolicyExpiry), DateTimeInterval.Days) > 45)
                            {
                                return Task.FromResult(HeroResult<PolicyDatesResponse>.Fail("OD Policy expiry date is greater than 45 days in future, I won't be allowing to issue the policy."));
                            }
                            else
                            {
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
                            if (dateTime.Compare(Convert.ToDateTime(request.TPPolicyExpiry), DateTimeInterval.Days) > 45)
                            {
                                return Task.FromResult(HeroResult<PolicyDatesResponse>.Fail("TP Policy expiry date is greater than 45 days in future, I won't be allowing to issue the policy."));
                            }
                            else
                            {
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
                    SetODTPTenure(request.PreviousPolicyTypeId, response);
                }
            }

            response.RegistrationDate = registrationDate;
            return Task.FromResult(HeroResult<PolicyDatesResponse>.Success(response));

        }

        private void SetODTPTenure(string policyType, PolicyDatesResponse response)
        {
            if (policyType.Equals(PolicyType.SATP))
            {
                response.PreviousPolicyTypeId = PolicyType.SATP;
                response.VehicleODTenure = 0;
                response.VehicleTPTenure = 1;
            }
            else if (policyType.Equals(PolicyType.SAOD))
            {
                response.PreviousPolicyTypeId = PolicyType.SAOD;
                response.VehicleODTenure = 1;
                response.VehicleTPTenure = 0;
            }
            else
            {
                response.PreviousPolicyTypeId = PolicyType.PackageComprehensive;
                response.VehicleODTenure = 1;
                response.VehicleTPTenure = 1;
            }
        }
    }
}
