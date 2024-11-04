using IBroke.Helpers;

namespace IBroke.Models;

public class Entry
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public EntryType Type { get; set; }
    public DateTime Date { get; set; }

    // Filtering
    public int Month => Date.Month;
    public int Day => Date.Day;
}
