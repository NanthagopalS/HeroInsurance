namespace Insurance.Domain.TATA
{
    public class TATAPOADocumentUploadRequestDto
    {
        public string req_id { get; set; }
        public string proposal_no { get; set; }
        public string doc_type { get; set; }
        public string id_type { get; set; }
        public string doc_base64 { get; set; }
    }
}
