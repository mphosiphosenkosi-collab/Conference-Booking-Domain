namespace API.Models
{
    public class ErrorResponse
    {
        public string Message { get; set; } = null!;
        public string? Details { get; set; }
        public string Category { get; set; } = "UnexpectedError"; // New add for etra credit 2.3
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
