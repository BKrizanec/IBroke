using IBroke.Helpers;
using IBroke.Repository;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly EntryRepository _repository;

    private string _totalIncomeLabel = "Total Income:";
    private string _totalExpensesLabel = "Total Expenses:";
    private string _netBalanceLabel = "Net Balance:";

    // Fields for the monthly averages
    private decimal _averageIncomePerMonth;
    private decimal _averageExpenditurePerMonth;
    private decimal _averageNetPerMonth;

    public MainViewModel(EntryRepository repository)
    {
        _repository = repository;
        _repository.EntriesChanged += OnEntriesChanged;
    }

    private void OnEntriesChanged()
    {
        CalculateMonthlyAverages();
    }

    public string TotalIncomeLabel
    {
        get => _totalIncomeLabel;
        set
        {
            _totalIncomeLabel = value;
            OnPropertyChanged(nameof(TotalIncomeLabel));
        }
    }

    public string TotalExpensesLabel
    {
        get => _totalExpensesLabel;
        set
        {
            _totalExpensesLabel = value;
            OnPropertyChanged(nameof(TotalExpensesLabel));
        }
    }

    public string NetBalanceLabel
    {
        get => _netBalanceLabel;
        set
        {
            _netBalanceLabel = value;
            OnPropertyChanged(nameof(NetBalanceLabel));
        }
    }

    // Properties for average values
    public decimal AverageIncomePerMonth
    {
        get => _averageIncomePerMonth;
        set
        {
            _averageIncomePerMonth = value;
            OnPropertyChanged(nameof(AverageIncomePerMonth));
        }
    }

    public decimal AverageExpenditurePerMonth
    {
        get => _averageExpenditurePerMonth;
        set
        {
            _averageExpenditurePerMonth = value;
            OnPropertyChanged(nameof(AverageExpenditurePerMonth));
        }
    }

    public decimal AverageNetPerMonth
    {
        get => _averageNetPerMonth;
        set
        {
            _averageNetPerMonth = value;
            OnPropertyChanged(nameof(AverageNetPerMonth));
        }
    }

    // Event for PropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    // Method to update labels based on the filter
    public void UpdateLabels(bool isLast30Days)
    {
        if (isLast30Days)
        {
            TotalIncomeLabel = "Total Income (Last 30 Days):";
            TotalExpensesLabel = "Total Expenses (Last 30 Days):";
            NetBalanceLabel = "Net Balance (Last 30 Days):";
        }
        else
        {
            TotalIncomeLabel = "Total Income:";
            TotalExpensesLabel = "Total Expenses:";
            NetBalanceLabel = "Net Balance:";
        }
    }

    // Method to calculate average income, expenditure, and net per month
    public void CalculateMonthlyAverages()
    {
        var months = _repository.GetAllEntries()
        .Where(entry => entry.Date <= DateTime.Now)
        .GroupBy(entry => new { entry.Date.Year, entry.Date.Month })
        .ToList();

        decimal totalMonthlyIncome = 0;
        decimal totalMonthlyExpenditure = 0;
        decimal totalMonthlyNet = 0;

        foreach (var month in months)
        {
            decimal monthlyIncome = month
                .Where(entry => entry.Type == EntryType.Income)
                .Sum(entry => entry.Amount);

            decimal monthlyExpenditure = month
                .Where(entry => entry.Type == EntryType.Expense)
                .Sum(entry => entry.Amount);

            totalMonthlyIncome += monthlyIncome;
            totalMonthlyExpenditure += monthlyExpenditure;
            totalMonthlyNet += (monthlyIncome - monthlyExpenditure);
        }

        int totalMonths = months.Count;

        AverageIncomePerMonth = totalMonths > 0 ? totalMonthlyIncome / totalMonths : 0;
        AverageExpenditurePerMonth = totalMonths > 0 ? totalMonthlyExpenditure / totalMonths : 0;
        AverageNetPerMonth = totalMonths > 0 ? totalMonthlyNet / totalMonths : 0;
    }

    // Notify changes for properties
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
