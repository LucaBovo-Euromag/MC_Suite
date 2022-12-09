using MC_Suite.Properties;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.SerialCommunication;
using System;


namespace MC_Suite.Services
{
    public class Configuration : INotifyPropertyChanged
    {
        private static Configuration _instance;
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Configuration();
                return _instance;
            }
        }

        public Settings.COMPortItem ComPort;
        public byte Protocol;

        public Settings.COMPortItem ComPort608;
        public byte Protocol608;

        #region Parametri_ModBus
            public byte MbAddress;
            public uint MbBaudRate;
            public byte MbParity;
            public ushort MbDataBits;
            public byte MbStopBits;
            public TimeSpan MbTimeOut;
        #endregion

        #region Parametri Info Verificator      
            public string Operator;
            public string Company;
            public string Company_Location;
            public string Customer;
            public string Customer_Location;
            public string Note;
        #endregion

        public bool ViewValim   = false;

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

    public class RunningConfiguration : INotifyPropertyChanged
    {
        private static RunningConfiguration _instance;
        public static RunningConfiguration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RunningConfiguration();
                return _instance;
            }
        }

        public bool QuickTarEnabled;

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
