using CSVReader.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSVReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Minimal code-behind: keep things simple and view-model driven.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes the main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Configures DataGrid columns - hides empty columns and sets headers
        /// </summary>
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg?.ItemsSource is not System.Data.DataView dv)
                return;

            var colIndex = dv.Table.Columns.IndexOf(e.PropertyName);
            if (colIndex < 0)
                return;

            bool allEmpty = true;
            foreach (System.Data.DataRow row in dv.Table.Rows)
            {
                if (!string.IsNullOrWhiteSpace(row[colIndex]?.ToString()))
                {
                    allEmpty = false;
                    break;
                }
            }

            if (allEmpty)
            {
                e.Cancel = true;
                return;
            }

            e.Column.Header = dv.Table.Columns[colIndex].ColumnName;
            e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }
    }
}