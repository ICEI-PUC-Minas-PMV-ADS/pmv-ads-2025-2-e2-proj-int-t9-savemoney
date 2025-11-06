using System;

namespace savemoney.Models
{
    public interface IRecurring
    {
        DateTime Data { get; set; }
        bool IsRecurring { get; set; }
        RecurrenceFrequency Frequency { get; set; }
        int Interval { get; set; }
        DateTime? RecurrenceEndDate { get; set; }
        int? RecurrenceOccurrences { get; set; }
    }
}
