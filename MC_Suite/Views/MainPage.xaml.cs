using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using System.Windows.Input;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using MC_Suite.Services;
using MC_Suite.Properties;
using MC_Suite.Modbus;
using MC_Suite.Euromag.Devices;
using MC_Suite.Euromag.Protocols;
using MC_Suite.Euromag.Protocols.StdCommands;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Uwp;


namespace MC_Suite.Views
{

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {

        public MainPage()
        {
            this.InitializeComponent();
            InitBundles();
            LoadingBox.Visibility = Visibility.Collapsed;
            LoadingBar.Visibility = Visibility.Collapsed;
            ClockSettings.Visibility = Visibility.Collapsed;
            SystemClock.Visibility = Visibility.Visible;

            Fields.KaRatio.PropertyChanged += KaRatio_PropertyChanged;

            //Sys Clock Refresh*********************************************************************
            DispatcherTimer RefreshTimer = new DispatcherTimer();
            RefreshTimer.Interval = TimeSpan.FromMilliseconds(5000);
            RefreshTimer.Tick += RefreshTimer_Tick;
            RefreshTimer.Start();
            SysDate = DateTime.Now.ToString("dd MMMM yyyy");
            SysTime = DateTime.Now.ToString("HH:mm");
        }

        private string _sysDate;
        public string SysDate
        {
            get { return _sysDate; }
            set
            {
                if (value != _sysDate)
                {
                    _sysDate = value;
                    OnPropertyChanged("SysDate");
                }
            }
        }

        private string _sysTime;
        public string SysTime
        {
            get { return _sysTime; }
            set
            {
                if (value != _sysTime)
                {
                    _sysTime = value;
                    OnPropertyChanged("SysTime");
                }
            }
        }

             
        private byte _bundleIndex;
        public byte BundleIndex
        {
            get { return _bundleIndex; }
            set
            {
                if (value != _bundleIndex)
                {
                    _bundleIndex = value;
                    OnPropertyChanged("BundleIndex");
                }
            }
        }

        private byte _bundleIndexMax;
        public byte BundleIndexMax
        {
            get { return _bundleIndexMax; }
            set
            {
                if (value != _bundleIndexMax)
                {
                    _bundleIndexMax = value;
                    OnPropertyChanged("BundleIndexMax");
                }
            }
        }


        private void RefreshTimer_Tick(object sender, object e)
        {
            SysDate = DateTime.Now.ToString("dd MMMM yyyy");
            SysTime = DateTime.Now.ToString("HH:mm");
        }

        #region Init Bundles

        private void InitBundles()
        {
            string dummyRead;
            dummyRead = Fields.AccumulatorNdigit.ValAsString;
            dummyRead = Fields.AccumulatorsTechUnit.ValAsString;
            dummyRead = Fields.AdcGain.ValAsString;
            dummyRead = Fields.AlignmentCalibrationFactor.ValAsString;
            dummyRead = Fields.AlignmentOffset.ValAsString;
            dummyRead = Fields.AwakeMeasurePeriod.ValAsString;
            dummyRead = Fields.BatteryAutosave.ValAsString;
            dummyRead = Fields.Bypass.ValAsString;
            dummyRead = Fields.BypassCount.ValAsString;
            dummyRead = Fields.CalibrationDate.ValAsString;
            dummyRead = Fields.CalibrationTemperature.ValAsString;
            dummyRead = Fields.CalibrationVoltage.ValAsString;
            dummyRead = Fields.ConverterId.ValAsString;
            dummyRead = Fields.ConverterSerialNumber.ValAsString;
            dummyRead = Fields.Cutoff.ValAsString;
            dummyRead = Fields.DAC161_Out_mA.ValAsString;
            dummyRead = Fields.DAC161_State.ValAsString;
            dummyRead = Fields.Damping.ValAsString;
            dummyRead = Fields.DampingSlow.ValAsString;
            dummyRead = Fields.DeltaTempMin.ValAsString;
            dummyRead = Fields.DensityCoeffA.ValAsString;
            dummyRead = Fields.DensityCoeffB.ValAsString;
            dummyRead = Fields.DensityCoeffC.ValAsString;
            dummyRead = Fields.DensityCoeffD.ValAsString;
            dummyRead = Fields.DeviceName.ValAsString;
            dummyRead = Fields.EmptyPipeCfg.ValAsString;
            dummyRead = Fields.EmptyPipeFreq.ValAsString;
            dummyRead = Fields.EmptyPipeFreqFast.ValAsString;
            dummyRead = Fields.EmptyPipeRelease.ValAsString;
            dummyRead = Fields.EmptyPipeThreshold.ValAsString;
            dummyRead = Fields.EnergyOption.ValAsString;
            dummyRead = Fields.EventsCount.ValAsString;
            dummyRead = Fields.ExcitationPause.ValAsString;
            dummyRead = Fields.FileSystem_FileCount.ValAsString;
            dummyRead = Fields.FileSystem_Flags.ValAsString;
            dummyRead = Fields.FileSystem_SavedEvents.ValAsString;
            dummyRead = Fields.FileSystem_SavedLogs.ValAsString;
            dummyRead = Fields.FlowrateFullscale.ValAsString;
            dummyRead = Fields.FlowRateMS.ValAsString;
            dummyRead = Fields.FlowrateNdigit.ValAsString;
            dummyRead = Fields.FlowRatePERC.ValAsString;
            dummyRead = Fields.FlowrateTechUnit.ValAsString;
            dummyRead = Fields.FlowrateTimeBase.ValAsString;
            dummyRead = Fields.FlowRateTU.ValAsString;
            dummyRead = Fields.FwRevision.ValAsString;
            dummyRead = Fields.FwVersion.ValAsString;
            dummyRead = Fields.InputStageStabTime.ValAsString;
            dummyRead = Fields.Insertion_Interp_Th.ValAsString;
            dummyRead = Fields.Insertion_sA_HI.ValAsString;
            dummyRead = Fields.Insertion_sA_LO.ValAsString;
            dummyRead = Fields.Insertion_sB_HI.ValAsString;
            dummyRead = Fields.Insertion_sB_LO.ValAsString;
            dummyRead = Fields.Insertion_sC_HI.ValAsString;
            dummyRead = Fields.Insertion_sC_LO.ValAsString;
            dummyRead = Fields.Insertion_sD_HI.ValAsString;
            dummyRead = Fields.Insertion_sD_LO.ValAsString;
            dummyRead = Fields.KaRatio.ValAsString;
            dummyRead = Fields.LeftuAh.ValAsString;
            dummyRead = Fields.LogLastRow.ValAsString;
            dummyRead = Fields.LowPowerMeasurePeriod.ValAsString;
            dummyRead = Fields.MainsLineFrequency.ValAsString;
            dummyRead = Fields.Manufacturer.ValAsString;
            dummyRead = Fields.OffsetReg.ValAsString;
            dummyRead = Fields.OtherFeatures.ValAsString;
            dummyRead = Fields.PartialNegativeM3.ValAsString;
            dummyRead = Fields.PartialPositiveM3.ValAsString;
            dummyRead = Fields.Password.ValAsString;
            dummyRead = Fields.PasswordTimeout.ValAsString;
            dummyRead = Fields.Peakcut.ValAsString;
            dummyRead = Fields.PeakcutCount.ValAsString;
            dummyRead = Fields.PressureMeasurePeriod.ValAsString;
            dummyRead = Fields.PressureOption.ValAsString;
            dummyRead = Fields.ProcessLogPeriod.ValAsString;
            dummyRead = Fields.PulseLength.ValAsString;
            dummyRead = Fields.PulseOutputTechUnit.ValAsString;
            dummyRead = Fields.PulseOutputVolume.ValAsString;
            dummyRead = Fields.SD24SamplingIndex.ValAsString;
            dummyRead = Fields.SensorDiameter.ValAsString;
            dummyRead = Fields.SensorId.ValAsString;
            dummyRead = Fields.SensorIsInsertion.ValAsString;
            dummyRead = Fields.SensorModel.ValAsString;
            dummyRead = Fields.SensorOffset.ValAsString;
            dummyRead = Fields.SpecificHeatCoeffA.ValAsString;
            dummyRead = Fields.SpecificHeatCoeffB.ValAsString;
            dummyRead = Fields.SpecificHeatCoeffC.ValAsString;
            dummyRead = Fields.SpecificHeatCoeffD.ValAsString;
            dummyRead = Fields.TemperatureMeasurePeriod.ValAsString;
            dummyRead = Fields.TemperatureOffset.ValAsString;
            dummyRead = Fields.TimeoutToMain.ValAsString;
            dummyRead = Fields.TotalMeasuresCount.ValAsString;
            dummyRead = Fields.TotalNegativeM3.ValAsString;
            dummyRead = Fields.TotalPositiveM3.ValAsString;
            dummyRead = Fields.TotalTimeAwake.ValAsString;
            dummyRead = Fields.TotalTimeInLowPower.ValAsString;
            dummyRead = Fields.TotaluAh.ValAsString;
            dummyRead = Fields.WakeupPoweroff.ValAsString;
            dummyRead = Fields.WaterDetectMeasEnable.ValAsString;
            dummyRead = Fields.WaterDetectMeasThreshold.ValAsString;
        }

        #endregion

        #region Istanze

        public Settings Config
        {
            get
            {
                return Settings.Instance;
            }
        }

        public TargetVariablesFields Fields
        {
            get
            {
                return TargetVariablesFields.Instance;
            }
        }

        public Settings ComSetup
        {
            get
            {
                return Settings.Instance;
            }
        }

        public IrCOMPortManager IrConnection
        {
            get
            {
                return IrCOMPortManager.Instance;
            }
        }

        public MbCOMPortManager MbConnection
        {
            get
            {
                return MbCOMPortManager.Instance;
            }
        }

        public AnalogsService AnalogMeasures
        {
            get
            {
                return AnalogsService.Instance;
            }
        }

        public SimulatorCOMPortManager SensorSimulator
        {
            get
            {
                return SimulatorCOMPortManager.Instance;
            }
        }

        public GPIO_Device GPIO_Control
        {
            get
            {
                return GPIO_Device.Instance;
            }
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

        private void MC406Select_Click(object sender, RoutedEventArgs e)
        {
            Config.ConverterSelection.MC406.Connected = true;
            Config.ConverterSelection.MC608.Connected = false;
            ComSetup.SensorOnly                       = false;
            Config.ConverterSelectionChanged = true;
            BundleIndexMax          = 10;
            LoadingBox.Visibility   = Visibility.Visible;
            LoadingBar.Visibility = Visibility.Visible;
            Config.SensorOnly = false;

            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            if (IrConnection.IsOpen)
                IrConnection.Close();

            if (MbConnection.IsOpen)
                MbConnection.Close();

            ComSetup.Verificator406PageReady = false;
            StartComPort( ComSetup.Protocol, ComSetup.ComPort );            
        }

        private void MC608Select_Click(object sender, RoutedEventArgs e)
        {
            Config.ConverterSelection.MC406.Connected = false;
            Config.ConverterSelection.MC608.Connected = true;
            ComSetup.SensorOnly                       = false;
            Config.ConverterSelectionChanged = true;
            LoadingBox.Visibility = Visibility.Visible;
            LoadingBar.Visibility = Visibility.Collapsed;
            Config.SensorOnly = false;

            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            if (IrConnection.IsOpen)
                IrConnection.Close();

            if (MbConnection.IsOpen)
                MbConnection.Close();

            ComSetup.Verificator608PageReady = false;
            StartComPort( ComSetup.Protocol608, ComSetup.ComPort608 );
        }

        #region Start Connection

        private void StartComPort(byte _protocol, Settings.COMPortItem _ComPort)
        {
            switch (_protocol)
            {
                default:
                case 0: //IrCOM
                    StartIrCOM( _ComPort );
                    break;
                case 1: //Modbus
                    StartModbus( _ComPort );
                    break;

            }
        }

        #region IrCom Connection

        private async void StartIrCOM( Settings.COMPortItem _port )
        {
            if (IrConnection.IsOpen)
            {
                if (IrConnection.IrConnectionStatus == IrCOMPortManager.IrConnectionStates.Stop)
                {
                    IrConnection.Start();
                }

                GPIO_Control.InitGPIO();

                if (AnalogMeasures.ADC_ModuleIsReady == false)
                    await AnalogMeasures.ADC_Module_Open();

                this.Frame.Navigate(typeof(VerificatorPage));
            }
            else
            {
                OpenIrCOM( _port );
            }
        }

        private async void OpenIrCOM( Settings.COMPortItem _port )
        {
            if (_port.ID == null)
            {
                var dialog = new MessageDialog("NO Com Port");
                await dialog.ShowAsync();
                LoadingBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                IrConnection.Open(_port, IrCOMPortManager.ComMode.Manual);
                InitReadTimer();
            }
        }

        private DispatcherTimer ReadTimer;
        private void InitReadTimer()
        {
            ReadTimer = new DispatcherTimer();
            ReadTimer.Interval = TimeSpan.FromMilliseconds(2000);
            ReadTimer.Tick += ReadTimer_Tick;
            BundleReaded = false;
            BundleIndex = 1;
            BundleAutoreading = true;
            ReadTimer.Start();
        }
        
        private bool BundleReaded;
        private static bool BundleAutoreading;
        private void BundleAutoRead()
        {
            IrConnection.ReadBundleCompleted += BundleAutoReadCompleted;
            IrConnection.ReadBundle(BundleIndex);
            ReadTimer.Stop();
        }

        private void BundleAutoReadCompleted(object sender, PropertyChangedEventArgs e)
        {
            IrCOMPortManager cmd = sender as IrCOMPortManager;

            if (IrConnection.ReadBundle_CommandResult.Outcome == Euromag.Protocols.CommandResultOutcomes.CommandSuccess)
                BundleReaded = true;
            else
                BundleReaded = false;

            if (BundleAutoreading)
                ReadTimer.Start();
            else
                ReadTimer.Stop();
        }

        private void KaRatio_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {            
            /*if(BundleAutoreading)
            {
                ReadTimer.Stop();
                BundleAutoreading = false;
                Settings.Instance.Verificator406PageReady = false;
                this.Frame.Navigate(typeof(VerificatorPage));
            }*/
        }

        private async void ReadTimer_Tick(object sender, object e)
        {
            if (BundleReaded)
            {
                if (BundleIndex < BundleIndexMax)
                {
                    BundleIndex++;
                }
                else
                {
                    BundleIndex       = 1;
                    BundleAutoreading = false;

                    GPIO_Control.InitGPIO();

                    if (AnalogMeasures.ADC_ModuleIsReady == false)
                        await AnalogMeasures.ADC_Module_Open();

                    this.Frame.Navigate(typeof(VerificatorPage));
                }
                BundleAutoRead();
                BundleReaded = false;
            }
            else
                BundleAutoRead();
        }

        #endregion

        private async void StartModbus(Settings.COMPortItem _port)
        {
            if (_port.ID == null)
            {
                var dialog = new MessageDialog("NO Com Port");
                await dialog.ShowAsync();
                LoadingBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                Settings.Instance.TimeOut = TimeSpan.FromSeconds(5);

                if (await MbConnection.Open( _port ))
                {
                    MC608.Reset();
                    MC608.Tipo_Connessione = MC608.Comunicazione.Connesso_ModBus;

                    GPIO_Control.InitGPIO();

                    if (AnalogMeasures.ADC_ModuleIsReady == false)
                        await AnalogMeasures.ADC_Module_Open();

                    this.Frame.Navigate(typeof(VerificatorPage608));
                }
            }
        }

        #endregion

        private void SensorSelect_Click(object sender, RoutedEventArgs e)
        {
            Config.ConverterSelection.MC406.Connected = false;
            Config.ConverterSelection.MC608.Connected = false;
            Config.SensorOnly                         = true;
            Config.ConverterSelectionChanged          = true;
            
            this.Frame.Navigate(typeof(SensorOnlyPage));
        }

        private void SetClockButton_Click(object sender, RoutedEventArgs e)
        {
            ClockSettings.Visibility = Visibility.Visible;
            SystemClock.Visibility = Visibility.Collapsed;

            newDate = DateTime.Now;
            newTime = DateTime.Now;

            SetTimePicker.Time = new TimeSpan(DateTime.Now.Ticks);
            SetDatePicker.Date = new DateTimeOffset(DateTime.Now);
        }

        private void SaveClockButton_Click(object sender, RoutedEventArgs e)
        {
            ClockSettings.Visibility = Visibility.Collapsed;
            SystemClock.Visibility = Visibility.Visible;

            DateTime DateTimeToSet = new DateTime(newDate.Year,
                                                  newDate.Month,
                                                  newDate.Day,
                                                  newTime.Hour,
                                                  newTime.Minute,
                                                  newTime.Second);

            Windows.System.DateTimeSettings.SetSystemDateTime(DateTimeToSet);
        }

        private DateTime newTime;
        private DateTime newDate;
        private void SetTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            var currentDate = DateTime.Now.ToUniversalTime();

            newTime = new DateTime(currentDate.Year,
                                    currentDate.Month,
                                    currentDate.Day,
                                    e.NewTime.Hours,
                                    e.NewTime.Minutes,
                                    e.NewTime.Seconds);                        
        }

        private void SetDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            newDate = e.NewDate.UtcDateTime;            
        }
    }
}
