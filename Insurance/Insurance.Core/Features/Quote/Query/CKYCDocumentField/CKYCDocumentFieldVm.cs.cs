﻿using Insurance.Domain.Quote;

namespace Insurance.Core.Features.Quote.Query.CKYCDocumentField;

public class CKYCDocumentFieldVm
{
    public string SectionName { get; set; }
    public List<ProposalFieldMasterModel> SectionDetails { get; set; }
}
