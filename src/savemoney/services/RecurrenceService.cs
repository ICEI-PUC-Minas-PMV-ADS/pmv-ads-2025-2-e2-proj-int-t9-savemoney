using savemoney.Models;

namespace savemoney.Services
{
    public class RecurrenceService
    {
        public IEnumerable<T> GenerateOccurrences<T>(T template, DateTime fromInclusive, DateTime toInclusive)
            where T : class, IRecurring, new()
        {
            if (!template.IsRecurring || template.Frequency == RecurrenceFrequency.None)
                yield break;

            var current = template.Data;
            var generated = 0;
            while (true)
            {
                if (current > toInclusive) yield break;
                if (current >= fromInclusive && current <= toInclusive)
                {
                    var copy = new T
                    {
                        Data = current,
                        IsRecurring = false
                    };
                    CopyProperties(template, copy);
                    yield return copy;
                    generated++;
                    if (template.RecurrenceOccurrences.HasValue && generated >= template.RecurrenceOccurrences.Value) yield break;
                }

                if (template.RecurrenceEndDate.HasValue && current >= template.RecurrenceEndDate.Value) yield break;

                current = template.Frequency switch
                {
                    RecurrenceFrequency.Daily => current.AddDays(template.Interval),
                    RecurrenceFrequency.Weekly => current.AddDays(7 * template.Interval),
                    RecurrenceFrequency.Monthly => current.AddMonths(template.Interval),
                    RecurrenceFrequency.Yearly => current.AddYears(template.Interval),
                    _ => current.AddDays(1)
                };
            }
        }

        private void CopyProperties<T>(T source, T target)
        {
            var props = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            foreach (var prop in props)
            {
                prop.SetValue(target, prop.GetValue(source));
            }
        }
    }
}
