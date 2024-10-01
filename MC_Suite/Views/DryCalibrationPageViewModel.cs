using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using MC_Suite.Helpers;
using MC_Suite.Services;
using MC_Suite.Properties;

namespace MC_Suite.Views
{
    public class DryCalibrationPageViewModel : INotifyPropertyChanged
    {
        private static DryCalibrationPageViewModel _instance;
        public static DryCalibrationPageViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DryCalibrationPageViewModel();
                return _instance;
            }
        }

        private string _comSelectionMsg;
        public string ComSelectionMsg
        {
            get { return _comSelectionMsg; }
            set
            {
                if (_comSelectionMsg != value)
                {
                    _comSelectionMsg = value;
                    OnPropertyChanged("ComSelectionMsg");
                }
            }
        }

        #region ObservableObject

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
