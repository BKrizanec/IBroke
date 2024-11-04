using IBroke.Helpers;
using IBroke.Models;
using IBroke.Repository;
using System;
using System.Windows;
using System.Windows.Controls;

namespace IBroke;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly EntryRepository _repository;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(EntryRepository repository)
    {
        InitializeComponent();
        _repository = repository ?? new EntryRepository(new EntryContext());
    }

    private void AddEntryButton_Click(object sender, RoutedEventArgs e)
    {
        // Step 1: Validate and parse the amount
        decimal amount;
        if (!decimal.TryParse(amountTextBox.Text, out amount))
        {
            MessageBox.Show("Please enter a valid amount.");
            return; // Exit the method if parsing fails
        }

        // Step 2: Check for ComboBox selection
        if (typeComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a type.");
            return; // Exit the method if no selection is made
        }

        // Step 3: Parse the selected type
        var selectedType = ((ComboBoxItem)typeComboBox.SelectedItem).Content.ToString();
        EntryType entryType;
        try
        {
            entryType = (EntryType)Enum.Parse(typeof(EntryType), selectedType);
        }
        catch (ArgumentException)
        {
            MessageBox.Show($"Invalid entry type selected: {selectedType}");
            return; // Exit the method if parsing fails
        }

        // Step 4: Create the new entry
        Entry newEntry = new Entry
        {
            Amount = amount,
            Type = entryType,
            Date = DateTime.Now
        };

        // Step 5: Add the entry to the repository
        _repository.AddEntry(newEntry);
        RefreshEntryList(); // Refresh the displayed list after adding
    }

    private void RefreshEntryList()
    {
        var entries = _repository.GetAllEntries();
        entriesListView.ItemsSource = entries;

        // Update totals
        totalIncomeText.Text = _repository.GetTotalIncome().ToString("C");
        totalExpensesText.Text = _repository.GetTotalExpenses().ToString("C");
        netBalanceText.Text = _repository.GetNetBalance().ToString("C");
    }

    private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (filterComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string filter = selectedItem.Content.ToString();
            if (filter == "Last 30 Days")
            {
                entriesListView.ItemsSource = _repository.GetEntriesFromLast30Days();
            }
            else if (filter == "All Entries")
            {
                entriesListView.ItemsSource = _repository.GetAllEntries();
            }

            // Update totals for the filtered list
            UpdateTotalsForCurrentView();
        }
    }

    private void UpdateTotalsForCurrentView()
    {
        var entries = entriesListView.ItemsSource as List<Entry>;
        totalIncomeText.Text = entries.Where(entry => entry.Type == EntryType.Income).Sum(entry => entry.Amount).ToString("C");
        totalExpensesText.Text = entries.Where(entry => entry.Type == EntryType.Expense).Sum(entry => entry.Amount).ToString("C");
        netBalanceText.Text = (entries.Where(entry => entry.Type == EntryType.Income).Sum(entry => entry.Amount) -
                              entries.Where(entry => entry.Type == EntryType.Expense).Sum(entry => entry.Amount)).ToString("C");
    }

    private void UpdateEntryButton_Click(object sender, RoutedEventArgs e)
    {
        if (entriesListView.SelectedItem is Entry selectedEntry)
        {
            selectedEntry.Amount = decimal.Parse(amountTextBox.Text);
            selectedEntry.Type = (EntryType)Enum.Parse(typeof(EntryType), typeComboBox.SelectedItem.ToString());
            selectedEntry.Date = DateTime.Now;

            _repository.Update(selectedEntry);
            RefreshEntryList();
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
}