using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace MC_Suite.Services
{
    public class VerificatorConfig : INotifyPropertyChanged
    {
        private static VerificatorConfig _instance;
        public static VerificatorConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VerificatorConfig();
                return _instance;
            }
        }

        public DateTime DataCreazioneFile;

        public bool TarMode             = true;

        public string DataLastTaratura = DateTime.Now.ToString("dd/MM/yyyy HH:mm"); // Ultima taratura eseguita in Euromag
        public string DataNextTaratura = DateTime.Now.AddYears(1).ToString("dd/MM/yyyy HH:mm"); // Prossima taratura definita dopo 1 anno
        public string SN_Verificator = "1234567"; // Da assegnare poi Al file XML
        public string SW_Ver_Verificator = VersionFull;

        public float Vbattery0        = 11.7f; // Volt 0%
        public float Vbattery100      = 12.7f; // Volt batteria 100%

        public float Out4_20mA_Offs    = 0;
        public float Out4_20mA_Gain    = 1;

        public float Out4_12mA_Offs    = 0;
        public float Out4_12mA_Gain    = 1;

        public float Icoil_Offs        = 0;
        public float Icoil_Gain        = 1;

        public float VAlim_Offs        = 0;
        public float VAlim_Gain        = 1;

        private static Version Version { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; } }
        private static string VersionFull { get { return Version.ToString(); } }
        private static string VersionMajor { get { return Version.Major.ToString(); } }
        private static string VersionMinor { get { return Version.Minor.ToString(); } }
        private static string VersionBuild { get { return Version.Build.ToString(); } }
        private static string VersionRevision { get { return Version.Revision.ToString(); } }

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
