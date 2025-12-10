using CSVReader.ViewModels;
using System.Windows;

namespace CSVReader.Views
{
    /// <summary>
    /// Interaction logic for Cell.xaml
    /// Minimal dialog: set DataContext and Title, nothing fancy.
    /// </summary>
    public partial class Cell : Window
    {
        public Cell(string title, string cellValue)
        {
            InitializeComponent();

            var vm = new CellViewModel(cellValue, title);
            DataContext = vm;
            Title = vm.Title;

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
