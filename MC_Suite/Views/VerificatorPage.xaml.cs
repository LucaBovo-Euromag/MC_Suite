using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using MC_Suite.Services;
using MC_Suite.Services.Printing;
using MC_Suite.Properties;
using MC_Suite.Euromag.Protocols.StdCommands;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MC_Suite.Modbus;



// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class VerificatorPage : Page
    {
        public VerificatorPage()
        {
            this.InitializeComponent();            

            IrConnection.PropertyChanged += IrConnection_PropertyChanged;
            Fields.ConverterId.PropertyChanged += ConverterId_PropertyChanged;

            if (RAM_Configuration.Operator != null)
                OperatorName.Text = RAM_Configuration.Operator;

            if (RAM_Configuration.Company != null)
                CompanyName.Text = RAM_Configuration.Company;

            if (RAM_Configuration.Company_Location != null)
                CompanyLocation.Text = RAM_Configuration.Company_Location;

            if (RAM_Configuration.Customer != null)
                CustomerName.Text = RAM_Configuration.Customer;

            if (RAM_Configuration.Customer_Location != null)
                CustomerLocation.Text = RAM_Configuration.Customer_Location;

            if (RAM_Configuration.Note != null)
                Note.Text = RAM_Configuration.Note;

            InitSensorPingTimer();
            InitIOTestTimer();
            InitECoilTest();
            InitTestSimulation();
            InitMonitorTimer();
            InitReadTimer();

            ManSensorModel.Visibility   = Visibility.Collapsed;
            ManSensorId.Visibility      = Visibility.Collapsed;
            ReadingProgress.Visibility  = Visibility.Collapsed;
            ReadingProgress.Value       = 0;

            LoadingBox.Visibility       = Visibility.Collapsed;

            DisableConverterTest();
            DisableSensorTest();
            AbortSensTestBtn.IsEnabled = false;

            //StartI2CAsync();
            GPioState.Fill = GPIO_Control.InitGPIO();

            if (ComSetup.Verificator406PageReady == false)
            {
                TestRunning = VerificatorTests.None;

                RisultatiTest.Init_Sensore();
                RisultatiTest.Init_IO();
                RisultatiTest.Init_ICoil();
                RisultatiTest.Init_AnalogOut();
                RisultatiTest.Init_Simulazione();
                RisultatiTest.Init_AnalogOut();

                InterfacciaConv.Write(Parametri.Simul.DAC_Zero);
                EPipeStatus.Fill = GPIO_Control.SetEPipe();

                //CheckDatabaseTest(true);

                ComSetup.Verificator406PageReady = true;
            }
            else
                ConncetionRing.Visibility = Visibility.Collapsed;


            if (IrConnection.IrConnectionStatus == IrCOMPortManager.IrConnectionStates.Connected)
            {
                ConverterState.Fill = new SolidColorBrush(Colors.LimeGreen);                
                IrConnection.Close();
            }
            else
                InitConnections();

            if (SensorSimulator.IsOpen)
            {
                SimulatorState.Fill = new SolidColorBrush(Colors.LimeGreen);
                EnableSensorTest();
            }
            else
            {
                OpenSensorSimulatorCom();
            }

            if (AnalogMeasures.ADC_Module.Ready)
            {
                VerificatorState.Fill = new SolidColorBrush(Colors.LimeGreen);
                AnalogMeasures.ADC_MeasuresStart();
                RefreshAnalogsStart();
            }

            Simulation_Read.Text = "-.-- m/s";            
        }

        private static int OldTestIndex;
        private static bool OldTestFound;

        private void ConverterId_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if((ConverterTestIsEnabled == false) && (TestRunning == VerificatorTests.None))
            {
                //CheckDatabaseTest(true);
            }
        }

        private async void CheckDatabaseTest(bool IsStartup)
        {
            OldTestIndex = 0;
            OldTestFound = false;

            for (int i = 0; i < Verificator.ReportList.Count; i++)
            {
                if (Verificator.ReportList[i].Matricola_Convertitore.Equals(Fields.ConverterId.Value))
                {
                    OldTestIndex = i;
                    OldTestFound = true;
                }
            }

            if(OldTestFound)
                LastTestInfo.Text = "Last saved test on " + Verificator.ReportList[OldTestIndex].Data_Test;

            if (IsStartup)
            {
                if (OldTestFound)
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "Report Database",
                        Content = "Converter " + Fields.ConverterId.Value + " Found\n" +
                                  "Last test on " + Verificator.ReportList[OldTestIndex].Data_Test,
                        CloseButtonText = "OK",
                    };

                    await dialog.ShowAsync();
                }
                else
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "Report Database",
                        Content = "Converter " + Fields.ConverterId.Value + " Not Found\n" +
                                  "No test data, default parameters will be used",
                        CloseButtonText = "OK",
                    };

                    await dialog.ShowAsync();
                }
            }


            ConverterState.Fill = new SolidColorBrush(Colors.LimeGreen);
            EnableConverterTest();
            ConncetionRing.Visibility = Visibility.Collapsed;
        }

        private void ReadPulse_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            ReadEEPROM cmd = sender as ReadEEPROM;
            if (cmd.Result != null)
            {
                if (cmd.Result.Outcome == Euromag.Protocols.CommandResultOutcomes.CommandSuccess)
                {
                    if(TechUnitRead == false)
                    {
                        ReadPulse.Variable = Fields.PulseOutputVolume;
                        ReadPulse.send();
                        TechUnitRead = true;
                    }
                    else
                    {
                        ConverterState.Fill = new SolidColorBrush(Colors.LimeGreen);
                        EnableConverterTest();
                        ConncetionRing.Visibility = Visibility.Collapsed;
                    }
                }
                else
                    ReadPulse.send();
            }                
        }

        private ReadEEPROM ReadPulse;
        private bool TechUnitRead;


        private enum VerificatorTests
        {
            None,
            Conv_Simulation,
            Conv_EnergyCoil,
            Conv_IO,
            Sensor            
        }
        VerificatorTests TestRunning;

        #region Abilitazioni


        private static bool ConverterTestIsEnabled;
        private void DisableConverterTest()
        {
            SimulationBtn.IsEnabled     = false;
            ECoilTestbtn.IsEnabled      = false;
            TestIObtn.IsEnabled         = false;
            AllTestBtn.IsEnabled        = false;
            AbortConvTestBtn.IsEnabled  = true;
            if(TestRunning == VerificatorTests.None)
                ConverterTestIsEnabled  = false;
            ClearAllButton.IsEnabled    = false;
            SaveReportBtn.IsEnabled     = false;
            OpenComBtn.IsEnabled        = false;
            TestDryBtn.IsEnabled        = false;
            TestWetBtn.IsEnabled        = false;
            AbortSensTestBtn.IsEnabled  = false;
            SaveInfoBtn.IsEnabled       = false;
        }

        private void EnableConverterTest()
        {
            SimulationBtn.IsEnabled     = true;
            ECoilTestbtn.IsEnabled      = true;
            TestIObtn.IsEnabled         = true;
            AllTestBtn.IsEnabled        = true;
            AbortConvTestBtn.IsEnabled  = false;
            ConverterTestIsEnabled      = true;
            ClearAllButton.IsEnabled    = true;
            SaveReportBtn.IsEnabled     = true;
            OpenComBtn.IsEnabled        = true;
            TestDryBtn.IsEnabled        = true;
            TestWetBtn.IsEnabled        = true;
            AbortSensTestBtn.IsEnabled  = false;
            SaveInfoBtn.IsEnabled       = true;
        }

        private void DisableSensorTest()
        {
            SimulationBtn.IsEnabled = false;
            ECoilTestbtn.IsEnabled = false;
            TestIObtn.IsEnabled = false;
            AllTestBtn.IsEnabled = false;
            AbortConvTestBtn.IsEnabled = false;
            if (TestRunning == VerificatorTests.None)
                ConverterTestIsEnabled = false;

            TestDryBtn.IsEnabled        = false;
            TestWetBtn.IsEnabled        = false;
            AbortSensTestBtn.IsEnabled  = true;
            ClearAllButton.IsEnabled    = false;
            SaveReportBtn.IsEnabled     = false;
            SaveInfoBtn.IsEnabled       = false;
            OpenComBtn.IsEnabled        = false;
        }

        private void EnableSensorTest()
        {
            SimulationBtn.IsEnabled = true;
            ECoilTestbtn.IsEnabled = true;
            TestIObtn.IsEnabled = true;
            AllTestBtn.IsEnabled = true;
            ClearAllButton.IsEnabled = true;
            OpenComBtn.IsEnabled = true;
            SaveReportBtn.IsEnabled = true;

            AbortConvTestBtn.IsEnabled = false;
            ConverterTestIsEnabled = true;
            SaveInfoBtn.IsEnabled = true;

            TestDryBtn.IsEnabled        = true;
            TestWetBtn.IsEnabled        = true;
            AbortSensTestBtn.IsEnabled  = false;
            SaveInfoBtn.IsEnabled       = true;
        }

        #endregion

        private void IrConnection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IrConnection.IrConnectionStatus == IrCOMPortManager.IrConnectionStates.Ready)
            {
                ConverterState.Fill = new SolidColorBrush(Colors.Yellow);
            }
            if (IrConnection.IrConnectionStatus == IrCOMPortManager.IrConnectionStates.Stop)
            {
                ConverterState.Fill = new SolidColorBrush(Colors.Red);
            }
            if (IrConnection.IrConnectionStatus == IrCOMPortManager.IrConnectionStates.Working)
            {
                ConverterState.Fill = new SolidColorBrush(Colors.LimeGreen);
            }
            if (IrConnection.IrConnectionStatus == IrCOMPortManager.IrConnectionStates.Connected)
            {
                ConverterState.Fill = new SolidColorBrush(Colors.LimeGreen);
            }
        }

        #region Instances

        public DataAccess Verificator
        {
            get
            {
                return DataAccess.Instance;
            }
        }

        public TargetVariablesFields Fields
        {
            get
            {
                return TargetVariablesFields.Instance;
            }
        }

        public SimulatorCOMPortManager SensorSimulator
        {
            get
            {
                return SimulatorCOMPortManager.Instance;
            }
        }

        public MbCOMPortManager MbConnection
        {
            get
            {
                return MbCOMPortManager.Instance;
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

        public Storage FileManager
        {
            get
            {
                return Storage.Instance;
            }
        }

        public SSM1_Com InterfacciaConv
        {
            get
            {
                return SSM1_Com.Instance;
            }
        }

        public Configuration RAM_Configuration
        {
            get
            {
                return Configuration.Instance;
            }
        }

        public GPIO_Device GPIO_Control
        {
            get
            {
                return GPIO_Device.Instance;
            }
        }

        public AnalogsService AnalogMeasures
        {
            get
            {
                return AnalogsService.Instance;
            }
        }

        #endregion

        #region Test Monitor

        private int MonitorTimeout;
        private DispatcherTimer MonitorTimer;
        private void InitMonitorTimer()
        {
            MonitorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            MonitorTimer.Tick += MonitorTimer_Tick;
            MonitorTimer.Start();
            MonitorTimeout = 0;
        }

        private void MonitorTimer_Tick(object sender, object e)
        {
            if (TestRunning == VerificatorTests.None)
            {
                MonitorTimeout = 0;
                AbortPending   = false;
            }
            else if (MonitorTimeout++ >= 10)
            {
                switch(TestRunning)
                {
                    case VerificatorTests.Conv_Simulation:
                        if (PortataMsReadTimer.IsEnabled == false)
                            PortataMsReadTimer.Start();
                        break;
                    case VerificatorTests.Conv_IO:
                        if (IOTestTimer.IsEnabled == false)
                            IOTestTimer.Start();
                        break;
                    case VerificatorTests.Conv_EnergyCoil:
                        if (ECoilTestTimer.IsEnabled == false)
                            ECoilTestTimer.Start();
                        break;
                    case VerificatorTests.Sensor:
                        if (SensorPingTimer.IsEnabled == false)
                            SensorPingTimer.Start();
                        break;
                }
                TimeoutCom = true;
                AbortPending = true;
            }
        }

        #endregion

        #region Connections

        private ICommand _initConnectionsCmd;
        public ICommand InitConnectionsCmd
        {
            get
            {
                if (_initConnectionsCmd == null)
                {
                    _initConnectionsCmd = new RelayCommand(
                        param => OpenSensorSimulatorCom()
                            );
                }
                return _initConnectionsCmd;
            }
        }

        private async void InitConnections()
        {
            LoadingBox.Visibility = Visibility.Visible;
            ConverterState.Fill = new SolidColorBrush(Colors.Yellow);

            //Apro comunicazione con il convertitore
            if (ComSetup.ComPort.ID == null)
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "COM Port Error",
                    Content = "IrCOM Port Not Found",
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            else
            {
                StartIrCOM();
            }
        }

        private async void OpenSensorSimulatorCom()
        {
            //Apro comunicazione con il Simulatore
            if (ComSetup.SimulatorComPort.ID == null)
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "COM Port Error",
                    Content = "SensorSimulator COM Port Not Found",
                    CloseButtonText = "OK",
                };
                await dialog.ShowAsync();
                DisableSensorTest();
            }
            else
            {
                if (await SensorSimulator.Open())
                {
                    SensorSimulator.SendCommand(SensorSimulator.portHandler, SimulatorCOMPortManager.CMD.Ping, 5);
                    SensorSimulator.CommandCompleted += Sensor_CommandCompleted;
                    STestStep = SensorTestStep.Ping;
                    EnableSensorTest();
                }
            }
        }

        #endregion

        #region IrCOM

        private ICommand _refreshDeviceCmd;
        public ICommand RefreshDeviceCmd
        {
            get
            {
                if (_refreshDeviceCmd == null)
                {
                    _refreshDeviceCmd = new RelayCommand(
                        param => RefreshDevice()
                            );
                }
                return _refreshDeviceCmd;
            }
        }

        private void RefreshDevice()
        {
            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            SimulatorState.Fill = new SolidColorBrush(Colors.Yellow);

            if (IrConnection.IsOpen)
                IrConnection.Close();

            if (MbConnection.IsOpen)
                MbConnection.Close();

            ClearSimulationResults();
            ClearIOTestResults();
            ClearECoilTestResults();
            ClearSensorResults();
            ClearLog(true);            

            DisableConverterTest();
            DisableSensorTest();
            AbortSensTestBtn.IsEnabled = false;

            InitConnections();
        }

        private void StartIrCOM()
        {
            if (IrConnection.IsOpen)
            {
                IrConnection.Close();
            }

            OpenIrCOM();
        }

        private void StopIrCOM()
        {
            ReadTimer.Stop();
            if (IrConnection.IsOpen)
            {
                IrConnection.Close();
            }
        }

        private async void OpenIrCOM()
        {
            if (ComSetup.ComPort.ID == null)
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "COM Port Error",
                    Content = "IrCOM Port Not Found",
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            else
            {
                IrConnection.Open(ComSetup.ComPort, IrCOMPortManager.ComMode.Manual);
                ReadTimer.Start();
            }
        }

        private DispatcherTimer ReadTimer;
        private void InitReadTimer()
        {
            ReadTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(2000)
            };
            ReadTimer.Tick += ReadTimer_Tick;
            BundleReaded = false;
            BundleIndex = 1;
            ReLoadingBar.Value = BundleIndex;
            BundleIndexMax = (byte)IrCOMPortManager.ReadingStateBundle.EepCustomizedDeviceInfoCmd;
            ReLoadingBar.Maximum = BundleIndexMax;
            ReadTimer.Stop();
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


        private bool BundleReaded;
        private void BundleAutoRead()
        {
            IrConnection.ReadBundleCompleted += BundleAutoReadCompleted;
            IrConnection.ReadBundle(BundleIndex);
            ReadTimer.Stop();
        }

        private void BundleAutoReadCompleted(object sender, PropertyChangedEventArgs e)
        {
            IrCOMPortManager cmd = sender as IrCOMPortManager;

            if( IrConnection.ReadBundle_CommandResult.Outcome == Euromag.Protocols.CommandResultOutcomes.CommandSuccess )
                BundleReaded = true;
            else
                BundleReaded = false;

            IrConnection.ReadBundleCompleted -= BundleAutoReadCompleted;

            ReadTimer.Start();
        }

        private void ReadTimer_Tick(object sender, object e)
        {
            if (BundleReaded)
            {
                if (BundleIndex < BundleIndexMax)
                {
                    BundleIndex++;
                    BundleAutoRead();
                    BundleReaded = false;
                    ReLoadingBar.Value = BundleIndex;
                }
                else
                {                    
                    ReadTimer.Stop();
                    LoadingBox.Visibility = Visibility.Collapsed;
                    StopIrCOM();
                    OpenSensorSimulatorCOM();
                }
            }
            else
                BundleAutoRead();
        }

        #endregion

        #region Converter

        private bool AllTest = false;
        private bool AbortPending = false;
        private bool TimeoutCom = false;

        private ICommand _startAllTestCmd;
        public ICommand StartAllTestCmd
        {
            get
            {
                if (_startAllTestCmd == null)
                {
                    _startAllTestCmd = new RelayCommand(
                        param => StartAllTest()
                            );
                }
                return _startAllTestCmd;
            }
        }

        private ICommand _clearAllTestCmd;
        public ICommand ClearAllTestCmd
        {
            get
            {
                if (_clearAllTestCmd == null)
                {
                    _clearAllTestCmd = new RelayCommand(
                        param => ClearAllTest()
                            );
                }
                return _clearAllTestCmd;
            }
        }

        private ICommand _abortTestCmd;
        public ICommand AbortTestCmd
        {
            get
            {
                if (_abortTestCmd == null)
                {
                    _abortTestCmd = new RelayCommand(
                        param => AbortTest()
                            );
                }
                return _abortTestCmd;
            }
        }

        private void AbortTest()
        {
            AbortPending = true;
        }

        private void StartAllTest()
        {
            AllTest = true;
            ClearSimulationResults();
            ClearIOTestResults();
            ClearECoilTestResults();
            TestSimulation();
            ReadingProgress.Visibility  = Visibility.Visible;
            ReadingProgress.Value       = 0;
            ReadingProgress.Maximum     = 12;
        }

        private async void ClearAllTest()
        {
            ContentDialog logdialog = new ContentDialog()
            {
                Title = "Clear all tests results and logs",
                Content = "Are you sure?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel",
                IsSecondaryButtonEnabled = false,
                DefaultButton = ContentDialogButton.Primary
            };

            ContentDialogResult logres = await logdialog.ShowAsync();

            if (logres == ContentDialogResult.Primary)
            {
                ClearSimulationResults();
                ClearIOTestResults();
                ClearECoilTestResults();
                ClearSensorResults();
                ClearLog(true);
            }
        }    
    

        #region Test Simulation

        private static SimulationTestStep SimTestStep;
        private static SimulationTestStep SimTestStep_Mem;
        public enum SimulationTestStep
        {
            None,
            Wait,
            Reading,
            GetStatus,
            Sim_0,
            Sim_Low,
            Sim_Hi,
            Sim_Full,
            Sim_Empty,
            Sim_End
        }

        private float Res_Zero_min = 0;
        private float Res_Zero_max = 0;
        private string Reference_Zero;

        private float Res_LO_min = 0;
        private float Res_LO_max = 0;
        private string Reference_LO;

        private float Res_HI_min = 0;
        private float Res_HI_max = 0;
        private string Reference_HI;


        private ICommand _startTestSimulation;
        public ICommand StartTestSimulation
        {
            get
            {
                if (_startTestSimulation == null)
                {
                    _startTestSimulation = new RelayCommand(
                        param => TestSimulation()
                            );
                }
                return _startTestSimulation;
            }
        }

        private void InitTestSimulation()
        {
            InitPortataMsReadTimer();
        }

        private bool OldSimTestFound;
        private void TestSimulation()
        {
            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            if (IrConnection.IsOpen == false)
            { 
                IrConnection.Open(ComSetup.ComPort, IrCOMPortManager.ComMode.Restart);
                SimTestStep = SimulationTestStep.Wait;
            }
            else
                SimTestStep = SimulationTestStep.Reading;

            SimTestStep_Mem = SimulationTestStep.None;

            if (AllTest == false)
            { 
                ReadingProgress.Visibility = Visibility.Visible;
                ReadingProgress.Value   = 0;
                ReadingProgress.Maximum = 5;
            }

            #region Init Limits

            Res_Zero_min = Parametri.Simul.zero_Res_min;
            Res_Zero_max = Parametri.Simul.zero_Res_max;
            Res_LO_min = Parametri.Simul.Low_Res_min;
            Res_LO_max = Parametri.Simul.Low_Res_max;
            Res_HI_min = Parametri.Simul.HI_Res_min;
            Res_HI_max = Parametri.Simul.HI_Res_max;

            OldSimTestFound = false;
            if (OldTestFound)
            {
                // Diverso da zero perchè nel caso di test precedentemente fatto andava in errore test
                if (Verificator.ReportList[OldTestIndex].LO_read != 0)
                {
                    Res_LO_min = (float)Verificator.ReportList[OldTestIndex].LO_read * (1 - Parametri.Simul.Tolleranza_MinPerc / 100);
                    Res_LO_max = (float)Verificator.ReportList[OldTestIndex].LO_read * (1 + Parametri.Simul.Tolleranza_MaxPerc / 100);
                    OldSimTestFound = true;
                }
                // Diverso da zero perchè nel caso di test precedentemente fatto andava in errore test
                if (Verificator.ReportList[OldTestIndex].Hi_read != 0)
                {
                    Res_HI_min = (float)Verificator.ReportList[OldTestIndex].Hi_read * (1 - Parametri.Simul.Tolleranza_MinPerc / 100);
                    Res_HI_max = (float)Verificator.ReportList[OldTestIndex].Hi_read * (1 + Parametri.Simul.Tolleranza_MaxPerc / 100);
                    OldSimTestFound = true;
                }
            }


            Reference_Zero = ">" + Res_Zero_min.ToString("#.00") + "/<" + Res_Zero_max.ToString("#.00");
            Reference_LO = ">" + Res_LO_min.ToString("#.00") + "/<" + Res_LO_max.ToString("#.00");
            Reference_HI = ">" + Res_HI_min.ToString("#.00") + "/<" + Res_HI_max.ToString("#.00");

            #endregion

            ReadTimer.Stop();

            InterfacciaConv.Write(Parametri.Simul.DAC_Zero);
            EPipeStatus.Fill = GPIO_Control.ResEPipe();

            SimulationTestRing.Visibility = Visibility.Visible;
            TestRunning = VerificatorTests.Conv_Simulation;
            ClearSimulationResults();
            Settings.Instance.VerificatorRunning = true;
            DisableConverterTest();
            MonitorTimeout = 0;
            Simulation_Read.Text = "-.-- m/s";

            if (( (Fields.FwVersion.Value * 100) + Fields.FwRevision.Value ) >= 130)
            {
                TestInfo.Text = "Set Verification Mode...";
                SetVerMode = new SetVerificationMode(IrCOMPortManager.Instance.portHandler);
                SetVerMode.Mode = SetVerificationMode.verif_mode.Enabled;
                SetVerMode.CommandCompleted += SetVerMode_CommandCompleted;
                if(SimTestStep == SimulationTestStep.Reading)
                    SetVerMode.send();
                else
                    PortataMsReadTimer.Start();
            }
            else
            {
                SetVerMode = null;
                PortataMsReadTimer.Start();
                TestInfo.Text = "Reading zero...";
            }
        }
        private SetVerificationMode SetVerMode;


        private void SetVerMode_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            SetVerificationMode cmd = sender as SetVerificationMode;
            if (TestRunning == VerificatorTests.Conv_Simulation)
            { 
                if (cmd.Result.Outcome == Euromag.Protocols.CommandResultOutcomes.CommandSuccess)
                {
                    if (SimTestStep == SimulationTestStep.Reading)
                        TestInfo.Text = "Reading zero...";
                }
                else
                {
                    if (SimTestStep == SimulationTestStep.Reading)
                    { 
                        TestInfo.Text = "Set Verification Mode...ERROR";
                        SimTestStep = SimulationTestStep.Sim_End;
                        AbortPending = true;
                    }
                }        
                PortataMsReadTimer.Start();
            }
        }

        public void Portata_m_s_Verif()
        {
            ReadRAM Cmd = new ReadRAM(IrCOMPortManager.Instance.portHandler)
            {
                Variable = Fields.FlowRateMS_Verif
            };
            Cmd.CommandCompleted += Portata_m_s_Verif_CommandCompleted;
            Cmd.send();
        }

        private void Portata_m_s_Verif_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            ReadRAM cmd = sender as ReadRAM;
            if (cmd.Result.Outcome == Euromag.Protocols.CommandResultOutcomes.CommandSuccess)
            {
                bufferLO.Add(Fields.FlowRateMS_Verif.Value);
                SumAverage += Fields.FlowRateMS_Verif.Value;
                Portata_ms_Average = SumAverage / bufferLO.Count;                
            }
            MonitorTimeout = 0;
            PortataMsReadTimer.Start();
        }

        private void GetTargetStatus()
        {
            GetTargetStatus getStatusCmd = new GetTargetStatus(IrConnection.portHandler);
            getStatusCmd.CommandCompleted += GetStatusCmd_CommandCompleted;
            getStatusCmd.send();
        }

        private bool EmptyPipeStatus;
        private void GetStatusCmd_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            GetTargetStatus cmd = sender as GetTargetStatus;
            if (cmd.Result.Outcome == Euromag.Protocols.CommandResultOutcomes.CommandSuccess)
            {
                EmptyPipeStatus = cmd.Status.EmptyPipe;
                SimTestStep = SimTestStep_Mem;
            }
            MonitorTimeout = 0;
        }

        private void SetEmptyPipe(byte value)
        {
            PortataMsReadTimer.Stop();
            Fields.EmptyPipeCfg.Value = value;
            WriteEEPROM Cmd = new WriteEEPROM(IrCOMPortManager.Instance.portHandler){ Variable = Fields.EmptyPipeCfg };
            Cmd.CommandCompleted += Cmd_CommandCompleted;
            Cmd.send();
        }

        private void Cmd_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            WriteEEPROM cmd = sender as WriteEEPROM;
            if (cmd.Result.Outcome == Euromag.Protocols.CommandResultOutcomes.CommandSuccess)
            {
                PortataMsReadTimer.Start();
            }
        }

        private List<double> bufferLO = new List<double>();
        private double SumAverage = 0;
        public double Portata_ms_Average { get; set; }
        private byte EpipeTry = 0;

        private DispatcherTimer PortataMsReadTimer;
        private void InitPortataMsReadTimer()
        {
            PortataMsReadTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            PortataMsReadTimer.Tick += PortataMsReadTimer_Tick;
            PortataMsReadTimer.Stop();
        }

        private void PortataMsReadTimer_Tick(object sender, object e)
        {
            if (AbortPending)
            {
                SimTestStep = SimulationTestStep.Sim_End;
                if(TimeoutCom)
                {
                    AbortPending = false;
                    TimeoutCom = false;
                }
            }            

            switch (SimTestStep)
            {
                case SimulationTestStep.Wait:
                    if (IrConnection.IsOpen)
                    {
                        SimTestStep = SimulationTestStep.Reading;
                        if (SetVerMode != null)
                        {
                            PortataMsReadTimer.Stop();
                            SetVerMode.Mode = SetVerificationMode.verif_mode.Enabled;
                            SetVerMode.send();
                        }                        
                    }
                    break;
                case SimulationTestStep.Reading:
                    if (Portata_ms_Average == 0)
                        Simulation_Read.Text = "-.-- m/s";
                    else
                    {
                        if (Portata_ms_Average < 1)
                            Simulation_Read.Text = "0.00 m/s";
                        else
                            Simulation_Read.Text = Portata_ms_Average.ToString(".##") + " m/s";
                    }

                    if (bufferLO.Count < 5)
                    {
                        Portata_m_s_Verif();
                        PortataMsReadTimer.Stop();
                    }
                    else
                    {
                        switch (SimTestStep_Mem)
                        {
                            case SimulationTestStep.None:
                                SimTestStep = SimulationTestStep.Sim_0;
                                RisultatiTest.Simulazione.Zero_read = Portata_ms_Average;                                
                                break;
                            case SimulationTestStep.Sim_0:
                                SimTestStep = SimulationTestStep.Sim_Low;
                                RisultatiTest.Simulazione.LO_read = Portata_ms_Average;
                                break;
                            case SimulationTestStep.Sim_Low:
                                SimTestStep = SimulationTestStep.Sim_Hi;
                                RisultatiTest.Simulazione.Hi_read = Portata_ms_Average;
                                break;
                            case SimulationTestStep.Sim_Hi:
                                SimTestStep = SimulationTestStep.Sim_Full;                                                               
                                break;
                            case SimulationTestStep.Sim_Full:
                                SimTestStep = SimulationTestStep.Sim_Empty;                                
                                break;
                            case SimulationTestStep.Sim_Empty:
                                SimTestStep = SimulationTestStep.Sim_End;                                
                                break;
                        }
                        ReadingProgress.Value += 1;
                    }
                    break;
                case SimulationTestStep.GetStatus:
                    GetTargetStatus();
                    SimTestStep = SimTestStep_Mem;
                    break;
                case SimulationTestStep.Sim_0:
                    ResetReading();
                    if (RisultatiTest.Simulazione.Zero_read >= Res_Zero_min && RisultatiTest.Simulazione.Zero_read <= Res_Zero_max)
                        RisultatiTest.Simulazione.Zero = RisultatiTest.Esito.PASS_Verify;
                    else
                        RisultatiTest.Simulazione.Zero = RisultatiTest.Esito.FAIL;

                    SimTestStep_Mem = SimTestStep;
                    InterfacciaConv.Write(Parametri.Simul.DAC_Low_406);
                    SimTestStep = SimulationTestStep.Reading;
                    TestInfo.Text = "Reading Mid...";
                    break;
                case SimulationTestStep.Sim_Low:
                    ResetReading();

                    if (RisultatiTest.Simulazione.LO_read >= Res_LO_min && RisultatiTest.Simulazione.LO_read <= Res_LO_max)
                    {
                        if (OldSimTestFound)
                            RisultatiTest.Simulazione.LO = RisultatiTest.Esito.PASS_Verify;
                        else
                            RisultatiTest.Simulazione.LO = RisultatiTest.Esito.PASS_Def;
                    }
                    else
                    {
                        if (RisultatiTest.Simulazione.LO_read >= Parametri.Simul.Low_Res_min && RisultatiTest.Simulazione.LO_read <= Parametri.Simul.Low_Res_max)
                            RisultatiTest.Simulazione.LO = RisultatiTest.Esito.PASS_Def;
                        else
                            RisultatiTest.Simulazione.LO = RisultatiTest.Esito.FAIL;
                    }

                    SimTestStep_Mem = SimTestStep;
                    InterfacciaConv.Write(Parametri.Simul.DAC_Hi_406);
                    SimTestStep = SimulationTestStep.Reading;
                    TestInfo.Text = "Reading Hi...";
                    break;
                case SimulationTestStep.Sim_Hi:
                    ResetReading();

                    if (RisultatiTest.Simulazione.Hi_read >= Res_HI_min && RisultatiTest.Simulazione.Hi_read <= Res_HI_max)
                    {
                        if (OldSimTestFound)
                            RisultatiTest.Simulazione.Hi = RisultatiTest.Esito.PASS_Verify;
                        else
                            RisultatiTest.Simulazione.Hi = RisultatiTest.Esito.PASS_Def;
                    }
                    else
                    {
                        if (RisultatiTest.Simulazione.Hi_read >= Parametri.Simul.HI_Res_min && RisultatiTest.Simulazione.Hi_read <= Parametri.Simul.HI_Res_max)
                            RisultatiTest.Simulazione.Hi = RisultatiTest.Esito.PASS_Def;
                        else                    
                            RisultatiTest.Simulazione.Hi = RisultatiTest.Esito.FAIL;
                    }

                    SimTestStep_Mem     = SimTestStep;
                    InterfacciaConv.Write(Parametri.Simul.DAC_Zero);
                    Simulation_Read.Text = "-.-- m/s";
                    TestInfo.Text       = "Simulation Full Pipe...";
                    SetEmptyPipe(1);
                    EPipeStatus.Fill    = GPIO_Control.SetEPipe();
                    EpipeTry            = 0;
                    SimTestStep         = SimulationTestStep.GetStatus;
                    SimTestStep_Mem     = SimulationTestStep.Sim_Full;                 
                    EmptyPipeStatus     = true;
                    
                    break;
                case SimulationTestStep.Sim_Full:
                    if (EmptyPipeStatus == false)
                    {
                        RisultatiTest.Simulazione.FullPype = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.Simulazione.Full_Read = "Full";

                        SimTestStep         = SimulationTestStep.GetStatus;
                        SimTestStep_Mem     = SimulationTestStep.Sim_Empty;

                        EPipeStatus.Fill    = GPIO_Control.ResEPipe();
                        SetEmptyPipe(1);

                        EpipeTry = 0;
                        TestInfo.Text       = "Simulation Empty Pipe...";
                        EmptyPipeStatus     = false;
                        ReadingProgress.Value += 1;
                    }
                    else if (EpipeTry++ >= 5)
                    {
                        RisultatiTest.Simulazione.FullPype = RisultatiTest.Esito.FAIL;
                        RisultatiTest.Simulazione.Full_Read = "Empty";

                        SimTestStep     = SimulationTestStep.GetStatus;
                        SimTestStep_Mem = SimulationTestStep.Sim_Empty;

                        EPipeStatus.Fill = GPIO_Control.ResEPipe();
                        SetEmptyPipe(1);

                        EpipeTry = 0;
                        TestInfo.Text = "Simulation Empty Pipe...";
                        EmptyPipeStatus = false;
                        ReadingProgress.Value += 1;
                    }
                    break;
                case SimulationTestStep.Sim_Empty:
                    if(EmptyPipeStatus == true)
                    { 
                        RisultatiTest.Simulazione.EmptyPype = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.Simulazione.Empy_Read = "Empty";

                        SetEmptyPipe(0);

                        TestInfo.Text = "...End";
                        SimTestStep = SimulationTestStep.Sim_End;
                        ReadingProgress.Value += 1;
                    }
                    else if(EpipeTry++ >= 5)
                    {
                        RisultatiTest.Simulazione.EmptyPype = RisultatiTest.Esito.FAIL;
                        RisultatiTest.Simulazione.Empy_Read = "Full";

                        SetEmptyPipe(0);

                        TestInfo.Text = "...End";
                        SimTestStep = SimulationTestStep.Sim_End;
                        ReadingProgress.Value += 1;
                    }
                    break;

                case SimulationTestStep.Sim_End:
                    RisultatoSimulationTest();
                    TestInfo.Text = "";
                    if(SetVerMode != null)
                    {
                        SetVerMode.Mode = SetVerificationMode.verif_mode.Disabled;
                        SetVerMode.send();
                    }
                    break;
            }
        }

        private void RisultatoSimulationTest()
        {
            if (AbortPending)
            {
                SimulationResultBorder.Background = new SolidColorBrush(Colors.LightCoral);
                SimulationResult.Text = "ABORTED";
            }
            else
            {
                RisultatiTest.Res_Simulazione();

                TestID = Notifica_Log(TestID,
                                      "Conv.",
                                      RisultatiTest.Simulazione.Test_Zero,
                                      Reference_Zero,
                                      RisultatiTest.Simulazione.Zero_read.ToString("#.00"),
                                      RisultatiTest.Simulazione.Zero);

                TestID = Notifica_Log(TestID,
                                      "Conv.",
                                      RisultatiTest.Simulazione.Test_LO,
                                      Reference_LO,
                                      RisultatiTest.Simulazione.LO_read.ToString("#.00"),
                                      RisultatiTest.Simulazione.LO);

                TestID = Notifica_Log(TestID,
                                      "Conv.",
                                      RisultatiTest.Simulazione.Test_Hi,
                                      Reference_HI,
                                      RisultatiTest.Simulazione.Hi_read.ToString("#.00"),
                                      RisultatiTest.Simulazione.Hi);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.Simulazione.Test_FullPype,
                                      "Full",
                                      RisultatiTest.Simulazione.Full_Read,
                                      RisultatiTest.Simulazione.FullPype);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.Simulazione.Test_EmptyPype,
                                      "Empty",
                                      RisultatiTest.Simulazione.Empy_Read,
                                      RisultatiTest.Simulazione.EmptyPype);

                Refresh_Log();

                ModulaLedsTest();
                SimulationResultBorder.Background = RisultatiTest.Simulazione.EsitoColor;
                SimulationResult.Text = RisultatiTest.Simulazione.EsitoTxt;
            }

            SimulationTestRing.Visibility = Visibility.Collapsed;
            Settings.Instance.VerificatorRunning = false;
            PortataMsReadTimer.Stop();

            if ((AllTest) && (!AbortPending))
                StartECoilTest();
            else
            {
                EnableConverterTest();
                AbortPending = false;
                ReadingProgress.Visibility = Visibility.Collapsed;
            }
        }

        void ClearSimulationResults()
        {
            RisultatiTest.Simulazione.Zero = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Simulazione.LO = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Simulazione.Hi = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Simulazione.EmptyPype = RisultatiTest.Esito.DaEseguire;
            ModulaLedsTest();
            SimulationResultBorder.Background = new SolidColorBrush(Colors.Transparent);
            SimulationResult.Text = "--";

            if (TestRunning != VerificatorTests.Conv_Simulation)
            {
                if (((Fields.FwVersion.Value * 100) + Fields.FwRevision.Value) >= 130)
                {
                    SetVerMode = new SetVerificationMode(IrCOMPortManager.Instance.portHandler);
                    SetVerMode.Mode = SetVerificationMode.verif_mode.Disabled;
                    SetVerMode.CommandCompleted += SetVerMode_CommandCompleted;
                }
            }
            else
                SetVerMode = null;
        }

        private void ResetReading()
        {
            Portata_ms_Average = 0;
            SumAverage = 0;
            bufferLO.Clear();
        }

        #endregion

        #region Test Energy Coil

        private static ECoilTestStep ECoil_TestStep;
        public enum ECoilTestStep
        {
            None,
            Wait,
            ECoil_Zero,
            ECoil_Read,
            ECoil_PosNeg,
            ECoil_End
        }

        private float Icoil_min_DEF = 0;
        private float Icoil_Max_DEF = 0;

        private float Icoil_Min_ref = 0;
        private float Icoil_Max_ref = 0;

        private string Reference_Icoil;

        private ICommand _startECoilTestCmd;
        public ICommand StartECoilTestCmd
        {
            get
            {
                if (_startECoilTestCmd == null)
                {
                    _startECoilTestCmd = new RelayCommand(
                        param => StartECoilTest()
                            );
                }
                return _startECoilTestCmd;
            }
        }

        private void InitECoilTest()
        {
            InitECoilTestTimer();            
        }

        private bool OldECoilTestFound;
        private void StartECoilTest()
        {
            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            if (IrConnection.IsOpen == false)
            {
                IrConnection.Open(ComSetup.ComPort, IrCOMPortManager.ComMode.Restart);
                ECoil_TestStep = ECoilTestStep.Wait;
            }
            else
                ECoil_TestStep = ECoilTestStep.None;

            if (AllTest == false)
            {
                ReadingProgress.Visibility = Visibility.Visible;
                ReadingProgress.Value = 0;
                ReadingProgress.Maximum = 4;
            }

            #region Init Limits
            Icoil_min_DEF = Parametri.ICoil.I_25mA_min;
            Icoil_Max_DEF = Parametri.ICoil.I_25mA_max;
            Icoil_Min_ref = Icoil_min_DEF;
            Icoil_Max_ref = Icoil_Max_DEF;

            if (OldTestFound)
            {
                // Diverso da zero perchè nel caso di test precedentemente fatto andava in errore test
                if (Verificator.ReportList[OldTestIndex].ICoil_Read != 0)
                {
                    Icoil_Min_ref = (float)Verificator.ReportList[OldTestIndex].ICoil_Read * (1 - Parametri.ICoil.Tolleranza_MinPerc / 100);
                    Icoil_Max_ref = (float)Verificator.ReportList[OldTestIndex].ICoil_Read * (1 + Parametri.ICoil.Tolleranza_MaxPerc / 100);
                    OldECoilTestFound = true;
                }
                else
                    OldECoilTestFound = false;
            }

            Reference_Icoil = ">" + Icoil_Min_ref.ToString("#.00") + "/<" + Icoil_Max_ref.ToString("#.00");
            #endregion

            ECoilTestRing.Visibility = Visibility.Visible;
            ClearECoilTestResults();
            ECoilTestTimer.Start();
            TestRunning = VerificatorTests.Conv_EnergyCoil;
            DisableConverterTest();
            MonitorTimeout = 0;
            CountOK_POS_verify = 0;
            CountOK_POS_default = 0;
            TestInfo.Text = "Start Coil Test...";
        }

        private void ClearECoilTestResults()
        {
            RisultatiTest.ICoil.Zero = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.ICoil.PosNeg = RisultatiTest.Esito.DaEseguire;

            ModulaLedsTest();

            ECoilTestResultBorder.Background = new SolidColorBrush(Colors.Transparent);
            ECoilTestResult.Text = "--";
        }

        private DispatcherTimer ECoilTestTimer;
        private void InitECoilTestTimer()
        {
            ECoilTestTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            ECoilTestTimer.Tick += ECoilTestTimer_Tick;
            ECoilTestTimer.Stop();
        }

        public void ReadICoil()
        {
            ReadRAM Cmd = new ReadRAM(IrCOMPortManager.Instance.portHandler)
            {
                Variable = Fields.ICoil_ma
            };
            Cmd.CommandCompleted += ReadICoil_CommandCompleted; 
            Cmd.send();
        }

        private void ReadICoil_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            ReadRAM cmd = sender as ReadRAM;
            if (cmd.Result.Outcome == Euromag.Protocols.CommandResultOutcomes.CommandSuccess)
            {
                RisultatiTest.ICoil.ICoil_Read = Fields.ICoil_ma.Value;

                if(OldECoilTestFound)
                { 
                    if (RisultatiTest.ICoil.ICoil_Read >= Icoil_Min_ref && RisultatiTest.ICoil.ICoil_Read <= Icoil_Max_ref)
                    {
                        CountOK_POS_verify++;
                    }
                    else
                        CountOK_POS_verify = 0;
                }

                if (RisultatiTest.ICoil.ICoil_Read >= Icoil_min_DEF && RisultatiTest.ICoil.ICoil_Read <= Icoil_Max_DEF)
                {
                    CountOK_POS_default++;
                }
                else
                    CountOK_POS_default = 0;                
            }
            ECoilTestTimer.Start();
            MonitorTimeout = 0;
        }

        private int CountOK_POS_default = 0;
        private int CountOK_POS_verify = 0;
        private int Ripetiz_OK = 5;
        private int Ripetizioni;
        private async void ECoilTestTimer_Tick(object sender, object e)
        {
            if (AbortPending)
            {
                ECoil_TestStep = ECoilTestStep.ECoil_End;
                if (TimeoutCom)
                {
                    AbortPending = false;
                    TimeoutCom = false;
                }
            }            

            switch (ECoil_TestStep)
            {
                case ECoilTestStep.Wait:
                    if (IrConnection.IsOpen)
                        ECoil_TestStep = ECoilTestStep.None;
                     break;
                case ECoilTestStep.None:
                    ECoil_TestStep = ECoilTestStep.ECoil_Zero;
                    TestInfo.Text = "Check Zero...";
                    ReadingProgress.Value += 1;
                    break;
                case ECoilTestStep.ECoil_Zero:
                    TestInfo.Text = "Reading Current...";
                    ECoil_TestStep = ECoilTestStep.ECoil_Read;
                    Ripetizioni = 0;
                    RisultatiTest.ICoil.Zero = RisultatiTest.Esito.PASS_Verify;
                    ReadingProgress.Value += 1;
                    break;
                case ECoilTestStep.ECoil_Read:
                    TestInfo.Text = TestInfo.Text + "..";
                    if ((CountOK_POS_verify >= Ripetiz_OK) || (CountOK_POS_default >= Ripetiz_OK))
                    { 
                        ECoil_TestStep = ECoilTestStep.ECoil_PosNeg;
                        ReadingProgress.Value += 1;
                    }

                    if (Ripetizioni++ <= 10)
                    { 
                        ReadICoil();
                        ECoilTestTimer.Stop();
                    }
                    else
                    {                       
                        ContentDialog dialog = new ContentDialog()
                        {
                            Title = "Communication Error",
                            Content = "No responce from Device",
                            CloseButtonText = "OK",
                        };
                        await dialog.ShowAsync();
                        ECoil_TestStep = ECoilTestStep.ECoil_End;
                    }
                    break;
                case ECoilTestStep.ECoil_PosNeg:
                    ReadingProgress.Value += 1;
                    TestInfo.Text = "Check Values...";
                    if (CountOK_POS_verify >= Ripetiz_OK)
                        RisultatiTest.ICoil.PosNeg = RisultatiTest.Esito.PASS_Verify;
                    else if (CountOK_POS_default >= Ripetiz_OK)
                        RisultatiTest.ICoil.PosNeg = RisultatiTest.Esito.PASS_Def;
                    else
                        RisultatiTest.ICoil.PosNeg = RisultatiTest.Esito.FAIL;
                    ECoil_TestStep = ECoilTestStep.ECoil_End;
                    break;
                case ECoilTestStep.ECoil_End:                    
                    TestInfo.Text = "";
                    RisultatiTestEcoil();
                    break;
            }
        }

        private void RisultatiTestEcoil()
        {
            if (AbortPending)
            {
                ECoilTestResultBorder.Background = new SolidColorBrush(Colors.LightCoral);
                ECoilTestResult.Text = "ABORTED";
            }
            else
            {
                RisultatiTest.Res_ICoil();


                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.ICoil.Test_Zero,
                                      "0",
                                      "0",
                                      RisultatiTest.ICoil.Zero);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.ICoil.Test_PosNeg,
                                      Reference_Icoil,
                                      RisultatiTest.ICoil.ICoil_Read.ToString("#.00"),
                                      RisultatiTest.ICoil.PosNeg);

                Refresh_Log();

                ModulaLedsTest();
                ECoilTestResultBorder.Background = RisultatiTest.ICoil.EsitoColor;
                ECoilTestResult.Text = RisultatiTest.ICoil.EsitoTxt;
            }

            ECoilTestTimer.Stop();
            ECoilTestRing.Visibility = Visibility.Collapsed;

            if ((AllTest) && (!AbortPending))
                StartIOTest();
            else
            {
                EnableConverterTest();
                AbortPending = false;
                ReadingProgress.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region TEST I/O

        private static IOTestStep IO_TestStep;
        public enum IOTestStep
        {
            None,
            Wait,
            IO_AllOff,
            IO_Pos_Imp,
            IO_Neg_Imp,
            IO_End
        }

        private ICommand _startIOTestCmd;
        public ICommand StartIOTestCmd
        {
            get
            {
                if (_startIOTestCmd == null)
                {
                    _startIOTestCmd = new RelayCommand(
                        param => StartIOTest()
                            );
                }
                return _startIOTestCmd;
            }
        }

        private void StartIOTest()
        {
            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            if (IrConnection.IsOpen == false)
            {
                IrConnection.Open(ComSetup.ComPort, IrCOMPortManager.ComMode.Restart);
                IO_TestStep = IOTestStep.Wait;
            }
            else
                IO_TestStep = IOTestStep.None;

            if (AllTest == false)
            {
                ReadingProgress.Visibility = Visibility.Visible;
                ReadingProgress.Value = 0;
                ReadingProgress.Maximum = 4;
            }

            IO_TestStep = IOTestStep.None;
            IOTestTestRing.Visibility = Visibility.Visible;
            ClearIOTestResults();
            IOTestTimer.Start();
            VAuxStatus.Fill = GPIO_Control.SetVAux();
            TestRunning = VerificatorTests.Conv_IO;
            DisableConverterTest();
            MonitorTimeout = 0;
        }

        private void ClearIOTestResults()
        {
            RisultatiTest.IO.PulseNegOFF = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.IO.PulseNegON = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.IO.PulsePosOFF = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.IO.PulsePosON = RisultatiTest.Esito.DaEseguire;

            ModulaLedsTest();

            TestIOResultBorder.Background = new SolidColorBrush(Colors.Transparent);
            TestIOResult.Text = "--";
        }

        private DispatcherTimer IOTestTimer;
        private void InitIOTestTimer()
        {
            IOTestTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            IOTestTimer.Tick += IOTestTimer_Tick; ;
            IOTestTimer.Stop();
        }

        private void IOTestTimer_Tick(object sender, object e)
        {
            if (AbortPending)
            {
                IO_TestStep = IOTestStep.IO_End;
                if (TimeoutCom)
                {
                    AbortPending = false;
                    TimeoutCom = false;
                }
            }

            MonitorTimeout = 0;  //T.B. Spostare!!!

            switch (IO_TestStep)
            {
                case IOTestStep.Wait:
                    if (IrConnection.IsOpen)
                        IO_TestStep = IOTestStep.None;
                    break;
                case IOTestStep.None:
                    TestInfo.Text = "Set All Off...";
                    IO_TestStep = IOTestStep.IO_AllOff;
                    SetPulseOut(false, false);                    
                    IOTestTimer.Stop();
                    ReadingProgress.Value += 1;
                    break;
                case IOTestStep.IO_AllOff:
                    TestInfo.Text = "Set Pulse Pos...";
                    IO_TestStep = IOTestStep.IO_Pos_Imp;
                    SetPulseOut(true, false);                    
                    IOTestTimer.Stop();
                    ReadingProgress.Value += 1;
                    break;
                case IOTestStep.IO_Pos_Imp:
                    TestInfo.Text = "Set Pulse Neg...";
                    IO_TestStep = IOTestStep.IO_Neg_Imp;
                    SetPulseOut(false, true);                    
                    IOTestTimer.Stop();
                    ReadingProgress.Value += 1;
                    break;
                case IOTestStep.IO_Neg_Imp:
                    TestInfo.Text = "...End";
                    IO_TestStep = IOTestStep.IO_End;
                    SetPulseOut(false, false);
                    ReadingProgress.Value += 1;
                    break;
                case IOTestStep.IO_End:
                    TestInfo.Text = "";
                    RisultatiTestIO();                    
                    VAuxStatus.Fill = GPIO_Control.ResVAux();
                    break;
            }
        }

        public void SetPulseOut(bool PulsePosOn, bool PulseNegOn)
        {
            TargetOutputs OutputStates = new TargetOutputs
            {
                PositivePulse = PulsePosOn,
                NegativePulse = PulseNegOn,
            };

            SetTargetOutputs SetOutputs = new SetTargetOutputs(IrCOMPortManager.Instance.portHandler)
            {
                Outputs = OutputStates                
            };

            SetOutputs.CommandCompleted += SetOutputs_CommandCompleted;
            SetOutputs.send();
        }

        private void SetOutputs_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            SetTargetOutputs cmd = sender as SetTargetOutputs;
            if (cmd.Result.Outcome == Euromag.Protocols.CommandResultOutcomes.CommandSuccess)
            {
                switch (IO_TestStep)
                {
                    case IOTestStep.IO_AllOff:
                        if (GPIO_Control.GetPulseNegOut().value == false)
                        {
                            RisultatiTest.IO.PulseNegOFF = RisultatiTest.Esito.PASS_Verify;
                            RisultatiTest.IO.Test_PulseNegOFF.Reading = "Off";
                        }
                        else
                        {
                            RisultatiTest.IO.PulseNegOFF = RisultatiTest.Esito.FAIL;
                            RisultatiTest.IO.Test_PulseNegOFF.Reading = "On";
                        }

                        if (GPIO_Control.GetPulsePosOut().value == false)
                        {
                            RisultatiTest.IO.PulsePosOFF = RisultatiTest.Esito.PASS_Verify;
                            RisultatiTest.IO.Test_PulsePosOFF.Reading = "Off";
                        }
                        else
                        {
                            RisultatiTest.IO.PulsePosOFF = RisultatiTest.Esito.FAIL;
                            RisultatiTest.IO.Test_PulsePosOFF.Reading = "On";
                        }
                        break;
                    case IOTestStep.IO_Pos_Imp:
                        if (GPIO_Control.GetPulsePosOut().value == true)
                        { 
                            RisultatiTest.IO.PulsePosON = RisultatiTest.Esito.PASS_Verify;
                            RisultatiTest.IO.Test_PulsePosON.Reading = "On";
                        }
                        else
                        { 
                            RisultatiTest.IO.PulsePosON = RisultatiTest.Esito.FAIL;
                            RisultatiTest.IO.Test_PulsePosON.Reading = "Off";
                        }
                        break;
                    case IOTestStep.IO_Neg_Imp:
                        if (GPIO_Control.GetPulseNegOut().value == true)
                        {
                            RisultatiTest.IO.PulseNegON = RisultatiTest.Esito.PASS_Verify;
                            RisultatiTest.IO.Test_PulseNegON.Reading = "On";
                        }
                        else
                        {
                            RisultatiTest.IO.PulseNegON = RisultatiTest.Esito.FAIL;
                            RisultatiTest.IO.Test_PulseNegON.Reading = "Off";
                        }
                        break;
                    default:
                        break;
                }
            }
            IOTestTimer.Start();
        }

        private void RisultatiTestIO()
        {
            if (AbortPending)
            {
                TestIOResultBorder.Background = new SolidColorBrush(Colors.LightCoral);
                TestIOResult.Text = "ABORTED";
            }
            else
            {
                RisultatiTest.Res_IO();


                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_PulseNegOFF,
                                      "Off",
                                      RisultatiTest.IO.Test_PulseNegOFF.Reading,
                                      RisultatiTest.IO.PulseNegOFF);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_PulsePosOFF,
                                      "Off",
                                      RisultatiTest.IO.Test_PulsePosOFF.Reading,
                                      RisultatiTest.IO.PulsePosOFF);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_PulseNegON,
                                      "On",
                                      RisultatiTest.IO.Test_PulseNegON.Reading,
                                      RisultatiTest.IO.PulseNegON);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_PulsePosON,
                                      "On",
                                      RisultatiTest.IO.Test_PulsePosON.Reading,
                                      RisultatiTest.IO.PulsePosON);

                Refresh_Log();

                ModulaLedsTest();
                TestIOResultBorder.Background = RisultatiTest.IO.EsitoColor;
                TestIOResult.Text = RisultatiTest.IO.EsitoTxt;
            }

            IOTestTimer.Stop();
            IOTestTestRing.Visibility = Visibility.Collapsed;
            ReadingProgress.Visibility = Visibility.Collapsed;

            AllTest = false;
            AbortPending = false;
            EnableConverterTest();
        }


        #endregion

        #endregion

        #region Sensor

        public enum SensorTestStep
        {
            Ping = 0,
            Error = 1,
            Start = 2,
            Wait = 3,
            GetResults = 4,
            End = 5
        }
        private SensorTestStep STestStep;

        private ICommand _openSimulatorCOMcmd;
        public ICommand OpenSimulatorCOMcmd
        {
            get
            {
                if (_openSimulatorCOMcmd == null)
                {
                    _openSimulatorCOMcmd = new RelayCommand(
                        param => OpenSensorSimulatorCOM()
                            );
                }
                return _openSimulatorCOMcmd;
            }
        }

        private async void OpenSensorSimulatorCOM()
        {
            if (ComSetup.SimulatorComPort.ID == null)
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "COM Port Error",
                    Content = "SensorSimulator COM Port Not Found",
                    CloseButtonText = "OK",
                };
                await dialog.ShowAsync();
            }
            else
            {
                if (await SensorSimulator.Open())
                {
                    SensorSimulator.SendCommand(SensorSimulator.portHandler, SimulatorCOMPortManager.CMD.Ping, 5);
                    SensorSimulator.CommandCompleted += Sensor_CommandCompleted;
                    STestStep = SensorTestStep.Ping;
                }
            }
        }

        private ICommand _startTestSensorDry;
        public ICommand StartTestSensorDry
        {
            get
            {
                if (_startTestSensorDry == null)
                {
                    _startTestSensorDry = new RelayCommand(
                        param => TestSensorDry()
                            );
                }
                return _startTestSensorDry;
            }
        }

        private async void TestSensorDry()
        {
            if (IrConnection.IsOpen)
                IrConnection.Close();

            if (SensorSimulator.IsOpen == false)
                await SensorSimulator.Open();

            ClearSensorResults();
            RisultatiTest.Sensore.dry = true;
            SensorTestRing.Visibility = Visibility.Visible;
            SensorSimulator.SendCommand(SensorSimulator.portHandler, SimulatorCOMPortManager.CMD.DryTest, 5);
            SensorSimulator.CommandCompleted += Sensor_CommandCompleted;
            STestStep = SensorTestStep.Start;
            DisableSensorTest();
            TestRunning = VerificatorTests.Sensor;
            MonitorTimeout = 0;
        }

        private ICommand _startTestSensorWet;
        public ICommand StartTestSensorWet
        {
            get
            {
                if (_startTestSensorWet == null)
                {
                    _startTestSensorWet = new RelayCommand(
                        param => TestSensorWet()
                            );
                }
                return _startTestSensorWet;
            }
        }

        private async void TestSensorWet()
        {
            if (IrConnection.IsOpen)
                IrConnection.Close();

            if (SensorSimulator.IsOpen == false)
                await SensorSimulator.Open();

            ClearSensorResults();
            RisultatiTest.Sensore.dry = false;
            SensorTestRing.Visibility = Visibility.Visible;
            SensorSimulator.SendCommand(SensorSimulator.portHandler, SimulatorCOMPortManager.CMD.DryTest, 5);
            SensorSimulator.CommandCompleted += Sensor_CommandCompleted;
            STestStep = SensorTestStep.Start;
            DisableSensorTest();
            TestRunning = VerificatorTests.Sensor;
        }

        private void StartDryTest()
        {
            SensorSimulator.SendCommand(SensorSimulator.portHandler, SimulatorCOMPortManager.CMD.DryTest, 5);
            SensorSimulator.CommandCompleted += Sensor_CommandCompleted;
        }

        private void SensorWait()
        {
            SensorSimulator.SendCommand(SensorSimulator.portHandler, SimulatorCOMPortManager.CMD.Ping, 5);
            SensorSimulator.CommandCompleted += Sensor_CommandCompleted;
        }

        private void SensorGetResults()
        {
            SensorSimulator.SendCommand(SensorSimulator.portHandler, SimulatorCOMPortManager.CMD.ReadResults, 5);
            SensorSimulator.CommandCompleted += Sensor_CommandCompleted;
        }

        private void Sensor_CommandCompleted(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SimulatorCOMPortManager CMD = sender as SimulatorCOMPortManager;

            SensorSimulator.CommandCompleted -= Sensor_CommandCompleted;

            if (CMD.CommandResult.Result == SimulatorCOMPortManager.SimCommandResult.Success)
            {
                if (CMD.CommandResult.Answer == SimulatorCOMPortManager.ANSWER.Test_Error)
                {
                    STestStep = SensorTestStep.Error;
                    SimulatorState.Fill = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    switch (STestStep)
                    {
                        case SensorTestStep.Ping:
                            EnableSensorTest();
                            CheckDatabaseTest(true);
                            SimulatorState.Fill = new SolidColorBrush(Colors.LimeGreen);
                            break;
                        case SensorTestStep.Error:
                            STestStep = SensorTestStep.Error;
                            break;
                        case SensorTestStep.Start:
                            STestStep = SensorTestStep.Wait;
                            break;
                        case SensorTestStep.Wait:
                            if (CMD.CommandResult.Answer == SimulatorCOMPortManager.ANSWER.StandBy)
                                STestStep = SensorTestStep.GetResults;
                            if (CMD.CommandResult.Answer == SimulatorCOMPortManager.ANSWER.Test_In_progress)
                                STestStep = SensorTestStep.Wait;
                            break;
                        case SensorTestStep.GetResults:
                            SensorTestResultData = CMD.CommandResult.DataResults;
                            STestStep = SensorTestStep.End;
                            break;
                    }
                }

                MonitorTimeout = 0;
                SensorPingTimer.Start();
            }                
        }

        private void SensorPingTimer_Tick(object sender, object e)
        {            

            switch (STestStep)
            {
                case SensorTestStep.Ping:
                    break;
                case SensorTestStep.Error:
                    SimulatorError();
                    SensorPingTimer.Stop();
                    break;
                case SensorTestStep.Start:
                    StartDryTest();
                    SensorPingTimer.Stop();
                    break;
                case SensorTestStep.Wait:
                    SensorWait();
                    SensorPingTimer.Stop();
                    break;
                case SensorTestStep.GetResults:
                    SensorGetResults();
                    SensorPingTimer.Stop();
                    break;
                case SensorTestStep.End:
                    RisultatoSensorTest();
                    SensorPingTimer.Stop();
                    STestStep = SensorTestStep.Ping;
                    break;
            }
        }

        private void RisultatoSensorTest()
        {
            RisultatiTest.Sensore.R_Coil = BitConverter.ToSingle(SensorTestResultData, 0);

            if (RisultatiTest.Sensore.R_Coil > 1000)
                ResistanceValue.Text = ">1KOhm";
            else
                ResistanceValue.Text = RtoKohm(RisultatiTest.Sensore.R_Coil);

            float RL_ABmin_ref = Parametri.TestSensore.RL_ABmin;
            float RL_ABmax_ref = Parametri.TestSensore.RL_ABmax;

            // Aggiungere parsing Res Coil
            if (RisultatiTest.Sensore.R_Coil >= RL_ABmin_ref && RisultatiTest.Sensore.R_Coil <= RL_ABmax_ref)
            {
                RisultatiTest.Sensore.RL_AB = RisultatiTest.Esito.PASS_Verify;
                ResistanceVauleBorder.Background = new SolidColorBrush(Colors.LimeGreen);
            }
            else if (RisultatiTest.Sensore.R_Coil >= Parametri.TestSensore.RL_ABmin && RisultatiTest.Sensore.R_Coil <= Parametri.TestSensore.RL_ABmax)
            {
                RisultatiTest.Sensore.RL_AB = RisultatiTest.Esito.PASS_Def;
                ResistanceVauleBorder.Background = new SolidColorBrush(Colors.YellowGreen);
            }
            else
            {
                RisultatiTest.Sensore.RL_AB = RisultatiTest.Esito.FAIL;
                ResistanceVauleBorder.Background = new SolidColorBrush(Colors.Red);
            }

            TestID = Notifica_Log(TestID,
                                 "Sens.",
                                 RisultatiTest.Sensore.Test_R_Coil,
                                 ">" + RtoKohm(Parametri.TestSensore.RL_ABmin) + "/<" + RtoKohm(Parametri.TestSensore.RL_ABmax),
                                 RtoKohm(RisultatiTest.Sensore.R_Coil),
                                 RisultatiTest.Sensore.RL_AB
                               );

            ///////////////////////////////////////////////////////

            RisultatiTest.Sensore.R_AC = BitConverter.ToSingle(SensorTestResultData, 16);

            if (RisultatiTest.Sensore.R_AC > Parametri.TestSensore.RH_Min)
                RisultatiTest.Sensore.RH_AC = RisultatiTest.Esito.PASS_Verify;
            else
                RisultatiTest.Sensore.RH_AC = RisultatiTest.Esito.FAIL;

            TestID = Notifica_Log(  TestID,
                                     "Sens.",
                                     RisultatiTest.Sensore.Test_RH_AC,
                                     ">" + RtoKohm(Parametri.TestSensore.RH_Min),
                                     RtoKohm(RisultatiTest.Sensore.R_AC),
                                     RisultatiTest.Sensore.RH_AC
                                   );

            /////////////////////////////////////////////////////////////
            if (RisultatiTest.Sensore.dry)
            {
                RisultatiTest.Sensore.R_DC = BitConverter.ToSingle(SensorTestResultData, 4);

                if (RisultatiTest.Sensore.R_DC > Parametri.TestSensore.RL_Min)
                    RisultatiTest.Sensore.RL_DC = RisultatiTest.Esito.PASS_Verify;
                else
                    RisultatiTest.Sensore.RL_DC = RisultatiTest.Esito.FAIL;

                TestID = Notifica_Log(  TestID,
                                        "Sens.",
                                        RisultatiTest.Sensore.Test_RL_DC,
                                        ">" + RtoKohm(Parametri.TestSensore.RL_Min),
                                        RtoKohm(RisultatiTest.Sensore.R_DC),
                                        RisultatiTest.Sensore.RL_DC
                                      );

                /////////////////////////////////////////////////////////

                RisultatiTest.Sensore.R_EC = BitConverter.ToSingle(SensorTestResultData, 8);

                if (RisultatiTest.Sensore.R_EC > Parametri.TestSensore.RL_Min)
                    RisultatiTest.Sensore.RL_EC = RisultatiTest.Esito.PASS_Verify;
                else
                    RisultatiTest.Sensore.RL_EC = RisultatiTest.Esito.FAIL;

                TestID = Notifica_Log(  TestID,
                                        "Sens.",
                                        RisultatiTest.Sensore.Test_RL_EC,
                                        ">" + RtoKohm(Parametri.TestSensore.RL_Min),
                                        RtoKohm(RisultatiTest.Sensore.R_EC),
                                        RisultatiTest.Sensore.RL_EC
                                      );

                /////////////////////////////////////////////////////////
                RisultatiTest.Sensore.R_TC = BitConverter.ToSingle(SensorTestResultData, 12);

                if (RisultatiTest.Sensore.R_TC > Parametri.TestSensore.RL_Min)
                    RisultatiTest.Sensore.RL_TC = RisultatiTest.Esito.PASS_Verify;
                else
                    RisultatiTest.Sensore.RL_TC = RisultatiTest.Esito.FAIL;

                TestID = Notifica_Log(  TestID,
                                        "Sens.",
                                        RisultatiTest.Sensore.Test_RL_TC,
                                        ">" + RtoKohm(Parametri.TestSensore.RL_Min),
                                        RtoKohm(RisultatiTest.Sensore.R_TC),
                                        RisultatiTest.Sensore.RL_TC
                                      );

                /////////////////////////////////////////////////////////

                RisultatiTest.Sensore.R_DC = BitConverter.ToSingle(SensorTestResultData, 20);

                if (RisultatiTest.Sensore.R_DC > Parametri.TestSensore.RH_Min)
                    RisultatiTest.Sensore.RH_DC = RisultatiTest.Esito.PASS_Verify;
                else
                    RisultatiTest.Sensore.RH_DC = RisultatiTest.Esito.FAIL;

                TestID = Notifica_Log(  TestID,
                                        "Sens.",
                                        RisultatiTest.Sensore.Test_RH_DC,
                                        ">" + RtoKohm(Parametri.TestSensore.RH_Min),
                                        RtoKohm(RisultatiTest.Sensore.R_DC),
                                        RisultatiTest.Sensore.RH_DC
                                      );

                //////////////////////////////////////////////////////////
                RisultatiTest.Sensore.R_EC = BitConverter.ToSingle(SensorTestResultData, 24);

                if (RisultatiTest.Sensore.R_EC > Parametri.TestSensore.RH_Min)
                    RisultatiTest.Sensore.RH_EC = RisultatiTest.Esito.PASS_Verify;
                else
                    RisultatiTest.Sensore.RH_EC = RisultatiTest.Esito.FAIL;

                TestID = Notifica_Log(  TestID,
                                        "Sens.",
                                        RisultatiTest.Sensore.Test_RH_EC,
                                        ">" + RtoKohm(Parametri.TestSensore.RH_Min),
                                        RtoKohm(RisultatiTest.Sensore.R_EC),
                                        RisultatiTest.Sensore.RH_EC
                                      );

                //////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////

                RisultatiTest.Sensore.R_TC = BitConverter.ToSingle(SensorTestResultData, 28);

                if (RisultatiTest.Sensore.R_TC > Parametri.TestSensore.RH_Min)
                    RisultatiTest.Sensore.RH_TC = RisultatiTest.Esito.PASS_Verify;
                else
                    RisultatiTest.Sensore.RH_TC = RisultatiTest.Esito.FAIL;

                TestID = Notifica_Log(  TestID,
                                        "Sens.",
                                        RisultatiTest.Sensore.Test_RH_TC,
                                        ">" + RtoKohm(Parametri.TestSensore.RH_Min),
                                        RtoKohm(RisultatiTest.Sensore.R_TC),
                                        RisultatiTest.Sensore.RH_TC
                                      );

                //////////////////////////////////////////////////////////
            }            

            ModulaLedsTest();

            if (RisultatiTest.Sensore.dry) // Determinare risultato in base al test dry/wet
            {
                if (RisultatiTest.Res_Sensor_Dry())
                {
                    TestSensorResultBorder.Background = new SolidColorBrush(Colors.LimeGreen);
                    TestSensorResult.Text = "PASS";
                }
                else
                {
                    TestSensorResultBorder.Background = new SolidColorBrush(Colors.Red);
                    TestSensorResult.Text = "FAIL";
                }
            }
            else
            {
                if (RisultatiTest.Res_Sensor_Wet())
                {
                    TestSensorResultBorder.Background = new SolidColorBrush(Colors.LimeGreen);
                    TestSensorResult.Text = "PASS";
                }
                else
                {
                    TestSensorResultBorder.Background = new SolidColorBrush(Colors.Red);
                    TestSensorResult.Text = "FAIL";
                }
            }

            EnableSensorTest();
            SensorTestRing.Visibility = Visibility.Collapsed;
        }

        string RtoKohm(float value )
        {
            string Unit;

            Unit = "Ohm";

            if (value >= 1000000)
            {
                float Tmp = value / 1000000;
                string TmpStr = Tmp.ToString() + " M" + Unit;
                return TmpStr;
            }
            else if (value >= 1000)
            {
                float Tmp = value / 1000;
                string TmpStr = Tmp.ToString() + " K" + Unit;
                return TmpStr;
            }
            else
                return (value.ToString("#.0 ") + Unit);
        }

        private void ClearSensorResults()
        {
            RisultatiTest.Sensore.R_Coil = 0f;
            RisultatiTest.Sensore.H_Coil = 0f;
            RisultatiTest.Sensore.RL_AB = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Sensore.RL_DC = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Sensore.RL_TC = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Sensore.RL_EC = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Sensore.RH_AC = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Sensore.RH_DC = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Sensore.RH_TC = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Sensore.RH_EC = RisultatiTest.Esito.DaEseguire;

            RisultatiTest.Sensore.R_AC = 0;
            RisultatiTest.Sensore.R_DC = 0;
            RisultatiTest.Sensore.R_TC = 0;
            RisultatiTest.Sensore.R_EC = 0;

            ModulaLedsTest();

            ResistanceValue.Text = "--";
            ResistanceVauleBorder.Background = new SolidColorBrush(Colors.Transparent);
            TestSensorResultBorder.Background = new SolidColorBrush(Colors.Transparent);
            TestSensorResult.Text = "--";
        }

        private void SimulatorError()
        {
            RisultatiTest.TestStep ErrorTest = new RisultatiTest.TestStep
            {
                ID = 99,
                Description = "Simulator Error",
                Reference = "Abort",
                Result = "Error"
            };
            Verificator.TestList.Add(ErrorTest);
            Verificator.TestView.Clear();
            Verificator.TestList.ForEach(p => Verificator.TestView.Add(p));
            SensorTestRing.Visibility = Visibility.Collapsed;
            TestSensorResultBorder.Background = new SolidColorBrush(Colors.Red);
            TestSensorResult.Text = "ABORTED";
        }

        private DispatcherTimer SensorPingTimer;
        private void InitSensorPingTimer()
        {
            SensorPingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(2000)
            };
            SensorPingTimer.Tick += SensorPingTimer_Tick;
            SensorPingTimer.Stop();
        }

        private byte[] SensorTestResultData = new byte[32];

        #endregion

        #region ADC

        private DispatcherTimer RefreshAnalogsTimer;
        private void RefreshAnalogsStart()
        {
            RefreshAnalogsTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
            RefreshAnalogsTimer.Tick += RefreshAnalogsTimer_Tick;
            RefreshAnalogsTimer.Start();
        }

        private void RefreshAnalogsTimer_Tick(object sender, object e)
        {
            ValimInVoltBlock.Text = AnalogMeasures.VAlimInVolt().ToString("#.00") + " V";
        }

        #endregion

        #region Common Resources

        ParametriVerificator Parametri = new ParametriVerificator(); // Soglie e tolleranze test
        private static int TestID = 1;

        private int Notifica_Log(int ID, string device, RisultatiTest.TestStep Test, string reference, string reading, RisultatiTest.Esito result )
        {
            Test.Device     = device;
            Test.Reference  = reference;
            Test.ID         = ID++;
            Test.Reading    = reading;
            switch(result)
            {
                case (RisultatiTest.Esito.DaEseguire):
                    Test.Result = "N.E.";
                    break;
                case (RisultatiTest.Esito.FAIL):
                    Test.Result = "FAIL";
                    break;
                case (RisultatiTest.Esito.PASS_Def):
                    Test.Result = "PASS";
                    break;
                case (RisultatiTest.Esito.PASS_Verify):
                    Test.Result = "PASS";
                    break;
            }            
            Verificator.TestList.Add(Test);
            TestRunning = VerificatorTests.None;
            Refresh_Log();
            return ID;
        }

        private void Refresh_Log()
        {
            Verificator.TestView.Clear();
            Verificator.TestList.ForEach(p => Verificator.TestView.Add(p));            
        }

        Brush LedTest( RisultatiTest.Esito res )
        {
            if (res == RisultatiTest.Esito.PASS_Verify)
            {
                return new SolidColorBrush(Colors.LimeGreen);
            }
            else if (res == RisultatiTest.Esito.PASS_Def)
            {
                return new SolidColorBrush(Colors.YellowGreen);
            }
            else if (res == RisultatiTest.Esito.FAIL)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else if (res == RisultatiTest.Esito.DaEseguire)
            {
                return new SolidColorBrush(Colors.Gray);
            }
            else
                return new SolidColorBrush(Colors.Transparent);
        }

        void ModulaLedsTest()
        {
            ZeroEnergyCheck.Fill= LedTest(RisultatiTest.ICoil.Zero);
            PosNegCheck.Fill    = LedTest(RisultatiTest.ICoil.PosNeg);

            ZeroCheck.Fill      = LedTest(RisultatiTest.Simulazione.Zero);
            MidCheck.Fill       = LedTest(RisultatiTest.Simulazione.LO);
            HiCheck.Fill        = LedTest(RisultatiTest.Simulazione.Hi);
            EPipeCheck.Fill     = LedTest(RisultatiTest.Simulazione.EmptyPype);

            PulsePosCheck.Fill = LedTest(RisultatiTest.IO.PulsePosON);
            PulseNegCheck.Fill = LedTest(RisultatiTest.IO.PulseNegON);

            IsolationACCheck.Fill = LedTest(RisultatiTest.Sensore.RH_AC);
            IsolationDCCheck.Fill = LedTest(RisultatiTest.Sensore.RH_DC);
            IsolationTCCheck.Fill = LedTest(RisultatiTest.Sensore.RH_TC);
            IsolationECCheck.Fill = LedTest(RisultatiTest.Sensore.RH_EC);
        }

        #endregion

        #region Log File
        
        private ICommand _clearLogCmd;
        public ICommand ClearLogCmd
        {
            get
            {
                if (_clearLogCmd == null)
                {
                    _clearLogCmd = new RelayCommand(
                        param => ClearLog(false)
                            );
                }
                return _clearLogCmd;
            }
        }

        private async void ClearLog(bool auto)
        {
            if(auto)
            {
                Verificator.TestView.Clear();
                Verificator.TestList.Clear();
                ReportTestGrid.ItemsSource = null;
                ReportTestGrid.ItemsSource = Verificator.TestView;
            }
            else
            {
                ContentDialog logdialog = new ContentDialog()
                {
                    Title = "Clear Logs",
                    Content = "Are you sure?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "Cancel",
                    IsSecondaryButtonEnabled = false,
                    DefaultButton = ContentDialogButton.Primary
                };

                ContentDialogResult logres = await logdialog.ShowAsync();

                if (logres == ContentDialogResult.Primary)
                {
                    Verificator.TestView.Clear();
                    Verificator.TestList.Clear();
                    ReportTestGrid.ItemsSource = null;
                    ReportTestGrid.ItemsSource = Verificator.TestView;
                }
            }
        }

        private ICommand _saveLogCmd;
        public ICommand SaveLogCmd
        {
            get
            {
                if (_saveLogCmd == null)
                {
                    _saveLogCmd = new RelayCommand(
                        param => SaveLog()
                            );
                }
                return _saveLogCmd;
            }
        }

        private async void SaveLog()
        {
            if(Verificator.TestList.Count != 0)
            {
                string LogLineStr;
                char[] LogLineArray;
                string FileName = "TestLog_" + Fields.ConverterId.ValAsString + "_" + Fields.SensorId.ValAsString + "_" + DateTime.Now.ToString("MM_dd_yyyy_hh_mm") + ".log";

                LogLineStr = Fields.ConverterId.ValAsString + ";" + Fields.SensorId.ValAsString + ";" + DateTime.Now.ToString("yyyy/MM/dd hh:mm") +  "\n";
                LogLineArray = LogLineStr.ToCharArray();
                await BinaryStorage.Append(FileName, FileManager.CurrentFolder.Path, LogLineArray);

                for (int i=0; i< Verificator.TestList.Count; i++)
                {
                    LogLineStr =    Verificator.TestList[i].ID.ToString() + ";" + 
                                    Verificator.TestList[i].Device + ";" +
                                    Verificator.TestList[i].Description + ";" +
                                    Verificator.TestList[i].Reference + ";" +
                                    Verificator.TestList[i].Reading + ";" +
                                    Verificator.TestList[i].Result + "\n";

                    LogLineArray = LogLineStr.ToCharArray();

                    await BinaryStorage.Append(FileName, FileManager.CurrentFolder.Path, LogLineArray);
                }

                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Log File",
                    Content = "Log File saved:\n" + FileName,
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }            
        }

        #endregion

        #region Report

        public VerificatorConfig RAM_VerifConfiguration
        {
            get
            {
                return VerificatorConfig.Instance;
            }
            set {; }
        }

        private ICommand _addReportCmd;
        public ICommand AddReportCmd
        {
            get
            {
                if (_addReportCmd == null)
                {
                    _addReportCmd = new RelayCommand(
                        param => AddReport()
                            );
                }
                return _addReportCmd;
            }
        }

        private async void AddReport()
        {
            ReportLine NewReport = new ReportLine();
            ReportLine LastReport = new ReportLine();

            Verificator.ReportList = DataAccess.GetData("MC_Suite_DataBase.db");
            if (Verificator.ReportList.Count != 0)
            {
                LastReport = Verificator.ReportList.Last<ReportLine>();
                NewReport.ID = LastReport.ID + 1;
            }
            else
                NewReport.ID = 1;

            NewReport.Data_Test = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            NewReport.OperatoreTest = OperatorName.Text;

            NewReport.Modello_Sensore = Fields.SensorModel.Value;
            NewReport.Matricola_Sensore = Fields.SensorId.Value;
            NewReport.Modello_Convertitore = Fields.DeviceName.Value;
            NewReport.Matricola_Convertitore = Fields.ConverterId.Value;
            NewReport.KA = Fields.KaRatio.ValAsString;
            NewReport.FondoScala = FlowRateFullScale_Converter() + " m3/h";
            NewReport.Impulsi = Fields.PulseOutputVolume.ValAsString + " " + PulseUnitToString( Fields.PulseOutputTechUnit.ValAsString );

            //Convertitore****************************************************
            NewReport.AnalogOut = "--";
            NewReport.Simulation = VerifyTest(RisultatiTest.Simulazione.Esito);
            if((RisultatiTest.Simulazione.Esito == RisultatiTest.Esito.PASS_Verify) ||
               (RisultatiTest.Simulazione.Esito == RisultatiTest.Esito.PASS_Def))
            { 
                NewReport.Zero_read = RisultatiTest.Simulazione.Zero_read;
                NewReport.Hi_read   = RisultatiTest.Simulazione.Hi_read;
                NewReport.LO_read   = RisultatiTest.Simulazione.LO_read;
            }
            else
            {
                NewReport.Zero_read = 0;
                NewReport.Hi_read = 0;
                NewReport.LO_read = 0;
            }

            NewReport.EmptyPype = VerifyTest(RisultatiTest.Simulazione.EmptyPype);

            NewReport.EnergyCoil = VerifyTest(RisultatiTest.ICoil.Esito);
            if((RisultatiTest.ICoil.Esito == RisultatiTest.Esito.PASS_Verify) ||
               (RisultatiTest.ICoil.Esito == RisultatiTest.Esito.PASS_Def))
            {
                NewReport.ICoil_Read = RisultatiTest.ICoil.ICoil_Read;
            }
            else
            {
                NewReport.ICoil_Read = 0;
            }

            NewReport.IO = VerifyTest(RisultatiTest.IO.Esito);
            NewReport.TempPCB = "20°C"; //Todo
            //****************************************************************

            //Sensore*********************************************************
            NewReport.CoilResistance = RtoKohm(RisultatiTest.Sensore.R_Coil);
            NewReport.IsolationAC = VerifyTest(RisultatiTest.Sensore.RH_AC);

            if(RisultatiTest.Sensore.RH_AC == RisultatiTest.Esito.DaEseguire)
            {
                NewReport.IsolationTC   = "N.E.";
                NewReport.IsolationDC   = "N.E.";
                NewReport.IsolationEC   = "N.E.";
                NewReport.TestType      = "--";
            }
            else if(RisultatiTest.Sensore.dry)
            { 
                NewReport.IsolationTC   = VerifyTest(RisultatiTest.Sensore.RH_TC);
                NewReport.IsolationDC   = VerifyTest(RisultatiTest.Sensore.RH_DC);
                NewReport.IsolationEC   = VerifyTest(RisultatiTest.Sensore.RH_EC);
                NewReport.TestType      = "DRY";
            }
            else
            {
                NewReport.IsolationTC = "N.E.";
                NewReport.IsolationDC = "N.E.";
                NewReport.IsolationEC = "N.E.";
                NewReport.TestType    = "WET";
            }
            //****************************************************************

            NewReport.Company               = CompanyName.Text;
            NewReport.CompanyLocation       = CompanyLocation.Text;
            NewReport.Customer              = CustomerName.Text;
            NewReport.CustomerLocation      = CustomerLocation.Text;
            NewReport.Note                  = Note.Text; 
            NewReport.SN_Verificator        = RAM_VerifConfiguration.SN_Verificator;
            NewReport.SW_Ver_Verificator    = RAM_VerifConfiguration.SW_Ver_Verificator;
            NewReport.DataCalibrazione      = RAM_VerifConfiguration.DataLastTaratura;
            NewReport.NuovaCalibrazione     = RAM_VerifConfiguration.DataNextTaratura;

            Verificator.ReportList.Add(NewReport);

            DataAccess.AddData("MC_Suite_DataBase.db", NewReport);

            Verificator.ReportView.Clear();
            Verificator.ReportList.ForEach(p => Verificator.ReportView.Add(p));

            ContentDialog dialog = new ContentDialog()
            {
                Title = "Report Saved",
                Content = "Report #" + NewReport.ID.ToString() + " added to Database\n" +
                          "Sensor ID: " + NewReport.Matricola_Sensore + "\n" +
                          "Converter ID : " + NewReport.Matricola_Convertitore,
                CloseButtonText = "OK",
            };
            await dialog.ShowAsync();

            CheckDatabaseTest(false);
        }

        public double kUT;
        public double kTB;
        public double LocFlowFullScale, FullScaleFactor;
        public const double PGRECO = 3.141592653589793;

        string FlowRateFullScale_Converter()
        {
            kUT = 0.001; // "cubic meter [m³]"
            kTB = 0.36;  // "hour [h]"
            FullScaleFactor = 2.5 * PGRECO * Fields.SensorDiameter.Value * Fields.SensorDiameter.Value * kUT * kTB;
            LocFlowFullScale = (Fields.FlowrateFullscale.Value / 10) * FullScaleFactor;
            return LocFlowFullScale.ToString("#.##");
        }

        string PulseUnitToString(string val)
        {
            if (String.IsNullOrEmpty(val))
                return null;

            Regex pattern = new Regex(@"(?<=\[)(.*?)(?=\])");
            Match match = pattern.Match(val);

            return match.Groups[1].Value;
        }

        string VerifyTest(RisultatiTest.Esito res)
        {
            if (res == RisultatiTest.Esito.PASS_Verify)
            {
                return "PASS";
            }
            else if (res == RisultatiTest.Esito.PASS_Def)
            {
                return "PASS (Default)";
            }
            else if (res == RisultatiTest.Esito.FAIL)
            {
                return "FAIL";
            }
            else if (res == RisultatiTest.Esito.DaEseguire)
            {
                return "N.E.";
            }
            else
                return "--";
        }    

        #endregion

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

        private async void SaveConfig()
        {
            //Salvo la Configurazione
            RAM_Configuration.Operator = OperatorName.Text;
            RAM_Configuration.Company = CompanyName.Text;
            RAM_Configuration.Company_Location = CompanyLocation.Text;
            RAM_Configuration.Customer = CustomerName.Text;
            RAM_Configuration.Customer_Location = CustomerLocation.Text;
            RAM_Configuration.Note = Note.Text;

            List<Configuration> NewCfg = new List<Configuration>();
            NewCfg.Add(RAM_Configuration);
            if (await SerializableStorage<Configuration>.Save(FileManager.ConfigFile, FileManager.MainFolder.Path, NewCfg))
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Report Info Save",
                    Content = "Report Info Saved",
                    CloseButtonText = "OK",
                };

                await dialog.ShowAsync();
            }
            else
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Report Info Save",
                    Content = "Report Info Saving Error",
                    CloseButtonText = "OK",
                };

                await dialog.ShowAsync();
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

        private void EPipeManual_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn == true)
                {
                    EPipeStatus.Fill = GPIO_Control.SetEPipe();
                }
                else
                {
                    EPipeStatus.Fill = GPIO_Control.ResEPipe();
                }
            }
        }

        private void VAuxManual_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn == true)
                {
                    VAuxStatus.Fill = GPIO_Control.SetVAux();
                }
                else
                {
                    VAuxStatus.Fill = GPIO_Control.ResVAux();
                }
            }
        }

        private void RL420mAManual_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn == true)
                {
                    RL420mAStatus.Fill = GPIO_Control.SetRL_4_20mA();
                }
                else
                {
                    RL420mAStatus.Fill = GPIO_Control.ResRL_4_20mA();
                }
            }
        }

        private void GPInManual_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn == true)
                {
                    GPInStatus.Fill = GPIO_Control.SetGPIn();
                }
                else
                {
                    GPInStatus.Fill = GPIO_Control.ResGPIn();
                }
            }
        }
    }
}
