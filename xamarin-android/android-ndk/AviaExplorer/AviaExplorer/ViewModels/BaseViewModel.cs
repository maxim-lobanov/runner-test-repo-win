using System.ComponentModel;

namespace AviaExplorer.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Auto-implemented
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}