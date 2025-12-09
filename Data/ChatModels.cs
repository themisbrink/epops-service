namespace EpopsService.Data
{
    public class ChatModels
    {
        public record GaiaBChatRequest(string Question, List<string>? History);

        public class GaiaBSyncResponse
        {
            public string job_id { get; set; }
            public string app_status { get; set; }
            public GaiaBResult app_results { get; set; }
        }

        public class GaiaBResult
        {
            public string? response { get; set; }
            public string? error { get; set; }
        }
    }
}
