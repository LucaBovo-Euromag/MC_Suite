using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using Windows.Devices.SerialCommunication;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MC_Suite.Properties;
using MC_Suite.Services;
using MC_Suite.Modbus;
using Windows.UI.Core;


// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MC608MainPage : Page
    {
        public MC608MainPage()
        {
            this.InitializeComponent();
            AggiornaInfo();
            AggiornaMisure();
            InitUpdateTimer();
            SetupVisibility();

            if (MbConnection.IsOpen == false)
                StartModbus();

            MbConnection.PropertyChanged += MbConnection_PropertyChanged;
        }

        private async void StartModbus()
        {
            if (ComSetup.ComPort608.ID == null)
            {
                var dialog = new MessageDialog("Modbus Port Not Found", "COM Port Error");
                await dialog.ShowAsync();
            }
            else
            {
                Settings.Instance.TimeOut = TimeSpan.FromSeconds(5);

                await MbConnection.Open(ComSetup.ComPort608);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateTimerStart();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            UpdateTimerStop();
        }

        private void MbConnection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetupVisibility();
        }

        private void SetupVisibility()
        {
            switch (MbConnection.MbConnectionStatus)
            {
                case MbCOMPortManager.MbConnectionStates.Ready:
                    LoadingVisibility();
                    UpdateTimerStart();
                    break;
                case MbCOMPortManager.MbConnectionStates.Stop:
                    OfflineVisibility();
                    UpdateTimerStop();
                    break;
                case MbCOMPortManager.MbConnectionStates.Working:
                    OnlineVisibility();
                    break;
            }
        }

        private void OfflineVisibility()
        {
            MC608_Display.Visibility = Visibility.Collapsed;
            Frontalino.Visibility = Visibility.Collapsed;
            TotalizersBox.Visibility = Visibility.Collapsed;
            InfoBox.Visibility = Visibility.Collapsed;
            ReadMeasuresBox.Visibility = Visibility.Collapsed;
            OfflineMessage.Visibility = Visibility.Visible;
            LoadingMessage.Visibility = Visibility.Collapsed;
            LoadingBar.Value = 0;
        }

        private void LoadingVisibility()
        {
            MC608_Display.Visibility = Visibility.Collapsed;
            Frontalino.Visibility = Visibility.Collapsed;
            TotalizersBox.Visibility = Visibility.Collapsed;
            InfoBox.Visibility = Visibility.Collapsed;
            ReadMeasuresBox.Visibility = Visibility.Collapsed;
            OfflineMessage.Visibility = Visibility.Collapsed;
            LoadingMessage.Visibility = Visibility.Visible;
            LoadingBar.Value = 1;
        }

        private void OnlineVisibility()
        {
            MC608_Display.Visibility = Visibility.Visible;
            Frontalino.Visibility = Visibility.Visible;
            TotalizersBox.Visibility = Visibility.Visible;
            InfoBox.Visibility = Visibility.Visible;
            ReadMeasuresBox.Visibility = Visibility.Visible;
            OfflineMessage.Visibility = Visibility.Collapsed;
            LoadingMessage.Visibility = Visibility.Collapsed;
            LoadingBar.Value = 2;
            ComSetup.ConverterSelection.MC608.ConfigEnabled = true;
            ComSetup.ConverterSelectionChanged = true;
        }

        #region Istanze
        public Settings ComSetup
        {
            get
            {
                return Settings.Instance;
            }
        }

        public MbCOMPortManager MbConnection
        {
            get
            {
                return MbCOMPortManager.Instance;
            }
        }

        public Protocol ModbusProtocol
        {
            get
            {
                return Protocol.Instance;
            }
        }

        public MC608 MC608_Device
        {
            get
            {
                return MC608.Instance;
            }
        }
        #endregion

        private async void AggiornaInfo()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ConnectionRing.Visibility = Visibility.Collapsed;

                //Info**********************************************************************************************
                ConverterModel.Text = MC608.Convertitore.Modello;
                ConverterMatricola.Text = MC608.Convertitore.Matricola;
                ConverterSerialNumber.Text = MC608.Convertitore.SN_Converitore.ToString();
                RelFW.Text = MC608.Release_FW.Versione.ToString() + "." + MC608.Release_FW.Revisione.ToString();
                RelHW.Text = MC608.Release_HW.Versione.ToString() + "." + MC608.Release_HW.Revisione.ToString();

                SensorModel.Text = MC608.Sensore.Modello;
                SensorDiameter.Text = MC608.Sensore.Diametro.ToString();
                SensorID.Text = MC608.Sensore.Matricola;
                if (MC608.Sensore.Tubo_vuoto)
                    EmptyPipe.IsChecked = true;
                else
                    EmptyPipe.IsChecked = false;
                SensorNotes.Text = MC608.Sensore.Note;
                
                //Parameters***************************************************************************************
                PulseVolumeUnit.Text = MC608_Device.UnitsList[MC608_Device.PulseOutUnit];            
                PulseVolume.Text = MC608_Device.PulseOutVolume.ToString();
                PulseWidth.Text = MC608_Device.PulseOutWidth.ToString();
                FreqOutFS.Text = MC608_Device.Frequenza_FS.ToString();
                ProgOutBox.Text = MC608_Device.ProgOutList[MC608_Device.ProgOutSetup];

                if ((ushort)MC608_Device.ProgOutSetup == 5) //Batching, disabilita la selezione del tipo di ingresso
                {
                    BatchingPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    BatchingPanel.Visibility = Visibility.Collapsed;
                }

                BatchingVolumeBox.Text = MC608_Device.BatchingVolume.ToString();
                BatchingVolumeUnit.Text = MC608_Device.UnitsList[MC608_Device.CountersUnit];
                ProgInBox.Text = MC608_Device.ProgInList[MC608_Device.ProgInSetup];
            });
        }

        private async void AggiornaMisure()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //Measures*****************************************************************************************
                TotalPositiveBox.Text = ValueConverter(MC608_Device.TotalPositive, 4);
                TotalPositiveDisplay.Text = ValueConverter(MC608_Device.TotalPositive, 4);
                TotalPositiveDisplayUnit.Text = MC608_Device.UnitsList[MC608_Device.CountersUnit];
                TotalPositiveUnit.Text = MC608_Device.UnitsList[MC608_Device.CountersUnit];

                TotalNegativeBox.Text = ValueConverter(MC608_Device.TotalNegative, 4);
                TotaleNegativeUnit.Text = MC608_Device.UnitsList[MC608_Device.CountersUnit];

                PartialPositiveBox.Text = ValueConverter(MC608_Device.PartialPositive, 4);
                PartialPositiveUnit.Text = MC608_Device.UnitsList[MC608_Device.CountersUnit];

                PartialNegativeBox.Text = ValueConverter(MC608_Device.PartialNegative, 4);
                PartialNegativeUnit.Text = MC608_Device.UnitsList[MC608_Device.CountersUnit];

                FlowRateVal.Text = ValueConverter(MC608_Device.Flow_scaled, 3);
                FlowRateUnitVal.Text = MC608_Device.UnitsList[MC608_Device.FlowrateUnit];
                FlowRateTimeBaseVal.Text = MC608_Device.TimebaseList[MC608_Device.FlowrateTimebase];
                FlowBar.Value = MC608_Device.Flow_perc;
                FlowPercVal.Text = ValueConverter(MC608_Device.Flow_perc, 1) + " %";
                FlowMsVal.Text = ValueConverter(MC608_Device.Flow_ms, 2) + " m/s";

                FlowFS.Text = ValueConverter(MC608_Device.FlowrateFS, 2) + " " + FlowRateUnitVal.Text + "/" + FlowRateTimeBaseVal.Text;

                CDateTime.Text = MC608_Device.ConvDateTime.ToShortDateString() + " " + MC608_Device.ConvDateTime.ToShortTimeString();
            });
        }

        public int FlowMaxDigit = 5;
        private string ValueConverter(float Value, int ndig)
        {
            string StrValue = Convert.ToString(Value, System.Globalization.CultureInfo.InvariantCulture);
            string StrValueConverted = "0.0";
            if (Value == 0)
            {
                switch (ndig)
                {
                    case 0:
                    StrValueConverted = StrValue;
                        break;
                    case 1:
                    StrValueConverted = StrValue + ".0";
                        break;
                    case 2:
                        StrValueConverted = StrValue + ".00";
                        break;
                    case 3:
                        StrValueConverted = StrValue + ".000";
                        break;
                }
            }
            else
            {
                int CommaIndex = StrValue.IndexOf(".");
                if (ndig == 0)
                {
                    StrValueConverted = StrValue.Substring(0, CommaIndex);
                }
                else
                {
                    int ValueLenght = StrValue.Length;
                    byte DecimalDigits = (byte)(ValueLenght - CommaIndex - 1);
                    if (DecimalDigits < ndig)
                        ndig = DecimalDigits;

                    if (CommaIndex <= FlowMaxDigit)
                        StrValueConverted = StrValue.Substring(0, CommaIndex + 1 + ndig);
                    else
                        StrValueConverted = StrValue.Substring(0, CommaIndex);
                }
            }
            return StrValueConverted;
        }


        #region Update Measures

        private bool Updated;

        private DispatcherTimer UpdateTimer;
        private void InitUpdateTimer()
        {
            UpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Stop();
            ParametersReaded = false;
            Updated = true;
            StartReadMeasuresBtn.Content = "Start";
        }

        private ICommand _startRead;
        public ICommand StartRead
        {
            get
            {
                if (_startRead == null)
                {
                    _startRead = new RelayCommand(
                        param => UpdateTimerStartStop()
                            );
                }
                return _startRead;
            }
        }

        private void UpdateTimerStartStop()
        {
            if(UpdateTimer.IsEnabled == false)
            { 
                UpdateTimer.Start();
                Updated = true;
                StartReadMeasuresBtn.Content = "Stop";
            }
            else
            {
                UpdateTimer.Stop();
                Updated = false;
                StartReadMeasuresBtn.Content = "Start";
            }
        }

        public void UpdateTimerStart()
        {
            if(UpdateTimer.IsEnabled == false)
            { 
                UpdateTimer.Start();
                Updated = true;
                StartReadMeasuresBtn.Content = "Stop";
            }
        }

        public void UpdateTimerStop()
        {
            if(UpdateTimer.IsEnabled == true)
            { 
                UpdateTimer.Stop();
                Updated = false;
                StartReadMeasuresBtn.Content = "Start";
            }
        }

        private bool ParametersReaded;
        private void UpdateTimer_Tick(object sender, object e)
        {
            Update();
        }

        private ICommand _readValues;
        public ICommand ReadValues
        {
            get
            {
                if (_readValues == null)
                {
                    _readValues = new RelayCommand(
                        param => Update()
                            );
                }
                return _readValues;
            }
        }

        private void Update()
        {
            if (Updated)
            {
                if (ParametersReaded)
                    ReadMeasures();
                else
                    ReadRegisters();

                Updated = false;
            }
        }


        private ICommand _readRegistercmd;
        public ICommand ReadRegistercmd
        {
            get
            {
                if (_readRegistercmd == null)
                {
                    _readRegistercmd = new RelayCommand(
                        param => ReadMeasures()
                            );
                }
                return _readRegistercmd;
            }
        }

        private void ReadMeasures()
        {
            if (MbConnection.IsOpen)
            {
                ModbusStatus.Text = "Reading...";
                ConnectionRing.Visibility = Visibility.Visible;
                MbConnection.ReadRegisters(ComSetup.Address, Map.Registri_CMD4.Measure_m_s, Map.Registri_CMD4.ExpPression, 3);
                MbConnection.ReadRegistersCompleted += MbConnection_ReadRegistersCompleted;
            }
            else
                ModbusStatus.Text = "No Connection";
        }

        private void ReadRegisters()
        {
            if (MbConnection.IsOpen)
            {
                ModbusStatus.Text = "Reading...";
                ConnectionRing.Visibility = Visibility.Visible;
                MbConnection.ReadRegisters(ComSetup.Address, Map.Registri_CMD3_16.Release_ModBus, Map.Registri_CMD3_16.ResetTotN, 3);
                MbConnection.ReadRegistersCompleted += MbConnection_ReadRegistersCompleted;
            }
            else
                ModbusStatus.Text = "No Connection";
        }

        private void MbConnection_ReadRegistersCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;
            if (cmd.ReadRegisters_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                ModbusStatus.Text = cmd.ReadRegisters_CommandResult.Message;
                MbConnection.MbConnectionStatus = MbCOMPortManager.MbConnectionStates.Working;

                if (ParametersReaded)
                    AggiornaMisure();
                else
                    AggiornaInfo();

                ParametersReaded = true;
            }
            else
            {
                ModbusStatus.Text = "Fail";
            }
            ConnectionRing.Visibility = Visibility.Collapsed;
            Updated = true;
        }


        #endregion

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

        private void SetConverterIDbtn_Click(object sender, RoutedEventArgs e)
        {
            WriteRegister();
        }

        private void WriteRegister()
        {
            if (MbConnection.IsOpen)
            {
                ConnectionRing.Visibility = Visibility.Visible;
                MbConnection.WriteRegisters(ComSetup.Address, Map.Registri_CMD3_16.Convert_matricola, Map.Registri_CMD3_16.Convert_matricola, 30);
                MbConnection.WriteRegistersCompleted += MbConnection_WriteRegistersCompleted;
            }
        }

        private void MbConnection_WriteRegistersCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;            
            ConnectionRing.Visibility = Visibility.Collapsed;
        }
    }
}
