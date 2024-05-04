namespace ReaderFast.webui.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string Message { get; set; } // Ensure you have a Message property

    }
}