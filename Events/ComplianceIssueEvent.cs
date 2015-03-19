using System;

namespace GS.Lib.Events
{
    public class ComplianceIssueEvent : SharkEvent
    {
        public String Issue { get; set; }

        public String Reason { get; set; }
    }
}
