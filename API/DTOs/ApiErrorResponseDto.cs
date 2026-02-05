namespace ConferenceRoomBooking.Domain.DTOs;

public class ApiErrorResponse
{
    public string Type { get; set; } = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
    public string Title { get; set; } = "One or more validation errors occurred.";
    public int Status { get; set; }
    public string TraceId { get; set; }
    public Dictionary<string, string[]> Errors { get; set; } = new();

    public ApiErrorResponse(int status, string title = null)
    {
        Status = status;
        if (title != null) Title = title;
    }
}

public class DomainErrorResponse : ApiErrorResponse
{
    public string ErrorCode { get; set; }
    public string Suggestion { get; set; }

    public DomainErrorResponse(string errorCode, string message, string suggestion = null) 
        : base(422, "Domain rule violation")
    {
        ErrorCode = errorCode;
        Title = message;
        Suggestion = suggestion;
    }
}