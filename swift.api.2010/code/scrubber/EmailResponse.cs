namespace swift.api._2010.code.scrubber
{
    public class EmailResponse
    {
        public string Address { get; set; }

        public string Status { get; set; }

        public string StatusCode { get; set; }

        public EmailResponse(string address, string status, string statusCode)
        {
            Address = address;
            Status = status;
            StatusCode = statusCode;
        }
    }
}