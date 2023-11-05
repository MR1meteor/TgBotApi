namespace TgBotApi.Models
{
    public class ExecuteResponse
    {
        public string? Response { get; set; } = string.Empty;
        public string? Error { get; set; } = string.Empty;

        public ExecuteResponse(string? response = null, string? error = null)
        {
            Response = response;
            Error = error;
        }
    }
}
