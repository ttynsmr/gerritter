
namespace Gerritter
{
    class GerritStream
    {
        public static dynamic GetValue(dynamic json, string key)
        {
            if (json == null) return null;
            return json.IsDefined(key) ? json[key] : null;
        }

        private const string verifyApprovalType = "VRIF";
        private const string codeReviewApprovalType = "CRVW";

        private const string changeStatusNew = "NEW";
        private const string changeStatusDraft = "DRAFT";
        private const string changeStatusSubmitted = "SUBMITTED";
        private const string changeStatusMerged = "MERGED";
        private const string changeStatusAbandoned = "ABANDONED";
    }
}
