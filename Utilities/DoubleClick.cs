using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CSVReader.Utilities
{
    /// <summary>
    /// Attached behavior that handles double-click on DataGrid cells
    /// </summary>
    public static class DoubleClick
    {
        public static readonly DependencyProperty CellDoubleClickCommandProperty =
            DependencyProperty.RegisterAttached(
                "CellDoubleClickCommand",
                typeof(ICommand),
                typeof(DoubleClick),
                new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Sets the command to execute when cell is double-clicked
        /// </summary>
        public static void SetCellDoubleClickCommand(DependencyObject obj, ICommand value)
            => obj.SetValue(CellDoubleClickCommandProperty, value);

        /// <summary>
        /// Gets the command attached to the DataGrid
        /// </summary>
        public static ICommand GetCellDoubleClickCommand(DependencyObject obj)
            => (ICommand)obj.GetValue(CellDoubleClickCommandProperty);

        /// <summary>
        /// Subscribes or unsubscribes to double-click events when command changes
        /// </summary>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid grid)
                return;

            if (e.OldValue == null && e.NewValue != null)
                grid.MouseDoubleClick += DataGrid_MouseDoubleClick;
            else if (e.OldValue != null && e.NewValue == null)
                grid.MouseDoubleClick -= DataGrid_MouseDoubleClick;
        }

        /// <summary>
        /// Handles double-click event, extracts cell info, and executes command
        /// </summary>
        private static void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not DataGrid dg)
                return;

            var dep = (DependencyObject)e.OriginalSource;
            while (dep != null && dep is not DataGridCell && dep is not DataGridColumnHeader)
                dep = VisualTreeHelper.GetParent(dep);

            if (dep is DataGridColumnHeader)
                return;

            if (dep is not DataGridCell cell)
                return;

            var cellText = GetCellValue(cell) ?? string.Empty;
            var rowIndex = GetRowIndex(cell, dg);
            var colIndex = cell.Column?.DisplayIndex ?? -1;

            var cellInfo = new CellInfo(rowIndex, colIndex, cellText);
            var command = GetCellDoubleClickCommand(dg);
            if (command?.CanExecute(cellInfo) == true)
                command.Execute(cellInfo);
        }

        /// <summary>
        /// Extracts the text value from a DataGrid cell
        /// </summary>
        private static string? GetCellValue(DataGridCell cell)
        {
            if (cell.DataContext is not DataRowView drv)
                return TryGetTextFromCellVisual(cell);

            if (cell.Column is DataGridBoundColumn boundColumn &&
                boundColumn.Binding is System.Windows.Data.Binding binding &&
                !string.IsNullOrEmpty(binding.Path?.Path))
            {
                try
                {
                    return drv.Row[binding.Path.Path]?.ToString();
                }
                catch { }
            }

            try
            {
                var idx = cell.Column.DisplayIndex;
                if (idx >= 0 && idx < drv.Row.Table.Columns.Count)
                    return drv.Row[idx]?.ToString();
            }
            catch { }

            return TryGetTextFromCellVisual(cell);
        }

        /// <summary>
        /// Gets the row index of the clicked cell
        /// </summary>
        private static int GetRowIndex(DataGridCell cell, DataGrid dg)
        {
            if (cell.DataContext is DataRowView drv)
            {
                try { return drv.Row.Table.Rows.IndexOf(drv.Row); }
                catch { }
            }

            try { return dg.Items.IndexOf(cell.DataContext); }
            catch { return -1; }
        }

        /// <summary>
        /// Searches visual tree to find TextBlock content in cell
        /// </summary>
        private static string? TryGetTextFromCellVisual(DataGridCell cell)
        {
            var txt = FindVisualChild<TextBlock>(cell);
            if (txt != null)
                return txt.Text;

            if (cell.Content is ContentPresenter cp)
            {
                var inner = FindVisualChild<TextBlock>(cp);
                if (inner != null)
                    return inner.Text;
            }

            return cell.Content?.ToString();
        }

        /// <summary>
        /// Recursively searches visual tree for a specific control type
        /// </summary>
        private static T? FindVisualChild<T>(DependencyObject? dep) where T : DependencyObject
        {
            if (dep == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
            {
                var child = VisualTreeHelper.GetChild(dep, i);
                if (child is T t)
                    return t;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
