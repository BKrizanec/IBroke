using IBroke.Helpers;
using IBroke.Models;

namespace IBroke.Repository;

public class EntryRepository
{
    private readonly EntryContext _context;
    public event Action EntriesChanged;

    public EntryRepository(EntryContext context)
    {
        _context = context;
    }

    public void SeedTestData()
    {
        var random = new Random();

        // Clear existing entries (optional, only for testing)
        _context.Entries.RemoveRange(_context.Entries);

        // Populate test entries over the past few months
        for (int monthOffset = 0; monthOffset < 6; monthOffset++)  // Seed for 6 months back
        {
            var date = DateTime.Now.AddMonths(-monthOffset);

            // Add a few income entries for each month
            for (int i = 0; i < 3; i++)
            {
                var incomeEntry = new Entry
                {
                    Amount = random.Next(500, 2000),  // Random income between 500 and 2000
                    Type = EntryType.Income,
                    Date = new DateTime(date.Year, date.Month, random.Next(1, 28))
                };
                _context.Entries.Add(incomeEntry);
            }

            // Add a few expense entries for each month
            for (int i = 0; i < 3; i++)
            {
                var expenseEntry = new Entry
                {
                    Amount = random.Next(100, 1500),  // Random expense between 100 and 1500
                    Type = EntryType.Expense,
                    Date = new DateTime(date.Year, date.Month, random.Next(1, 28))
                };
                _context.Entries.Add(expenseEntry);
            }
        }

        // Save changes to the database
        _context.SaveChanges();
    }

    public void AddEntry(Entry entry)
    {
        _context.Entries.Add(entry);
        _context.SaveChanges();
        EntriesChanged?.Invoke();
    }

    public List<Entry> GetAllEntries()
    {
        return _context.Entries.ToList();
    }

    public void Update(Entry entry)
    {
        _context.Entries.Update(entry);
        _context.SaveChanges();
        EntriesChanged?.Invoke();
    }

    public void Delete(int id)
    {
        Entry entry = _context.Entries.Find(id);
        if (entry != null)
        {
            _context.Entries.Remove(entry);
            _context.SaveChanges();
            EntriesChanged?.Invoke();
        }
    }

    public decimal GetTotalIncome()
    {
        return _context.Entries
                       .Where(entry => entry.Type == EntryType.Income)
                       .Sum(entry => entry.Amount);
    }

    public decimal GetTotalExpenses()
    {
        return _context.Entries
                       .Where(entry => entry.Type == EntryType.Expense)
                       .Sum(entry => entry.Amount);
    }

    public decimal GetNetBalance()
    {
        return GetTotalIncome() - GetTotalExpenses();
    }

    public decimal GetTotalSpendingLast30Days()
    {
        DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
        return _context.Entries
                       .Where(entry => entry.Type == EntryType.Expense && entry.Date >= thirtyDaysAgo)
                       .Sum(entry => entry.Amount);
    }

    public decimal GetTotalIncomeLast30Days()
    {
        DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
        return _context.Entries
                       .Where(entry => entry.Type == EntryType.Income && entry.Date >= thirtyDaysAgo)
                       .Sum(entry => entry.Amount);
    }

    public decimal GetNetBalanceLast30Days()
    {
        // Calculate the net balance for the last 30 days by subtracting the total expenses from total income
        return GetTotalIncomeLast30Days() - GetTotalSpendingLast30Days();
    }

    public decimal GetAverageDailySpendingLast30Days()
    {
        var last30DaysEntries = _context.Entries
                                         .Where(entry => entry.Date >= DateTime.Now.AddDays(-30))
                                         .ToList();
        return last30DaysEntries.Count > 0
            ? last30DaysEntries.Sum(entry => entry.Amount) / 30
            : 0; // Prevent division by zero
    }

    public decimal GetAverageMonthlySpending()
    {
        var monthlySpending = _context.Entries
                                       .GroupBy(entry => new { entry.Date.Year, entry.Date.Month })
                                       .Select(g => g.Sum(entry => entry.Amount))
                                       .ToList();
        return monthlySpending.Count > 0
            ? monthlySpending.Average()
            : 0; // Prevent division by zero
    }

    public decimal GetOverallAverageSpending()
    {
        var allSpending = _context.Entries
                                   .Select(entry => entry.Amount)
                                   .ToList();
        return allSpending.Count > 0
            ? allSpending.Average()
            : 0; // Prevent division by zero
    }
}
