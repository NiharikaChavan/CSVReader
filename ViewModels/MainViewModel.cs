using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CSVReader.Services;
using CSVReader.Utilities;
using CSVReader.Views;
using Microsoft.Win32;
using Serilog;

namespace CSVReader.ViewModels
{
    /// <summary>
    /// Main view model that handles CSV file operations and UI state
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CsvService csvService = new CsvService();

        private DataView tableView;
        public DataView TableView
        {
            get => tableView;
            set => SetProperty(ref tableView, value);
        }

        private string statusMessage;
        public string StatusMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        public ICommand LoadCsvCommand { get; }
        public ICommand BrowseCsvCommand { get; }
        public ICommand CellDoubleClickCommand { get; }

        private string? _currentCsvPath;

        /// <summary>
        /// Constructor - initializes commands and loads sample CSV file if available
        /// </summary>
        public MainViewModel()
        {
            LoadCsvCommand = new RelayCommand(_ => LoadCsv());
            BrowseCsvCommand = new RelayCommand(_ => BrowseAndLoad());
            CellDoubleClickCommand = new RelayCommand(ExecuteCellDoubleClick);
            
            var samplePath = Path.Combine(AppContext.BaseDirectory, "Files", "sample.csv");
            if (File.Exists(samplePath))
            {
                _currentCsvPath = samplePath;
                LoadCsv(samplePath);
            }
        }

        /// <summary>
        /// Loads CSV file from the given path and displays it in the grid
        /// </summary>
        public void LoadCsv(string? path = null)
        {
            try
            {
                StatusMessage = "Loading CSV...";
                Log.Information(StatusMessage);

                if (string.IsNullOrEmpty(path))
                    path = _currentCsvPath;

                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    StatusMessage = $"CSV not found.";
                    Log.Warning(StatusMessage);
                    return;
                }

                var fileName = Path.GetFileName(path);
                Log.Information("Reading CSV from: {FileName}", fileName);
                
                var dt = csvService.LoadCsvToDataTable(path);
                TableView = dt.DefaultView;

                var success = $"Loaded {dt.Rows.Count} rows, {dt.Columns.Count} columns.";
                StatusMessage = success;
                Log.Information(success);
            }
            catch (Exception ex)
            {
                StatusMessage = "Failed to load CSV.";
                Log.Error(ex, StatusMessage);
            }
        }

        /// <summary>
        /// Opens file dialog for user to browse and select a CSV file
        /// </summary>
        private void BrowseAndLoad()
        {
            try
            {
                Log.Information("Opening file dialog");
                var dlg = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    DefaultExt = "csv",
                    Title = "Select CSV file"
                };

                bool? result = dlg.ShowDialog();
                if (result == true && !string.IsNullOrEmpty(dlg.FileName))
                {
                    _currentCsvPath = dlg.FileName;
                    LoadCsv(_currentCsvPath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error selecting CSV file");
                StatusMessage = "Failed to select CSV. See desktop log.";
            }
        }

        /// <summary>
        /// Handles cell double-click event and opens popup window with cell value
        /// </summary>
        private void ExecuteCellDoubleClick(object parameter)
        {
            try
            {
                string value;
                string title = "Cell";

                if (parameter is CellInfo ci)
                {
                    value = ci.Value;
                    title = ci.Title;
                }
                else
                {
                    value = parameter?.ToString() ?? string.Empty;
                    Log.Information("Cell double-clicked: {Value}", value);
                }

                var win = new Cell(title, value) 
                { 
                    Owner = Application.Current?.MainWindow
                };
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"");
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Updates property value and notifies UI of changes
        /// </summary>
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
