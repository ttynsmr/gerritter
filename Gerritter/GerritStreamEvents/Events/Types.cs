using System;

namespace Gerritter.GerritStreamEvents.Events
{
    public enum EventType
    {
        PatchsetCreated,
        DraftPublished,
        ChangeAbandoned,
        ChangeRestored,
        ChangeMerged,
        CommentAdded,
        RefUpdated,
        ReviewerAdded,
        MergeFailed
    }

    public static class Event
    {
        private const string patchsetCreatedType = "patchset-created";
        private const string draftPublishedType = "draft-published";
        private const string changeAbandonedType = "change-abandoned";
        private const string changeRestoredType = "change-restored";
        private const string changeMergedType = "change-merged";
        private const string commentAddedType = "comment-added";
        private const string refUpdatedType = "ref-updated";
        private const string reviewerAddedType = "reviewer-added";
        private const string mergeFailedType = "merge-failed";

        public static EventType GetEventTyepe(string type)
        {
            switch (type)
            {
                case patchsetCreatedType:
                    return EventType.PatchsetCreated;

                case draftPublishedType:
                    return EventType.DraftPublished;

                case changeAbandonedType:
                    return EventType.ChangeAbandoned;

                case changeRestoredType:
                    return EventType.ChangeRestored;

                case changeMergedType:
                    return EventType.ChangeMerged;

                case commentAddedType:
                    return EventType.CommentAdded;

                case refUpdatedType:
                    return EventType.RefUpdated;

                case reviewerAddedType:
                    return EventType.ReviewerAdded;

                case mergeFailedType:
                    return EventType.MergeFailed;

                default:
                    throw new ArgumentException(string.Format("\"{0}\" is unknown type."), type);
            }
        }
    }
}
