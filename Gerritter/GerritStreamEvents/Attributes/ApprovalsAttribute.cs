using System.Collections;
using System.Collections.Generic;

namespace Gerritter.GerritStreamEvents.Attributes
{
    public class ApprovalsAttribute : IEnumerable<ApprovalAttribute>
    {
        public ApprovalsAttribute(dynamic json)
        {
            approvals = new List<ApprovalAttribute>();
            if (json == null) return;
            foreach (var a in json)
            {
                approvals.Add(new ApprovalAttribute(a));
            }
        }

        List<ApprovalAttribute> approvals;

        public void Add(ApprovalAttribute approval)
        {
            approvals.Add(approval);
        }

        public ApprovalAttribute this[int index]
        {
            get
            {
                return approvals[index];
            }
        }
        public int Count
        {
            get
            {
                return approvals.Count;
            }
        }

        public IEnumerator<ApprovalAttribute> GetEnumerator()
        {
            return new ApprovalAttributeEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ApprovalAttributeEnumerator(this);
        }
    }

    public class ApprovalAttributeEnumerator : IEnumerator<ApprovalAttribute>
    {
        int index;
        ApprovalsAttribute approvals;

        public ApprovalAttributeEnumerator(ApprovalsAttribute approvals)
        {
            index = -1;
            this.approvals = approvals;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (index < approvals.Count - 1)
            {
                index++;
                return true;
            }
            return false;
        }

        public ApprovalAttribute Current
        {
            get
            {
                if (index == -1)
                {
                    return null;
                }
                return approvals[index];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public void Reset()
        {
            index = -1;
        }
    }
}
