using IBroke.Helpers;
using IBroke.Models;

namespace IBroke.Repository;

public class EntryRepository
{
    private readonly EntryContext _context;

    public EntryRepository(EntryContext context)
    {
        _context = context;
    }

    public void AddEntry(Entry entry)
    {
        _context.Entries.Add(entry);
        _context.SaveChanges();
    }

    public List<Entry> GetAllEntries()
    {
        return _context.Entries.ToList();
    }

    public List<Entry> GetEntriesFromLast30Days()
    {
        DateTime date30DaysAgo = DateTime.Now.AddDays(-30);
        return _context.Entries
                       .Where(entry => entry.Date >= date30DaysAgo)
                       .ToList();
    }

    public void Update(Entry entry)
    {
        _context.Entries.Update(entry);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        Entry entry = _context.Entries.Find(id);
        if (entry != null)
        {
            _context.Entries.Remove(entry);
            _context.SaveChanges();
        }
    }

    // Method to calculate total income
    public decimal GetTotalIncome()
    {
        return _context.Entries
                       .Where(entry => entry.Type == EntryType.Income)
                       .Sum(entry => entry.Amount);
    }

    // Method to calculate total expenses
    public decimal GetTotalExpenses()
    {
        return _context.Entries
                       .Where(entry => entry.Type == EntryType.Expense)
                       .Sum(entry => entry.Amount);
    }

    // Method to calculate net balance (income - expenses)
    public decimal GetNetBalance()
    {
        return GetTotalIncome() - GetTotalExpenses();
    }
}
