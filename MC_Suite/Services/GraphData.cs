using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml;
using MC_Suite.Services;
using MC_Suite.Properties;
using MC_Suite.Modbus;
using System.Runtime.CompilerServices;

namespace MC_Suite.Services
{
    public class GraphData : INotifyPropertyChanged
    {
        private static GraphData _instance;
        public static GraphData Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GraphData();
                return _instance;
            }
        }

        public ObservableCollection<GraphValue> Graph { get; set; }
        public ObservableCollection<BatteryGraphValue> BatteryGraph { get; set; }
        public DispatcherTimer UpdateTimer { get; set; }

        public enum GraphModes
        {
            Off,
            Recording,
            Stop
        }

        private GraphModes _graphRecording;
        public GraphModes GraphRecording
        {
            get { return _graphRecording; }
            set
            {
                if (value != _graphRecording)
                {
                    _graphRecording = value;
                    OnPropertyChanged("GraphRecording");
                }
            }
        }

        private int _batteryPercValue;
        public int BatteryPercValue
        {
            get { return _batteryPercValue; }
            set
            {
                if (value != _batteryPercValue)
                {
                    _batteryPercValue = value;
                    OnPropertyChanged("BatteryPercValue");
                }
            }
        }

        private bool _charging;
        public bool Charging
        {
            get { return _charging; }
            set
            {
                if (value != _charging)
                {
                    _charging = value;
                    OnPropertyChanged("Charging");
                }
            }
        }

        #region ObservableObject

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
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
