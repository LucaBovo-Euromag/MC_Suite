using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MC_Suite.Helpers;
using MC_Suite.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using System.Collections.ObjectModel;
using MC_Suite.Properties;
using Windows.UI.Popups;

using System.Linq;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace MC_Suite.Views
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings-codebehind.md
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        public SettingsPage()
        {
            InitializeComponent();

            this.BatteryChart.DataContext = new BatteryChartViewModel();

            SaveCfgBtn.IsEnabled = false;
            SaveCfgBtn1.IsEnabled = false;
            SysTimePicker.Time = DateTime.Now.TimeOfDay;
            SysDatePicker.Date = DateTime.Now.Date;

            IPAddress.Text = GetLocalIp(HostNameType.Ipv4);

            if(AnalogMeasures.PowerMonitorOn)
            {
                ValimSelection.Visibility = Visibility.Visible;
                if (RAM_Configuration.ViewValim == true)
                    VAlimToggle.IsOn = true;
                else
                    VAlimToggle.IsOn = false;
            }
            else
            {
                ValimSelection.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Initialize();
        }

        private void Initialize()
        {
            UpdatePortLists();
            VersionDescription = GetVersionDescription();
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void ThemeChanged_CheckedAsync(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null)
            {
                await ThemeSelectorService.SetThemeAsync((ElementTheme)param);
            }
        }

        #region COMPortSettings

        public Settings ComSetup
        {
            get
            {
                return Settings.Instance;
            }
        }

        private string _comPortMsg;
        public string ComPortMsg
        {
            get { return _comPortMsg; }
            set
            {
                if (value != _comPortMsg)
                {
                    _comPortMsg = value;
                    OnPropertyChanged("ComPortMsg");
                }
            }
        }

        private string _deviceMsg;
        public string DeviceMsg
        {
            get { return _deviceMsg; }
            set
            {
                if (value != _deviceMsg)
                {
                    _deviceMsg = value;
                    OnPropertyChanged("DeviceMsg");
                }
            }
        }

        private void UpdatePortLists()
        {
            if(ComSetup.COMPortsList.Count > 0)
            {
                ComSetup.COMPorts.Clear();
                ComSetup.COMPortsList.ForEach(p => ComSetup.COMPorts.Add(p));

                //Importare da file CFG
                ComPortComboBox.ItemsSource = ComSetup.COMPorts;
                ComPortComboBox608.ItemsSource = ComSetup.COMPorts;                

                if (ComSetup.ComPort.ID == null)
                    ComPortComboBox.SelectedIndex = -1;
                else
                {
                    for (int i = 0; i < ComSetup.COMPorts.Count; i++)
                    {
                        if (ComSetup.COMPorts[i].Index == ComSetup.ComPort.Index)
                            ComPortComboBox.SelectedIndex = i;
                    }
                }

                if (ComSetup.ComPort608.ID == null)
                    ComPortComboBox608.SelectedIndex = -1;
                else
                {
                    for(int i=0;i< ComSetup.COMPorts.Count;i++)
                    {
                        if(ComSetup.COMPorts[i].Index == ComSetup.ComPort608.Index)
                            ComPortComboBox608.SelectedIndex = i;
                    }
                }
            }
            else
                DeviceMsg = "NO COM PORTS AVAIABLES";
        }

        #endregion

        #region ConfigurationFile

        public Storage FileManager
        {
            get
            {
                return Storage.Instance;
            }
        }

        public Configuration RAM_Configuration
        {
            get
            {
                return Configuration.Instance;
            }
        }

        public AnalogsService AnalogMeasures
        {
            get
            {
                return AnalogsService.Instance;
            }
        }

        public VerificatorConfig RAM_VerifConfiguration
        {
            get
            {
                return VerificatorConfig.Instance;
            }
            set {; }
        }


        private ICommand _saveConfigCmd;
        public ICommand SaveConfigCmd
        {
            get
            {
                if (_saveConfigCmd == null)
                {
                    _saveConfigCmd = new RelayCommand(
                        param => SaveConfig()
                            );
                }
                return _saveConfigCmd;
            }
        }

        private void SaveConfig()
        {
            //Salvo la Configurazione
            RAM_Configuration.ComPort       = ComSetup.ComPort;
            RAM_Configuration.Protocol      = ComSetup.Protocol;
            RAM_Configuration.ComPort608    = ComSetup.ComPort608;
            RAM_Configuration.Protocol608   = ComSetup.Protocol608;
            RAM_Configuration.MbAddress     = ComSetup.Address;
            RAM_Configuration.MbBaudRate    = ComSetup.BaudRate;
            RAM_Configuration.MbParity      = ComSetup.Parity;
            RAM_Configuration.MbDataBits    = ComSetup.DataBits;
            RAM_Configuration.MbTimeOut     = ComSetup.TimeOut;

            List<Configuration> NewCfg = new List<Configuration>();
            NewCfg.Add(RAM_Configuration);
            SerializableStorage<Configuration>.Save(FileManager.ConfigFile, FileManager.MainFolder.Path, NewCfg);
            SaveCfgBtn.IsEnabled = false;
            SaveCfgBtn1.IsEnabled = false;
        }

        #endregion

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

        private void ProtocolBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComSetup.Protocol = (byte)ProtocolBox.SelectedValue;
            SaveCfgBtn.IsEnabled = true;
        }

        private void BaudRateBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComSetup.BaudRate = (uint)BaudRateBox.SelectedValue;
            SaveCfgBtn1.IsEnabled = true;
        }

        private void ParityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComSetup.Parity = (byte)ParityBox.SelectedValue;
            SaveCfgBtn1.IsEnabled = true;
        }

        private void DataBitsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComSetup.DataBits = (ushort)DataBitsBox.SelectedValue;
            SaveCfgBtn1.IsEnabled = true;
        }

        private void StopBitsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComSetup.StopBits = (byte)StopBitsBox.SelectedValue;
            SaveCfgBtn1.IsEnabled = true;
        }

        private async void AddressVal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if((AddressVal.Text != "")&&(AddressVal.Text != string.Empty))
            { 
                try
                {
                    ComSetup.Address = Convert.ToByte(AddressVal.Text);
                }
                catch
                {
                    var dialog = new MessageDialog("Please insert valid Address");
                    await dialog.ShowAsync();
                    AddressVal.Text = string.Empty;
                    return;
                }
                finally
                {
                    SaveCfgBtn1.IsEnabled = true;
                }
            }
        }

        private void ComPortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComSetup.ComPort = ComPortComboBox.SelectedValue as Settings.COMPortItem;
            SaveCfgBtn.IsEnabled = true;
        }

        private void ProtocolBox608_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComSetup.Protocol608 = (byte)ProtocolBox608.SelectedValue;
            SaveCfgBtn1.IsEnabled = true;
        }

        private void ComPortComboBox608_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComSetup.ComPort608 = ComPortComboBox608.SelectedValue as Settings.COMPortItem;
            SaveCfgBtn1.IsEnabled = true;
        }


        private string _sysDateTime;
        public string SysDateTime
        {
            get { return _sysDateTime; }
            set
            {
                if (value != _sysDateTime)
                {
                    _sysDateTime = value;
                    OnPropertyChanged("SysDateTime");
                }
            }
        }


        #region Local IP
        public static string GetLocalIp(HostNameType hostNameType = HostNameType.Ipv4)
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();
            IReadOnlyList<HostName> HostNames;

            if (icp != null)
            {
                if (icp?.NetworkAdapter == null) return "0.0.0.0";

                HostNames = NetworkInformation.GetHostNames();

                if (HostNames.Count != 0)
                {
                    HostName hostname = HostNames.FirstOrDefault(
                                hn =>
                                hn.Type == hostNameType &&
                                hn.IPInformation?.NetworkAdapter != null &&
                                hn.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId);
                    // the ip address
                    return hostname?.CanonicalName;
                }
                else
                    return "0.0.0.0";
            }
            return "0.0.0.0";
        }
        #endregion

        private void SysDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            Windows.System.DateTimeSettings.SetSystemDateTime(e.NewDate.UtcDateTime);
        }

        private void SysTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            var currentDate = DateTime.Now.ToUniversalTime();

            var newDateTime = new DateTime(currentDate.Year,
                                           currentDate.Month,
                                           currentDate.Day,
                                           e.NewTime.Hours,
                                           e.NewTime.Minutes,
                                           e.NewTime.Seconds);

            Windows.System.DateTimeSettings.SetSystemDateTime(newDateTime);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(passwordBox.Password == "160518")
            {                             
                List<VerificatorConfig> ConfigList = new List<VerificatorConfig>();
                RAM_VerifConfiguration.TarMode = true;
                RunningConfiguration.Instance.QuickTarEnabled = true;
                ConfigList.Add(RAM_VerifConfiguration);
                SerializableStorage<VerificatorConfig>.Save(FileManager.VerificatorConfigFile, FileManager.MainFolder.Path, ConfigList);

                passwordResultTxt.Text = "Calibration Mode Active";
            }
            if (passwordBox.Password == "231042")
            {
                RunningConfiguration.Instance.QuickTarEnabled = true;
                passwordResultTxt.Text = "Quick Calibration Mode Active";
            }
        }

        private void VAlimToggle_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    RAM_Configuration.ViewValim = true;
                }
                else
                {
                    RAM_Configuration.ViewValim = false;
                }
                SaveConfig();
            }            
        }
    }
}
