using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using MC_Suite.Helpers;
using MC_Suite.Services;
using MC_Suite.Properties;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.UI.Popups;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{

    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class DryCalibrationPage : Page, INotifyPropertyChanged
    {
        public DryCalibrationPage()
        {
            this.InitializeComponent();
            UpdatePortLists();
            InitRecieveTimer();
            this.DataContext = new DryCalibrationPageViewModel();
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

        private DispatcherTimer RecieveTimer;
        private void InitRecieveTimer()
        {
            RecieveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            RecieveTimer.Tick += RecieveTimer_Tick;
            RecieveTimer.Stop();
        }

        SlaveCOMPortManager.SlaveCmd SimValue = new SlaveCOMPortManager.SlaveCmd();
        private async void RecieveTimer_Tick(object sender, object e)
        {
            RecieveTimer.Stop();


            SimValue = await SlaveManager.ReceiveCommand();
            if (SimValue != null)
            { 
                SlaveManager.SendResponse(SimValue);
                Simulatore.Write((ushort)SimValue.value);
                SimulationValue.Text = SimValue.value.ToString();
            }
            RecieveTimer.Start();
        }        


        private void UpdatePortLists()
        {
            if (ComSetup.COMPortsList.Count > 0)
            {
                ComSetup.COMPorts.Clear();
                ComSetup.COMPortsList.ForEach(p => ComSetup.COMPorts.Add(p));

                //Importare da file CFG
                ComPortComboBox.ItemsSource = ComSetup.COMPorts;

                if (ComSetup.ComPort.ID == null)
                    ComPortComboBox.SelectedIndex = -1;
                else
                {
                    for (int i = 0; i < ComSetup.COMPorts.Count; i++)
                    {
                        if (ComSetup.COMPorts[i].Index == ComSetup.ComPort.Index)
                            ComPortComboBox.SelectedIndex = i;
                    }
                    ComPortMessage.Text = "Seleziona COM Port";
                }
            }
            else
                ComPortMessage.Text = "NO COM PORTS AVAIABLES";
        }

        private ICommand _openSlavePortCmd;
        public ICommand OpenSlavePortCmd
        {
            get
            {
                if (_openSlavePortCmd == null)
                {
                    _openSlavePortCmd = new RelayCommand(
                        param => OpenSlavePort()
                            );
                }
                return _openSlavePortCmd;
            }
        }

        private void OpenSlavePort()
        {
            OpenSlaveCOM(ComSetup.ComPortSlave);
            ComPortMessage.Text = "OK";
        }

        private async void OpenSlaveCOM(Settings.COMPortItem _port)
        {
            if (_port.ID == null)
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "COM Slave Port",
                    Content = "NO Com Port Selected",
                    CloseButtonText = "OK",
                };
                await dialog.ShowAsync();
            }
            else
            {
                SlaveManager.Open(_port);
            }
        }

        
        private ICommand _sendDataCmd;
        public ICommand SendDataCmd
        {
            get
            {
                if (_sendDataCmd == null)
                {
                    _sendDataCmd = new RelayCommand(
                        param => SendData()
                            );
                }
                return _sendDataCmd;
            }
        }

        private void SendData()
        {
            List<byte> data = new List<byte>();
            data.Add(0xAA);
            data.Add(0xAA);
            data.Add(0xAA);
            SlaveManager.SendData(data);
        }

        private ICommand _startReceiveDataCmd;
        public ICommand StartReceiveDataCmd
        {
            get
            {
                if (_startReceiveDataCmd == null)
                {
                    _startReceiveDataCmd = new RelayCommand(
                        param => StartReceiveData()
                            );
                }
                return _startReceiveDataCmd;
            }
        }

        private void StartReceiveData()
        {
            SimulationValue.Text = "--";
            RecieveTimer.Start();
        }


        public Settings ComSetup
        {
            get
            {
                return Settings.Instance;
            }
        }

        public SlaveCOMPortManager SlaveManager
        {
            get
            {
                return SlaveCOMPortManager.Instance;
            }
        }

        public SSM1_Com Simulatore
        {
            get
            {
                return SSM1_Com.Instance;
            }
        }

        private void ComPortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #region INotifyPropertyChanged

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
