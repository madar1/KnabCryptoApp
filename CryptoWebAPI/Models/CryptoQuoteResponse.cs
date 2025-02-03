namespace CryptoWebAPI.Models
{
    public class CryptoQuoteResponse
    {
        public string RequestedBy { get; set; }
        public Dictionary<string, decimal> Quote { get; set; }

        public CryptoQuoteResponse(string requestedBy, Dictionary<string, decimal> quote)
        {
            RequestedBy = requestedBy;
            Quote = quote;
        }
    }
}
