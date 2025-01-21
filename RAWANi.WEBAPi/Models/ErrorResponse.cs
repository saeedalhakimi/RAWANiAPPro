namespace RAWANi.WEBAPi.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string? StatusPhrase { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string>? ErrorsDetails { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; }
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? Detail { get; set; } // Optional detailed message or suggestion
        public string? CorrelationId { get; set; } // For tracking requests and errors across systems

    }
}
