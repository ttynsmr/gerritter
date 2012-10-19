
namespace Gerritter.GerritStreamEvents.Events
{
    interface IGerritEvent
    {
        EventType EventType { get; }
        string ProjectName { get; }

        NotificationMessage CreateNotificationMessage();
    }
}
