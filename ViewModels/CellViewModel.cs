using System.ComponentModel;

namespace CSVReader.ViewModels
{
    /// <summary>
    /// View model for the cell popup window
    /// </summary>
    public class CellViewModel : INotifyPropertyChanged
    {
        private string _value;
        public string Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(nameof(Value)); }
        }

        private string _title = "Cell";
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        /// <summary>
        /// Creates a new view model with cell value and title
        /// </summary>
        public CellViewModel(string cellValue, string title = null)
        {
            Value = cellValue ?? string.Empty;
            if (!string.IsNullOrEmpty(title))
                Title = title;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Notifies UI when property changes
        /// </summary>
        private void OnPropertyChanged(string prop) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
