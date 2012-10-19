
namespace Gerritter.GerritStreamEvents
{
    public class NotificationMessage
    {
        public NotificationMessage(string title, string message, string url)
        {
            Title = title;
            Message = message;
            Url = url;
        }

        public string Title { get; private set; }
        public string Message { get; private set; }
        public string Url { get; private set; }
    }
}
