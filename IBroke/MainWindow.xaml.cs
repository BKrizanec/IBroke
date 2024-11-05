using IBroke.Helpers;
using IBroke.Models;
using IBroke.Repository;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace IBroke;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly EntryRepository _repository;
    private readonly MainViewModel _mainViewModel;

    public MainWindow(EntryRepository repository)
    {
        _repository = repository;
        _mainViewModel = new MainViewModel(_repository);
        DataContext = _mainViewModel;
        InitializeComponent();
        UpdateTotalsForLast30Days();
        _mainViewModel.CalculateMonthlyAverages();
    }

    private void AddEntryButton_Click(object sender, RoutedEventArgs e)
    {
        // Get the input from the TextBox
        string input = amountTextBox.Text.Trim();

        // Check if the input is empty
        if (string.IsNullOrEmpty(input))
        {
            MessageBox.Show("Please enter an amount.");
            return; // Exit the method if the input is empty
        }

        // Check if the input matches the expected format (only digits and one period with exactly two decimal places)
        bool isValidFormat = Regex.IsMatch(input, @"^\d+(\.\d{2})?$");

        if (!isValidFormat)
        {
            MessageBox.Show("Please enter a valid amount (use '.' as the decimal separator and exactly two decimal places).");
            return; // Exit the method if the format is invalid
        }

        // Now try to parse the input to a decimal
        if (!decimal.TryParse(input, out decimal amount))
        {
            MessageBox.Show("Please enter a valid amount.");
            return; // Exit the method if parsing fails
        }

        if (typeComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a type.");
            return; // Exit the method if no selection is made
        }

        var selectedType = ((ComboBoxItem)typeComboBox.SelectedItem).Content.ToString();
        if (!Enum.TryParse(selectedType, out EntryType entryType))
        {
            MessageBox.Show($"Invalid entry type selected: {selectedType}");
            return; // Exit the method if parsing fails
        }

        Entry newEntry = new Entry
        {
            Amount = amount,
            Type = entryType,
            Date = DateTime.Now
        };

        _repository.AddEntry(newEntry);
        RefreshEntryList(); // Refresh the displayed list after adding
    }

    private void UpdateEntryButton_Click(object sender, RoutedEventArgs e)
    {
        if (entriesListView.SelectedItem is Entry selectedEntry)
        {
            // Ensure that only Amount and Type are updated
            selectedEntry.Amount = decimal.Parse(amountTextBox.Text); // Update the amount

            // Update the Type, ensuring we're correctly accessing the selected value in ComboBox
            if (typeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                selectedEntry.Type = (EntryType)Enum.Parse(typeof(EntryType), selectedItem.Content.ToString()); // Update the type
            }

            // Now, update the entry in the repository
            _repository.Update(selectedEntry);

            // Refresh the entry list to reflect changes
            RefreshEntryList();
        }
    }

    private void EntriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (entriesListView.SelectedItem is Entry selectedEntry)
        {
            // Populate the Amount field in the TextBox with 2 decimal places
            amountTextBox.Text = selectedEntry.Amount.ToString("F2"); // Format as 2 decimal places

            // Set the selected type in the ComboBox
            typeComboBox.SelectedItem = typeComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == selectedEntry.Type.ToString());
        }
    }

    private void DeleteEntryButton_Click(object sender, RoutedEventArgs e)
    {
        if (entriesListView.SelectedItem is Entry selectedEntry)
        {
            _repository.Delete(selectedEntry.Id);
            RefreshEntryList();
        }
    }

    private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (filterComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            bool isLast30Days = selectedItem.Content.ToString() == "Last 30 Days";

            // Call the method to update labels based on the selected filter
            _mainViewModel.UpdateLabels(isLast30Days);

            if (isLast30Days)
            {
                RefreshEntryList();
                UpdateTotalsForLast30Days();
            }
            else
            {
                RefreshEntryList(); // Refresh all entries
                RefreshAllTimeTotals(); // Update totals for all time
            }
        }
    }


    private void RefreshEntryList()
    {
        var entries = _repository.GetAllEntries();
        entriesListView.ItemsSource = entries;
        RefreshAllTimeTotals(); // Update totals every time we refresh the entry list
    }

    private void RefreshAllTimeTotals()
    {
        totalIncomeText.Text = _repository.GetTotalIncome().ToString("C");
        totalExpensesText.Text = _repository.GetTotalExpenses().ToString("C");
        netBalanceText.Text = _repository.GetNetBalance().ToString("C");
    }

    private void UpdateTotalsForLast30Days()
    {
        totalIncomeText.Text = _repository.GetTotalIncomeLast30Days().ToString("C");
        totalExpensesText.Text = _repository.GetTotalSpendingLast30Days().ToString("C");
        netBalanceText.Text = _repository.GetNetBalanceLast30Days().ToString("C");
    }
}