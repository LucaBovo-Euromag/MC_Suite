using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MC_Suite
{
    public class CommonResources : INotifyPropertyChanged
    {
        private static CommonResources _instance;
        public static CommonResources Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CommonResources();
                return _instance;
            }
        }

        private Visibility _dryTestVisibility;
        public Visibility DryTestVisibility
        {
            get { return _dryTestVisibility; }
            set
            {
                if (_dryTestVisibility != value)
                {
                    _dryTestVisibility = value;
                    OnPropertyChanged("DryTestVisibility");
                }
            }
        }

        private bool _dryTestEnabled;
        public bool DryTestEnabled
        {
            get { return _dryTestEnabled; }
            set
            {
                if (_dryTestEnabled != value)
                {
                    _dryTestEnabled = value;
                    OnPropertyChanged("DryTestEnabled");
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
