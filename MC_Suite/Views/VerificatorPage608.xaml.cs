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
using MC_Suite.Modbus;

using System.Security.Cryptography;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class VerificatorPage608 : Page
    {
        public VerificatorPage608()
        {
            this.InitializeComponent();

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
            InitTest420mATimer();
            InitMonitorTimer();

            GPioState.Fill = GPIO_Control.InitGPIO();

            DisableConverterTest();
            DisableSensorTest();
            AbortSensTestBtn.IsEnabled = false;

            if (ComSetup.Verificator608PageReady == false)
            {
                RisultatiTest.Init_Sensore();
                RisultatiTest.Init_IO();
                RisultatiTest.Init_ICoil();
                RisultatiTest.Init_AnalogOut();
                RisultatiTest.Init_Simulazione();
                RisultatiTest.Init_AnalogOut();

                InterfacciaConv.Write(Parametri.Simul.DAC_Zero);
                EPipeStatus.Fill = GPIO_Control.SetEPipe();

                ComSetup.Verificator608PageReady = true;
            }

            if (MbConnection.MbConnectionStatus == MbCOMPortManager.MbConnectionStates.Working)
            {
                ConverterState.Fill = new SolidColorBrush(Colors.LimeGreen);
                AggiornaInfo();
                EnableConverterTest();
                ConncetionRing.Visibility = Visibility.Collapsed;
            }
            else
                InitMbConnections();

            Simulation_Read.Text = "-.-- m/s";

            InitAnalogMeasures();
        }

        private async void InitAnalogMeasures()
        {
            if (AnalogMeasures.ADC_ModuleIsReady)
            {
                VerificatorState.Fill = new SolidColorBrush(Colors.LimeGreen);
                AnalogMeasures.ADC_MeasuresStart();
                RefreshAnalogsStart();
            }
            else
            {
                var dialog = new MessageDialog("ADC Module Not Ready", "ADC Module Error");
                await dialog.ShowAsync();
            }
        }

        private static int OldTestIndex;
        private static bool OldTestFound;

        private async void CheckDatabaseTest(bool IsStartup)
        {
            OldTestIndex = 0;
            OldTestFound = false;

            for (int i = 0; i < Verificator.ReportList.Count; i++)
            {
                if (Verificator.ReportList[i].Matricola_Convertitore.Equals(MC608.Convertitore.Matricola))
                //if (Verificator.ReportList[i].Matricola_Sensore.Equals(MC608.Sensore.Matricola))
                {
                    OldTestIndex = i;
                    OldTestFound = true;
                }
            }

            if (OldTestFound)
                LastTestInfo.Text = "Last saved test on " + Verificator.ReportList[OldTestIndex].Data_Test;

            if (IsStartup)
            {
                if (OldTestFound)
                {
                    var dialog = new MessageDialog("Converter " + MC608.Convertitore.Matricola + " Found\n" +
                                                   "Last test on " + Verificator.ReportList[OldTestIndex].Data_Test, "Old Report");
                    await dialog.ShowAsync();
                }
                else
                {
                    var dialog = new MessageDialog("Converter " + MC608.Convertitore.Matricola + " Not Found\n" +
                                                   "No test data, default parameters will be used", "Old Report");
                    await dialog.ShowAsync();
                }
            }

            ConverterState.Fill = new SolidColorBrush(Colors.LimeGreen);
        }

        private enum VerificatorTests
        {
            None,
            Conv_420mA,
            Conv_Simulation,
            Conv_EnergyCoil,
            Conv_IO,
            Sensor
        }

        VerificatorTests TestRunning;

        #region Abilitazioni


        private void DisableConverterTest()
        {
            Test420mABtn.IsEnabled = false;
            SimulationBtn.IsEnabled = false;
            ECoilTestbtn.IsEnabled = false;
            TestIObtn.IsEnabled = false;
            AllTestBtn.IsEnabled = false;
            AbortConvTestBtn.IsEnabled = true;
            TestDryBtn.IsEnabled = false;
            TestWetBtn.IsEnabled = false;
            AbortSensTestBtn.IsEnabled = false;
        }

        private void EnableConverterTest()
        {
            if(AnalogMeasures.VAlimLow == false)
                Test420mABtn.IsEnabled = true;
            SimulationBtn.IsEnabled = true;
            ECoilTestbtn.IsEnabled = true;
            TestIObtn.IsEnabled = true;
            AllTestBtn.IsEnabled = true;
            AbortConvTestBtn.IsEnabled = false;
            if (SensorSimulator.IsReady)
            {
                TestDryBtn.IsEnabled = true;
                TestWetBtn.IsEnabled = true;
                AbortSensTestBtn.IsEnabled = false;
            }
        }

        private void DisableSensorTest()
        {
            Test420mABtn.IsEnabled = false;
            SimulationBtn.IsEnabled = false;
            ECoilTestbtn.IsEnabled = false;
            TestIObtn.IsEnabled = false;
            AllTestBtn.IsEnabled = false;
            AbortConvTestBtn.IsEnabled = false;

            TestDryBtn.IsEnabled = false;
            TestWetBtn.IsEnabled = false;
            AbortSensTestBtn.IsEnabled = true;
        }

        private void EnableSensorTest()
        {
            if (AnalogMeasures.VAlimLow == false)
                Test420mABtn.IsEnabled = true;
            SimulationBtn.IsEnabled = true;
            ECoilTestbtn.IsEnabled = true;
            TestIObtn.IsEnabled = true;
            AllTestBtn.IsEnabled = true;
            AbortConvTestBtn.IsEnabled = false;

            TestDryBtn.IsEnabled = true;
            TestWetBtn.IsEnabled = true;
            AbortSensTestBtn.IsEnabled = false;
        }

        #endregion

        #region Instances

        public DataAccess Verificator
        {
            get
            {
                return DataAccess.Instance;
            }
        }

        public SimulatorCOMPortManager SensorSimulator
        {
            get
            {
                return SimulatorCOMPortManager.Instance;
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

        public CustomDictionary Customer
        {
            get
            {
                return CustomDictionary.Instance;
            }
        }

        #endregion

        private int RefreshAnalogsTimeout;
        private DispatcherTimer MonitorTimer;
        private void InitMonitorTimer()
        {
            MonitorTimer = new DispatcherTimer();
            MonitorTimer.Interval = TimeSpan.FromMilliseconds(1000);
            MonitorTimer.Tick += MonitorTimer_Tick;
            MonitorTimer.Start();
            RefreshAnalogsTimeout = 0;
        }
        
        private void MonitorTimer_Tick(object sender, object e)
        {
            if (TestRunning == VerificatorTests.None)
            {
                if (RefreshAnalogsTimeout++ >= 30)
                { 
                    AnalogMeasures.ADC_Restart();
                    RefreshAnalogsTimeout = 0;
                }
            }
        }

        private ICommand _initConnectionsCmd;
        public ICommand InitConnectionsCmd
        {
            get
            {
                if (_initConnectionsCmd == null)
                {
                    _initConnectionsCmd = new RelayCommand(
                        param => InitMbConnections()
                            );
                }
                return _initConnectionsCmd;
            }
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

                if (await MbConnection.Open(ComSetup.ComPort608))
                {
                    MC608.Reset();
                    MC608.Tipo_Connessione = MC608.Comunicazione.Connesso_ModBus;
                    ConverterState.Fill = new SolidColorBrush(Colors.Yellow);
                    InitModbusReader();
                }
            }
        }

        private void ReadRegister()
        {
            if (MbConnection.IsOpen)
            {
                MbConnection.ReadRegisters(ComSetup.Address, Map.Registri_CMD3_16.Release_ModBus, Map.Registri_CMD3_16.ResetTotN, 3);                
                MbConnection.ReadRegistersCompleted += MbConnection_ReadRegistersCompleted;
            }
        }

        private void ReadBatteryMode()
        {
            if (MbConnection.IsOpen)
            {
                MbConnection.ReadRegisters(Settings.Instance.Address, Map.Registri_CMD4.GetPowerMode, Map.Registri_CMD4.GetPowerMode, 3);
                ReadBatteryModeReady = true;
                MbConnection.ReadRegistersCompleted += MbConnection_ReadRegistersCompleted;
            }
        }

        private void AggiornaInfo()
        {
            DeviceName.Text = Customer.ConverterModel(MC608.Convertitore.Modello);
            ConverterId.Text = MC608.Convertitore.Matricola;
            SensorModel.Text    = Customer.SensorModel(MC608.Sensore.Modello);            
            SensorId.Text = MC608.Sensore.Matricola;
            FwRelease.Text = MC608.Release_FW.Versione.ToString() + "." + MC608.Release_FW.Revisione.ToString();

            OpenSensorSimulatorCom();
        }

        private void CancellaInfo()
        {
            DeviceName.Text = "";
            ConverterId.Text = "";
            SensorModel.Text = "";
            SensorId.Text = "";
            FwRelease.Text = "";
        }

        private void ReadTaratura()
        {
            if (MbConnection.IsOpen)
            {
                MbConnection.ReadRegisters(ComSetup.Address, Map.Registri_CMD3_16.DAT_CALIB, Map.Registri_CMD3_16.Inserzione, 3);
                ReadTaraReady = true;
                MbConnection.ReadRegistersCompleted += MbConnection_ReadRegistersCompleted;
            }
        }

        private enum ModbusReadStates
        {
            Idle,
            ReadParams,
            ReadTara,
            ReadBatteryMode,
            ReadMeas,
            SetEmptyPipe,
            ReadEmptyPipe,
            ReadDigitalIO
        }
        ModbusReadStates ModbusReadState;

        private DispatcherTimer ModbusReader;
        private void InitModbusReader()
        {
            ModbusReader = new DispatcherTimer();
            ModbusReader.Interval = TimeSpan.FromMilliseconds(1000);
            ModbusReader.Tick += ModbusReader_Tick; ;
            ModbusReadState = ModbusReadStates.ReadParams;
            ModbusReader.Start();
        }

        private ICommand _refreshDevice608Cmd;
        public ICommand RefreshDevice608Cmd
        {
            get
            {
                if (_refreshDevice608Cmd == null)
                {
                    _refreshDevice608Cmd = new RelayCommand(
                        param => RefreshDevice608()
                            );
                }
                return _refreshDevice608Cmd;
            }
        }

        void RefreshDevice608()
        {
            //Chiudo connessioni precedenti
            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            SimulatorState.Fill = new SolidColorBrush(Colors.Yellow);

            if (IrConnection.IsOpen)
                IrConnection.Close();

            if (MbConnection.IsOpen)
                MbConnection.Close();

            DisableConverterTest();
            DisableSensorTest();
            AbortSensTestBtn.IsEnabled = false;
            CancellaInfo();

            ClearTest420mAResults();
            ClearSimulationResults();
            ClearIOTestResults();
            ClearECoilTestResults();
            ClearSensorResults();
            ClearLog(true);

            ConncetionRing.Visibility = Visibility.Visible;

            //Riapro connessione
            InitMbConnections();
        }

        private void ModbusReader_Tick(object sender, object e)
        {
            switch(ModbusReadState)
            {
                case ModbusReadStates.Idle:
                    break;
                case ModbusReadStates.ReadParams:
                    ReadRegister();
                    ModbusReader.Stop();
                    break;
                case ModbusReadStates.ReadTara:
                    ReadTaratura();
                    ModbusReader.Stop();
                    break;
                case ModbusReadStates.ReadBatteryMode:
                    ReadBatteryMode();
                    ModbusReader.Stop();
                    break;
            }            
        }

        private void InitMbConnections()
        {
            //Apro comunicazione con il convertitore
            StartModbus();
        }

        #region Converter

        private bool AllTest = false;
        private bool AbortPending = false;

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
            Test420mATimer.Stop();
            PortataMsReadTimer.Stop();
            IOTestTimer.Stop();
            ECoilTestTimer.Stop();
            SensorPingTimer.Stop();

            TestRunning = VerificatorTests.None;

            AbortPending    = true;
            AllTest         = false;

            ClearTest420mAResults();
            ClearSimulationResults();
            ClearIOTestResults();
            ClearECoilTestResults();
            ClearSensorResults();

            TestInfo.Text = "";
            EnableConverterTest();
        }

        private void StartAllTest()
        {
            AllTest = true;
            ClearTest420mAResults();
            ClearSimulationResults();
            ClearIOTestResults();
            ClearECoilTestResults();
            if (Test420mABtn.IsEnabled)
                Test420mA();
            else
                TestSimulation();
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

        private async void ClearAllTest()
        {
            var dialog = new MessageDialog("Are you sure?", "Clear all tests results and logs");
            dialog.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });
            var res = await dialog.ShowAsync();

            if ((int)res.Id == 0)
            {
                ClearTest420mAResults();
                ClearSimulationResults();
                ClearIOTestResults();
                ClearECoilTestResults();
                ClearSensorResults();
                ClearLog(true);
            }
        }

        private void AbortAllTests()
        {
            if(AbortPending == false)
            { 
                switch (TestRunning)
                {
                    case VerificatorTests.Conv_420mA:
                        if (Test420mATimer.IsEnabled == false)
                            Test420mATimer.Start();
                        break;
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
                AbortPending = true;
                AllTest = false;
            }
        }


        #region Test 4-20mA

        private static Test420mAStep Test420mA_Step;
        public enum Test420mAStep
        {
            ResetTest   = 0,
            TestOffset  = 1,
            Test4mA     = 2,
            Test12mA    = 3,
            Test20mA    = 4,
            TestResult  = 5,
            TestEnd     = 6
        }

        private ICommand _test420mACmd;
        public ICommand Test420mACmd
        {
            get
            {
                if (_test420mACmd == null)
                {
                    _test420mACmd = new RelayCommand(
                        param => Test420mA()
                            );
                }
                return _test420mACmd;
            }
        }

        private async void Test420mA()
        {
            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            if(MbConnection.IsOpen == false)
                await MbConnection.Open(ComSetup.ComPort608);

            TestInfo.Text = "Start Test 4/20 mA...";
            Test420mA_Step      = Test420mAStep.ResetTest;
            
            ClearTest420mAResults();
            Test420mATimer.Start();
            DisableConverterTest();

            AbortPending = false;

            if (MC608.Release_HW.Versione == 6 && MC608.Release_HW.Revisione >= 3)
            {                
                VAuxStatus.Fill = GPIO_Control.SetVAux();
                RL420mAStatus.Fill = GPIO_Control.ResRL_4_20mA();
            }
            else
            {
                VAuxStatus.Fill = GPIO_Control.ResVAux();
                RL420mAStatus.Fill = GPIO_Control.SetRL_4_20mA();
            }

            TestRunning = VerificatorTests.Conv_420mA;           

            AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].TestOffset = 0;

            ReadingProgress.Minimum = (double)Test420mAStep.ResetTest;
            ReadingProgress.Maximum = (double)Test420mAStep.Test20mA;
            ReadingProgress.Value = (double)Test420mA_Step;

            Test420mARing.Visibility = Visibility.Visible;
        }

        private void ClearTest420mAResults()
        {
            RisultatiTest.AnOut.I_OffSet = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.AnOut.I_4mA = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.AnOut.I_12mA = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.AnOut.I_20mA = RisultatiTest.Esito.DaEseguire;

            ModulaLedsTest();

            Test420mAResultBorder.Background = new SolidColorBrush(Colors.Transparent);
            Test420mAResult.Text = "--";
            Test420mARing.Visibility = Visibility.Collapsed;
        }

        private DispatcherTimer Test420mATimer;

        private void InitTest420mATimer()
        {
            Test420mATimer = new DispatcherTimer();
            Test420mATimer.Interval = TimeSpan.FromMilliseconds(500);
            Test420mATimer.Tick += Test420mATimer_Tick;
            Test420mATimer.Stop();
        }

        private byte Test420mRetry;
        private byte Test420mAStabilizationTimer;
        private bool NextStepReady;
        private async void Test420mATimer_Tick(object sender, object e)
        {
            if (AbortPending)
            {
                Test420mA_Step = Test420mAStep.TestEnd;
            }

            NextStepReady = true;
            ReadingProgress.Value = (double)Test420mA_Step;

            switch (Test420mA_Step)
            {
                case Test420mAStep.ResetTest:
                    TestInfo.Text = "Reset...";
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 0, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    Test420mATimer.Stop();
                    Test420mRetry = 0;
                    break;
                case Test420mAStep.TestOffset:
                    TestInfo.Text = "Set Offset...";
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.ENABLE_SIMUL, 0, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    Test420mATimer.Stop();
                    break;
                case Test420mAStep.Test4mA:
                    //Impostazione Offset e lettura 4 mA
                    if(Test420mAStabilizationTimer-- > 0)
                    { 
                        TestInfo.Text = "Test 4 mA...";
                        RisultatiTest.AnOut.I_OffSet_read = 4.0f - AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].Misura;

                        AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].TestOffset = RisultatiTest.AnOut.I_OffSet_read;

                        if (AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].Misura > 10f)
                        {
                            RisultatiTest.AnOut.I_OffSet = RisultatiTest.Esito.FAIL;
                            Test420mA_Step = Test420mAStep.TestEnd;
                            var dialog = new MessageDialog("Current Loop Power error:\n\rTry to connect power supply and charge Battery", "Current Loop Error");
                            await dialog.ShowAsync();
                        }

                        if (RisultatiTest.AnOut.I_OffSet_read >= Parametri.Para4_20mA.Offset_min
                            && RisultatiTest.AnOut.I_OffSet_read <= Parametri.Para4_20mA.Offset_max)
                        {
                            RisultatiTest.AnOut.I_OffSet = RisultatiTest.Esito.PASS_Verify;
                            Test420mA_Step = Test420mAStep.Test12mA;
                            Test420mRetry = 0;
                        }
                        else
                        {
                            if(Test420mRetry++ >= 6)
                            {
                                RisultatiTest.AnOut.I_OffSet = RisultatiTest.Esito.FAIL;
                                Test420mA_Step = Test420mAStep.TestEnd;
                            }
                        }
                    }
                    else
                        TestInfo.Text = "...";
                    break;
                case Test420mAStep.Test12mA:
                    //Lettura 12 mA
                    if (Test420mAStabilizationTimer-- > 0)
                    {
                        RisultatiTest.AnOut.I_4mA_read = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].Misura;

                        if (RisultatiTest.AnOut.I_4mA_read >= Parametri.Para4_20mA.I4mA_min &&
                            RisultatiTest.AnOut.I_4mA_read <= Parametri.Para4_20mA.I4mA_max)
                        {
                            RisultatiTest.AnOut.I_4mA = RisultatiTest.Esito.PASS_Verify;
                            TestInfo.Text = "Test 12 mA...";
                            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.ENABLE_SIMUL, 50, 3);
                            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                            Test420mATimer.Stop();
                            Test420mRetry = 0;
                        }
                        else
                        {
                            if (Test420mRetry++ >= 10)
                            {
                                RisultatiTest.AnOut.I_4mA = RisultatiTest.Esito.FAIL;
                                TestInfo.Text = "Test 12 mA...";
                                MbConnection.SendCommand(ComSetup.Address, Map.Comandi.ENABLE_SIMUL, 50, 3);
                                MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                                Test420mATimer.Stop();
                                Test420mRetry = 0;
                            }
                            OffsetUpdate();
                        }
                    }
                    else
                        TestInfo.Text = "...";
                    break;
                case Test420mAStep.Test20mA:
                    if (Test420mAStabilizationTimer-- > 0)
                    {
                        RisultatiTest.AnOut.I_12mA_read = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].Misura2;

                        if (RisultatiTest.AnOut.I_12mA_read >= Parametri.Para4_20mA.I12mA_min &&
                            RisultatiTest.AnOut.I_12mA_read <= Parametri.Para4_20mA.I12mA_max)
                        {
                            RisultatiTest.AnOut.I_12mA = RisultatiTest.Esito.PASS_Verify;
                            TestInfo.Text = "Test 20 mA...";
                            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.ENABLE_SIMUL, 100, 3);
                            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                            Test420mATimer.Stop();
                            Test420mRetry = 0;
                        }
                        else
                        {
                            if (Test420mRetry++ >= 10)
                            {
                                RisultatiTest.AnOut.I_12mA = RisultatiTest.Esito.FAIL;
                                TestInfo.Text = "Test 20 mA...";
                                MbConnection.SendCommand(ComSetup.Address, Map.Comandi.ENABLE_SIMUL, 100, 3);
                                MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                                Test420mATimer.Stop();
                                Test420mRetry = 0;
                            }
                            OffsetUpdate();
                        }
                    }
                    else
                        TestInfo.Text = "...";
                    break;
                case Test420mAStep.TestResult:
                    if (Test420mAStabilizationTimer-- > 0)
                    {
                        RisultatiTest.AnOut.I_20mA_read = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].Misura;
                        if (RisultatiTest.AnOut.I_20mA_read >= Parametri.Para4_20mA.I20mA_min &&
                            RisultatiTest.AnOut.I_20mA_read <= Parametri.Para4_20mA.I20mA_max)
                        {
                            RisultatiTest.AnOut.I_20mA = RisultatiTest.Esito.PASS_Verify;
                            TestInfo.Text = "Test end...";
                            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.DISABLE_SIMUL, 0, 3);
                            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                            ReadingProgress.Value = 0;
                            Test420mATimer.Stop();
                            Test420mRetry = 0;
                        }
                        else
                        {
                            if (Test420mRetry++ >= 10)
                            {
                                RisultatiTest.AnOut.I_20mA = RisultatiTest.Esito.FAIL;
                                TestInfo.Text = "Test end...";
                                MbConnection.SendCommand(ComSetup.Address, Map.Comandi.DISABLE_SIMUL, 0, 3);
                                MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                                ReadingProgress.Value = 0;
                                Test420mATimer.Stop();
                            }
                            OffsetUpdate();
                        }
                    }
                    else
                        TestInfo.Text = "...";
                    break;
                case Test420mAStep.TestEnd:
                    TestInfo.Text = "";

                    VAuxStatus.Fill = GPIO_Control.ResVAux();
                    RL420mAStatus.Fill = GPIO_Control.ResRL_4_20mA();

                    RisultatiTest420mA();
                    Test420mATimer.Stop();
                    break;
            }
        }
        

        private void OffsetUpdate()
        {
            //Incremento offset per misura 4-20mA *******************************************************************************
            if (RisultatiTest.AnOut.I_OffSet_read >= Parametri.Para4_20mA.Offset_min
                && RisultatiTest.AnOut.I_OffSet_read <= Parametri.Para4_20mA.Offset_max)
            {
                if (RisultatiTest.AnOut.I_20mA_read < Parametri.Para4_20mA.I20mA_min)
                {
                    AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].TestOffset += 0.05f;
                    if (AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].TestOffset >= Parametri.Para4_20mA.Offset_max)
                        AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].TestOffset = (float)Parametri.Para4_20mA.Offset_max;
                }
                if (RisultatiTest.AnOut.I_20mA_read > Parametri.Para4_20mA.I20mA_max)
                {
                    AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].TestOffset -= 0.05f;
                    if (AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].TestOffset <= Parametri.Para4_20mA.Offset_min)
                        AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].TestOffset = (float)Parametri.Para4_20mA.Offset_min;
                }
                RisultatiTest.AnOut.I_OffSet_read = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].TestOffset;
            }
            //*********************************************************************************************************************
        }

        private void RisultatiTest420mA()
        {
            if (AbortPending)
            {
                Test420mAResultBorder.Background = new SolidColorBrush(Colors.LightCoral);
                Test420mAResult.Text = "ABORTED";
            }
            else
            {
                RisultatiTest.Res_AnalogOut();

                float IOffSetread = RisultatiTest.AnOut.I_OffSet_read;// / 100;
                TestID = Notifica_Log(TestID,
                                       "Conv.",
                                       RisultatiTest.AnOut.Test_I_OffSet,
                                       Parametri.Para4_20mA.Offset_limit,
                                       IOffSetread.ToString("#.0"),
                                       RisultatiTest.AnOut.I_OffSet);

                float I4mAread = RisultatiTest.AnOut.I_4mA_read;// / 100;
                TestID = Notifica_Log(TestID,
                                       "Conv.",
                                       RisultatiTest.AnOut.Test_I_4mA,
                                       Parametri.Para4_20mA.I4mA_limit,
                                       I4mAread.ToString("#.0"),
                                       RisultatiTest.AnOut.I_4mA);

                float I12mAread = RisultatiTest.AnOut.I_12mA_read;// / 100;
                TestID = Notifica_Log(TestID,
                                       "Conv.",
                                       RisultatiTest.AnOut.Test_I_12mA,
                                       Parametri.Para4_20mA.I12mA_limit,
                                       I12mAread.ToString("#.0"),
                                       RisultatiTest.AnOut.I_12mA);

                float I20mAread = RisultatiTest.AnOut.I_20mA_read;// / 100;
                TestID = Notifica_Log(TestID,
                                       "Conv.",
                                       RisultatiTest.AnOut.Test_I_20mA,
                                       Parametri.Para4_20mA.I20mA_limit,
                                       I20mAread.ToString("#.0"),
                                       RisultatiTest.AnOut.I_20mA);

                Refresh_Log();

                ModulaLedsTest();
                Test420mAResultBorder.Background = RisultatiTest.AnOut.EsitoColor;
                Test420mAResult.Text = RisultatiTest.AnOut.EsitoTxt;
            }

            Test420mATimer.Stop();
            Test420mARing.Visibility = Visibility.Collapsed;
            TestRunning = VerificatorTests.None;

            if ((AllTest) && (!AbortPending))
                TestSimulation();
            else
            {
                EnableConverterTest();
                AbortPending = false;
            }
        }

        #endregion

        #region Test Simulation

        private static SimulationTestStep SimTestStep;
        private static SimulationTestStep SimTestStep_Mem;
        public enum SimulationTestStep
        {
            None = 0,
            Reading = 1,
            SetEmptyPipe = 2,
            GetEmptyPipe = 3,
            Sim_0 = 4,
            Sim_Low = 5,
            Sim_Hi = 6,
            Sim_Change = 7,
            Sim_Full = 8,
            Sim_Empty = 9,
            Sim_End = 10,
            CheckResults = 11
        }

        private float Res_Zero_min  = 0;
        private float Res_Zero_max  = 0;
        private float Res_LO_min_DEF    = 0;
        private float Res_LO_max_DEF    = 0;
        private float Res_HI_min_DEF    = 0;
        private float Res_HI_max_DEF    = 0;

        private float Res_LO_min_VER    = 0;
        private float Res_LO_max_VER    = 0;
        private float Res_HI_min_VER    = 0;
        private float Res_HI_max_VER    = 0;

        private string Reference_Zero;
        private string Reference_HI;
        private string Reference_LO;

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
        private async void TestSimulation()
        {
            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            if (MbConnection.IsOpen == false)
                await MbConnection.Open(ComSetup.ComPort608);

            TestInfo.Text = "Simulation Zero...";
            Simulation_Read.Text = "-- m/s";

            #region Init Limits

            Res_Zero_min = Parametri.Simul.zero_Res_min;
            Res_Zero_max = Parametri.Simul.zero_Res_max;
            Res_LO_min_DEF = Parametri.Simul.Low_Res_min;
            Res_LO_max_DEF = Parametri.Simul.Low_Res_max;
            Res_HI_min_DEF = Parametri.Simul.HI_Res_min;
            Res_HI_max_DEF = Parametri.Simul.HI_Res_max;

            OldSimTestFound = false;
            if (OldTestFound)
            {
                // Diverso da zero perchè nel caso di test precedentemente fatto andava in errore test
                if (Verificator.ReportList[OldTestIndex].LO_read != 0)
                {
                    Res_LO_min_VER = (float)Verificator.ReportList[OldTestIndex].LO_read * (1 - Parametri.Simul.Tolleranza_MinPerc / 100);
                    Res_LO_max_VER = (float)Verificator.ReportList[OldTestIndex].LO_read * (1 + Parametri.Simul.Tolleranza_MaxPerc / 100);
                    OldSimTestFound = true;
                }
                // Diverso da zero perchè nel caso di test precedentemente fatto andava in errore test
                if (Verificator.ReportList[OldTestIndex].Hi_read != 0)
                {
                    Res_HI_min_VER = (float)Verificator.ReportList[OldTestIndex].Hi_read * (1 - Parametri.Simul.Tolleranza_MinPerc / 100);
                    Res_HI_max_VER = (float)Verificator.ReportList[OldTestIndex].Hi_read * (1 + Parametri.Simul.Tolleranza_MaxPerc / 100);
                    OldSimTestFound = true;
                }
            }

            Reference_Zero = ">" + Res_Zero_min.ToString("#.00") + "/<" + Res_Zero_max.ToString("#.00");

            #endregion

            AbortPending = false;

            ReadingProgress.Value = 0;

            InterfacciaConv.Write(Parametri.Simul.DAC_Zero);
            SimTestStep     = SimulationTestStep.Reading;
            SimTestStep_Mem = SimulationTestStep.None;            
            ResetReading();
            
            ClearSimulationResults();
            DisableConverterTest();
            TestRunning = VerificatorTests.Conv_Simulation;
            SimulationTestRing.Visibility = Visibility.Visible;

            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ENTER_VERIFICATION, 0, 3);
            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
        }

        public void Portata_m_s_Verif()
        {
            NextSampleReady = false;
            MbConnection.ReadRegisters(ComSetup.Address, Map.Registri_CMD4.Measure_m_s, Map.Registri_CMD4.Measure_m_s, 3);
            MbConnection.ReadRegistersCompleted += MbConnection_ReadRegistersCompleted;
        }

        private bool EmptyPipeStatus;
        private void GetEmptyPipeStatus()
        {
            ModbusReadState = ModbusReadStates.ReadEmptyPipe;
            MbConnection.ReadRegisters(ComSetup.Address, Map.Registri_CMD4.res_EmptyPipeCk, Map.Registri_CMD4.res_EmptyPipeCk, 3);
            MbConnection.ReadRegistersCompleted += MbConnection_ReadRegistersCompleted;
        }

        private void SetEmptyPipeStatus()
        {
            ModbusReadState = ModbusReadStates.SetEmptyPipe;
            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_TEST_EMPTP, 0, 3);
            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
        }

        private void Cmd_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            WriteEEPROM cmd = sender as WriteEEPROM;
            if (cmd.Result.Outcome == Euromag.Protocols.CommandResultOutcomes.CommandSuccess)
            {

            }
        }

        private List<double> bufferLO = new List<double>();
        private double SumAverage = 0;
        public double Portata_ms_Average { get; set; }
        private byte EpipeTry = 0;
        private byte MeasCount;

        private DispatcherTimer PortataMsReadTimer;
        private void InitPortataMsReadTimer()
        {
            PortataMsReadTimer = new DispatcherTimer();
            PortataMsReadTimer.Interval = TimeSpan.FromMilliseconds(500);
            PortataMsReadTimer.Tick += PortataMsReadTimer_Tick;
            PortataMsReadTimer.Stop();
        }

        private async void PortataMsReadTimer_Tick(object sender, object e)
        {
            if (AbortPending)
            {
                SimTestStep = SimulationTestStep.Sim_End;
            }

            ModbusReadState = ModbusReadStates.ReadMeas;

            switch (SimTestStep)
            {
                case SimulationTestStep.Reading:
                    TestInfo.Text = TestInfo.Text + "...";
                    if (Portata_ms_Average == 0)
                        Simulation_Read.Text = "-.-- m/s";
                    else
                    {
                        if(Portata_ms_Average < 1)
                            Simulation_Read.Text = "0.00 m/s";
                        else
                            Simulation_Read.Text = Portata_ms_Average.ToString(".##") + " m/s";
                    }
                    if (MeasCount < 5)
                    {
                        Portata_m_s_Verif();
                        PortataMsReadTimer.Stop();
                    }
                    else
                    {
                        if (Portata_ms_Average == 0)
                        {
                            RisultatiTest.Simulazione.Zero  = RisultatiTest.Esito.FAIL;
                            RisultatiTest.Simulazione.LO    = RisultatiTest.Esito.FAIL;
                            RisultatiTest.Simulazione.Hi    = RisultatiTest.Esito.FAIL;

                            ResetReading();
                            SimTestStep = SimulationTestStep.Sim_Hi;
                            InterfacciaConv.Write(Parametri.Simul.DAC_Zero);
                            Simulation_Read.Text = "-.-- m/s";
                            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_EXIT_VERIFICATION, 0, 3);
                            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                            PortataMsReadTimer.Stop();

                            var dialog = new MessageDialog("Simulation error:\n\rCheck connectors or try to connect power supply and charge Battery", "Simulation Board Error");
                            await dialog.ShowAsync();
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
                        }
                    }
                    ReadingProgress.Value = MeasCount;
                    break;
                case SimulationTestStep.SetEmptyPipe:
                    if(SimTestStep_Mem == SimulationTestStep.Sim_Full)
                        TestInfo.Text = TestInfo.Text + "Set Full...";
                    if (SimTestStep_Mem == SimulationTestStep.Sim_Empty)
                        TestInfo.Text = TestInfo.Text + "Set Empty...";
                    SetEmptyPipeStatus();
                    PortataMsReadTimer.Stop();
                    break;
                case SimulationTestStep.GetEmptyPipe:
                    TestInfo.Text = TestInfo.Text + "Check";
                    GetEmptyPipeStatus();
                    PortataMsReadTimer.Stop();                    
                    break;
                case SimulationTestStep.Sim_0:
                    TestInfo.Text = "Simulation Mid...";
                    ResetReading();
                    if (RisultatiTest.Simulazione.Zero_read >= Res_Zero_min && RisultatiTest.Simulazione.Zero_read <= Res_Zero_max)
                        RisultatiTest.Simulazione.Zero = RisultatiTest.Esito.PASS_Verify;
                    else
                        RisultatiTest.Simulazione.Zero = RisultatiTest.Esito.FAIL;

                    SimTestStep_Mem = SimTestStep;
                    InterfacciaConv.Write(Parametri.Simul.DAC_Low_608);
                    SimTestStep = SimulationTestStep.Reading;
                    break;
                case SimulationTestStep.Sim_Low:
                    TestInfo.Text = "Simulation Hi...";
                    ResetReading();

                    Reference_LO = ">" + Res_LO_min_DEF.ToString("#.00") + "/<" + Res_LO_max_DEF.ToString("#.00");

                    if (RisultatiTest.Simulazione.LO_read >= Res_LO_min_DEF && RisultatiTest.Simulazione.LO_read <= Res_LO_max_DEF)
                    {
                       RisultatiTest.Simulazione.LO = RisultatiTest.Esito.PASS_Def;
                    }
                    else
                        RisultatiTest.Simulazione.LO = RisultatiTest.Esito.FAIL;

                    if (OldSimTestFound)
                    { 
                        if (RisultatiTest.Simulazione.LO_read >= Res_LO_min_VER && RisultatiTest.Simulazione.LO_read <= Res_LO_max_VER)
                        {
                            Reference_LO = ">" + Res_LO_min_VER.ToString("#.00") + "/<" + Res_LO_max_VER.ToString("#.00");
                            RisultatiTest.Simulazione.LO = RisultatiTest.Esito.PASS_Verify;
                        }
                    }

                    SimTestStep_Mem = SimTestStep;
                    InterfacciaConv.Write(Parametri.Simul.DAC_Hi_608);
                    SimTestStep = SimulationTestStep.Reading;
                    break;
                case SimulationTestStep.Sim_Hi:                    
                    ResetReading();

                    Reference_HI = ">" + Res_HI_min_DEF.ToString("#.00") + "/<" + Res_HI_max_DEF.ToString("#.00");

                    if (RisultatiTest.Simulazione.Hi_read >= Res_HI_min_DEF && RisultatiTest.Simulazione.Hi_read <= Res_HI_max_DEF)
                    {
                        RisultatiTest.Simulazione.Hi = RisultatiTest.Esito.PASS_Def;
                    }
                    else 
                        RisultatiTest.Simulazione.Hi = RisultatiTest.Esito.FAIL;

                    if (OldSimTestFound)
                    {
                        if (RisultatiTest.Simulazione.Hi_read >= Res_HI_min_VER && RisultatiTest.Simulazione.Hi_read <= Res_HI_max_VER)
                        {
                            Reference_HI = ">" + Res_HI_min_VER.ToString("#.00") + "/<" + Res_HI_max_VER.ToString("#.00");
                            RisultatiTest.Simulazione.Hi = RisultatiTest.Esito.PASS_Verify;
                        }
                    }
                   
                    SimTestStep_Mem = SimTestStep;
                    InterfacciaConv.Write(Parametri.Simul.DAC_Zero);
                    Simulation_Read.Text = "-.-- m/s";
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_EXIT_VERIFICATION, 0, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    PortataMsReadTimer.Stop();
                    break;

                case SimulationTestStep.Sim_Change:
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 1, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;

                    TestInfo.Text = "Simulation Full Pipe...";
                    EPipeStatus.Fill = GPIO_Control.SetEPipe();
                    EpipeTry = 0;
                    SimTestStep = SimulationTestStep.SetEmptyPipe;
                    SimTestStep_Mem = SimulationTestStep.Sim_Full;
                    EmptyPipeStatus = true;
                    PortataMsReadTimer.Stop();
                    break;
                case SimulationTestStep.Sim_Full:                    
                    if (EmptyPipeStatus == false)
                    {
                        RisultatiTest.Simulazione.FullPype = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.Simulazione.Full_Read = "Full";
                        TestInfo.Text = "Simulation Empty Pipe...";
                        EPipeStatus.Fill = GPIO_Control.ResEPipe();
                        EpipeTry = 0;
                        SimTestStep = SimulationTestStep.SetEmptyPipe;
                        SimTestStep_Mem = SimulationTestStep.Sim_Empty;                        
                        EmptyPipeStatus = false;
                    }
                    else if (EpipeTry++ >= 5)
                    {
                        RisultatiTest.Simulazione.FullPype = RisultatiTest.Esito.FAIL;
                        RisultatiTest.Simulazione.Full_Read = "Empty";
                        TestInfo.Text = "Simulation Empty Pipe...";
                        EPipeStatus.Fill = GPIO_Control.ResEPipe();
                        SimTestStep_Mem = SimTestStep;
                        SimTestStep = SimulationTestStep.Sim_End;
                        TestInfo.Text = "Simulation End...";
                    }

                    break;
                case SimulationTestStep.Sim_Empty:                    
                    if (EmptyPipeStatus == true)
                    {
                        RisultatiTest.Simulazione.EmptyPype = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.Simulazione.Empy_Read = "Empty";
                        SimTestStep_Mem = SimTestStep;
                        SimTestStep = SimulationTestStep.Sim_End;
                        TestInfo.Text = "Simulation End...";

                        if (RisultatiTest.Simulazione.FullPype == RisultatiTest.Esito.PASS_Verify)
                            RisultatiTest.Simulazione.EpipeTest = RisultatiTest.Esito.PASS_Verify;
                        else
                            RisultatiTest.Simulazione.EpipeTest = RisultatiTest.Esito.FAIL;
                    }
                    else if (EpipeTry++ >= 5)
                    {
                        RisultatiTest.Simulazione.EmptyPype = RisultatiTest.Esito.FAIL;
                        RisultatiTest.Simulazione.EpipeTest = RisultatiTest.Esito.FAIL;
                        RisultatiTest.Simulazione.Empy_Read = "Full";
                        SimTestStep_Mem = SimTestStep;
                        SimTestStep = SimulationTestStep.Sim_End;
                        TestInfo.Text = "Simulation End...";
                    }

                    break;
                case SimulationTestStep.Sim_End:
                    //Per evitare allarme tubo vuoto:
                    EPipeStatus.Fill = GPIO_Control.SetEPipe();
                    TestInfo.Text = "";
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 0, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    PortataMsReadTimer.Stop();
                    break;
                case SimulationTestStep.CheckResults:
                    RisultatoSimulationTest();
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
            }
            
        }

        void ClearSimulationResults()
        {
            RisultatiTest.Simulazione.Zero = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Simulazione.LO = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Simulazione.Hi = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Simulazione.FullPype = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Simulazione.EmptyPype = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.Simulazione.EpipeTest = RisultatiTest.Esito.DaEseguire;
            ModulaLedsTest();
            SimulationResultBorder.Background = new SolidColorBrush(Colors.Transparent);
            SimulationResult.Text = "--";
            SimulationTestRing.Visibility = Visibility.Collapsed;
        }

        private void ResetReading()
        {
            ReadingProgress.Value   = 0;
            ReadingProgress.Minimum = 0;
            ReadingProgress.Maximum = 5;
            Portata_ms_Average      = 0;
            SumAverage              = 0;
            MeasCount               = 0;
            bufferLO.Clear();
        }

        #endregion

        #region Test Energy Coil

        private static ECoilTestStep ECoil_TestStep;
        private static ECoilTestPhase ECoil_TestPhase;
        public enum ECoilTestStep
        {
            None         = 0,
            ECoil_Init   = 1,
            ECoil_Read   = 2,
            ECoil_End    = 3,
            CheckResults = 4
        }

        public enum ECoilTestPhase
        {
            None        = 0,
            ECoil_Zero  = 1,
            ECoil_Neg   = 2,
            ECoil_Pos   = 3,
            ECoil_End   = 4
        }


        private float Icoil_min_DEF = 0;
        private float Icoil_Max_DEF = 0;

        private float Icoil_min_VER = 0;
        private float Icoil_Max_VER = 0;

        private string Reference_Icoil_DEF;
        private string Reference_Icoil_VER;

        private string Reference_Icoil_Pos;
        private string Reference_Icoil_Neg;

        private string Reference_Offset;

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
        private async void StartECoilTest()
        {
            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            if (MbConnection.IsOpen == false)
                await MbConnection.Open(ComSetup.ComPort608);

            TestInfo.Text = "Start ECoil Test...";

            #region Init Limits

            switch (MC608.Bobine.I_Coil)
            {
                case (byte)MC608.ICoil.I_125mA:
                    Icoil_min_DEF = Parametri.ICoil.I_125mA_min;
                    Icoil_Max_DEF = Parametri.ICoil.I_125mA_max;
                    break;
                case (byte)MC608.ICoil.I_62mA:
                    Icoil_min_DEF = Parametri.ICoil.I_65mA_min;
                    Icoil_Max_DEF = Parametri.ICoil.I_65mA_max;
                    break;
                case (byte)MC608.ICoil.I_50mA:
                    Icoil_min_DEF = Parametri.ICoil.I_50mA_min;
                    Icoil_Max_DEF = Parametri.ICoil.I_50mA_max;
                    break;
                case (byte)MC608.ICoil.I_31mA:
                    Icoil_min_DEF = Parametri.ICoil.I_31mA_min;
                    Icoil_Max_DEF = Parametri.ICoil.I_31mA_max;
                    break;
                case (byte)MC608.ICoil.I_25mA:
                    Icoil_min_DEF = Parametri.ICoil.I_25mA_min;
                    Icoil_Max_DEF = Parametri.ICoil.I_25mA_max;
                    break;
            }


            if (OldTestFound)
            {
                // Diverso da zero perchè nel caso di test precedentemente fatto andava in errore test
                if (Verificator.ReportList[OldTestIndex].ICoil_Read != 0)
                {
                    Icoil_min_VER = (float)Verificator.ReportList[OldTestIndex].ICoil_Read * (1 - Parametri.ICoil.Tolleranza_MinPerc / 100);
                    Icoil_Max_VER = (float)Verificator.ReportList[OldTestIndex].ICoil_Read * (1 + Parametri.ICoil.Tolleranza_MaxPerc / 100);
                    OldECoilTestFound = true;
                }
                else
                    OldECoilTestFound = false;
            }

            Reference_Offset = ">" + Parametri.ICoil.Offset_min.ToString("#.00") + "/<" + Parametri.ICoil.Offset_max.ToString("#.00");
            Reference_Icoil_DEF = ">" + Icoil_min_DEF.ToString(".##") + "/<" + Icoil_Max_DEF.ToString(".##");
            Reference_Icoil_VER = ">" + Icoil_min_VER.ToString(".##") + "/<" + Icoil_Max_VER.ToString(".##");
            #endregion

            AbortPending = false;

            AnalogMeasures.Analogiche[AnalogsService.Icoil].TestOffset = 0;

            ECoil_TestStep              = ECoilTestStep.None;
            ECoil_TestPhase             = ECoilTestPhase.None;
            
            ClearECoilTestResults();
            ECoilTestTimer.Start();
            DisableConverterTest();
            TestRunning         =  VerificatorTests.Conv_EnergyCoil;
            CountOK_POS_verify  = 0;
            CountOK_POS_default = 0;

            ECoilTestRing.Visibility = Visibility.Visible;
            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 1, 3);
            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
        }

        private void ClearECoilTestResults()
        {
            RisultatiTest.ICoil.Zero = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.ICoil.Pos = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.ICoil.Neg = RisultatiTest.Esito.DaEseguire;

            ModulaLedsTest();

            ECoilTestResultBorder.Background = new SolidColorBrush(Colors.Transparent);
            ECoilTestResult.Text = "--";
            ECoilTestRing.Visibility = Visibility.Collapsed;
        }

        private DispatcherTimer ECoilTestTimer;
        private void InitECoilTestTimer()
        {
            ECoilTestTimer = new DispatcherTimer();
            ECoilTestTimer.Interval = TimeSpan.FromMilliseconds(500);
            ECoilTestTimer.Tick += ECoilTestTimer_Tick;
            ECoilTestTimer.Stop();
        }

        private void ReadICoil()
        {
            float min_def, max_def, min_ver, max_ver, measure;

            switch (ECoil_TestPhase)
            {
                case ECoilTestPhase.ECoil_Zero:
                    min_def = Parametri.ICoil.Offset_min;
                    max_def = Parametri.ICoil.Offset_max;
                    min_ver = Parametri.ICoil.Offset_min;
                    max_ver = Parametri.ICoil.Offset_max;

                    RisultatiTest.ICoil.Offset_I = AnalogMeasures.Analogiche[AnalogsService.Icoil].Misura;
                    AnalogMeasures.Analogiche[AnalogsService.Icoil].TestOffset = RisultatiTest.ICoil.Offset_I;

                    measure = RisultatiTest.ICoil.Offset_I;
                    break;
                case ECoilTestPhase.ECoil_Pos:
                    min_def = Icoil_min_DEF;
                    max_def = Icoil_Max_DEF;
                    min_ver = Icoil_min_VER;
                    max_ver = Icoil_Max_VER;
                    RisultatiTest.ICoil.Pos_I = AnalogMeasures.Analogiche[AnalogsService.Icoil].Misura;
                    measure = RisultatiTest.ICoil.Pos_I;
                    break;
                case ECoilTestPhase.ECoil_Neg:                                        
                    min_def = Icoil_min_DEF;
                    max_def = Icoil_Max_DEF;
                    min_ver = Icoil_min_VER;
                    max_ver = Icoil_Max_VER;
                    RisultatiTest.ICoil.Neg_I = AnalogMeasures.Analogiche[AnalogsService.Icoil].Misura;
                    measure = RisultatiTest.ICoil.Neg_I;
                    break;
                default:
                    min_def = Parametri.ICoil.Offset_min;
                    max_def = Parametri.ICoil.Offset_max;
                    min_ver = Parametri.ICoil.Offset_min;
                    max_ver = Parametri.ICoil.Offset_max;

                    RisultatiTest.ICoil.Offset_I = AnalogMeasures.Analogiche[AnalogsService.Icoil].Misura;
                    AnalogMeasures.Analogiche[AnalogsService.Icoil].TestOffset = RisultatiTest.ICoil.Offset_I;
                    measure = RisultatiTest.ICoil.Offset_I;
                    break;
            }

            if (OldECoilTestFound)
            {
                if (measure >= min_ver && measure <= max_ver)
                {
                    CountOK_POS_verify++;
                }
            }

            if (measure >= min_def && measure <= max_def)
            {
                CountOK_POS_default++;
            }
        }

        const byte INV_COIL = 0x01;
        const byte ON_COIL = 0x10;
        const byte OFF_COIL = 0x00;

        private int CountOK_POS_verify = 0;
        private int CountOK_POS_default = 0;
        private int Ripetiz_OK = 1;
        private int Ripetiz_DFT = 5;
        private int Ripetiz_FAIL = 20;
        private int Ripetizioni;
        private void ECoilTestTimer_Tick(object sender, object e)
        {
            if (AbortPending)
            {
                ECoil_TestStep = ECoilTestStep.ECoil_End;
            }

            switch (ECoil_TestStep)
            {
                case ECoilTestStep.None:
                    NextStepReady = true;
                    break;
                case ECoilTestStep.ECoil_Init:
                    TestInfo.Text       = "Set Zero...";
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ACT_COIL_set, OFF_COIL, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    AnalogMeasures.StartNewMeasure(AnalogsService.Icoil);
                    ECoilTestTimer.Stop();
                    NextStepReady = true;
                    break;
                case ECoilTestStep.ECoil_Read:
                    TestInfo.Text = TestInfo.Text + "...";
                    if ((CountOK_POS_verify >= Ripetiz_OK) || (CountOK_POS_default >= Ripetiz_DFT))
                    {                        
                        switch(ECoil_TestPhase)
                        {
                            case (ECoilTestPhase.ECoil_Zero):
                                if(CountOK_POS_verify >= Ripetiz_OK)
                                    RisultatiTest.ICoil.Zero = RisultatiTest.Esito.PASS_Verify;
                                else
                                    RisultatiTest.ICoil.Zero = RisultatiTest.Esito.PASS_Def;
                                ECoil_TestPhase = ECoilTestPhase.ECoil_Neg;
                                TestInfo.Text = "Set Neg...";
                                MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ACT_COIL_set, ON_COIL, 3);
                                MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                                break;
                            case (ECoilTestPhase.ECoil_Neg):
                                if (CountOK_POS_verify >= Ripetiz_OK)
                                {
                                    RisultatiTest.ICoil.Neg = RisultatiTest.Esito.PASS_Verify;
                                    Reference_Icoil_Neg = Reference_Icoil_VER;
                                }
                                else
                                {
                                    RisultatiTest.ICoil.Neg = RisultatiTest.Esito.PASS_Def;
                                    Reference_Icoil_Neg = Reference_Icoil_DEF;
                                }
                                ECoil_TestPhase = ECoilTestPhase.ECoil_Pos;
                                TestInfo.Text = "Set Pos...";
                                MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ACT_COIL_set, ON_COIL + INV_COIL, 3);
                                MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                                break;
                            case (ECoilTestPhase.ECoil_Pos):
                                if (CountOK_POS_verify >= Ripetiz_OK)
                                {
                                    RisultatiTest.ICoil.Pos = RisultatiTest.Esito.PASS_Verify;
                                    Reference_Icoil_Pos = Reference_Icoil_VER;
                                }
                                else
                                {
                                    RisultatiTest.ICoil.Pos = RisultatiTest.Esito.PASS_Def;
                                    Reference_Icoil_Pos = Reference_Icoil_DEF;
                                }

                                ECoil_TestPhase = ECoilTestPhase.ECoil_End;
                                TestInfo.Text = "End...";

                                MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ACT_COIL_set, OFF_COIL, 3);
                                MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                                break;
                        }
                        CountOK_POS_verify = 0;
                        CountOK_POS_default = 0;
                        Ripetizioni = 0;
                        AnalogMeasures.StartNewMeasure(AnalogsService.Icoil);
                        ECoilTestTimer.Stop();
                        NextStepReady = true;
                    }
                    else
                    { 
                        if (Ripetizioni++ <= Ripetiz_FAIL)
                        {
                            ReadICoil();
                        }
                        else
                        {
                            switch (ECoil_TestPhase)
                            {
                                case (ECoilTestPhase.ECoil_Zero):
                                    RisultatiTest.ICoil.Zero = RisultatiTest.Esito.FAIL;                                    
                                    TestInfo.Text = "End...";
                                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ACT_COIL_set, OFF_COIL, 3);
                                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                                    break;
                                case (ECoilTestPhase.ECoil_Neg):
                                    RisultatiTest.ICoil.Neg = RisultatiTest.Esito.FAIL;
                                    TestInfo.Text = "End...";
                                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ACT_COIL_set, OFF_COIL, 3);
                                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                                    break;
                                case (ECoilTestPhase.ECoil_Pos):
                                    RisultatiTest.ICoil.Pos = RisultatiTest.Esito.FAIL;
                                    TestInfo.Text = "End...";
                                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ACT_COIL_set, OFF_COIL, 3);
                                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                                    break;
                            }
                            CountOK_POS_verify = 0;
                            CountOK_POS_default = 0;
                            Ripetizioni = 0;
                            ECoilTestTimer.Stop();
                            NextStepReady = true;
                        }
                    }
                    break;
                case ECoilTestStep.ECoil_End:
                    TestInfo.Text = "";
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 0, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    ECoilTestTimer.Stop();
                    NextStepReady = true;
                    break;
                case ECoilTestStep.CheckResults:
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
                RisultatiTest.Res_ICoil608();


                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.ICoil.Test_Zero,
                                      Reference_Offset,
                                      RisultatiTest.ICoil.Offset_I.ToString("#.00"),
                                      RisultatiTest.ICoil.Zero);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.ICoil.Test_Neg,
                                      Reference_Icoil_Neg,
                                      RisultatiTest.ICoil.Neg_I.ToString("#.00"),
                                      RisultatiTest.ICoil.Neg);

                TestID = Notifica_Log(TestID,
                                      "Conv.",
                                      RisultatiTest.ICoil.Test_Pos,
                                      Reference_Icoil_Pos,
                                      RisultatiTest.ICoil.Pos_I.ToString("#.00"),
                                      RisultatiTest.ICoil.Pos);

                Refresh_Log();

                ModulaLedsTest();
                ECoilTestResultBorder.Background = RisultatiTest.ICoil.EsitoColor;
                ECoilTestResult.Text = RisultatiTest.ICoil.EsitoTxt;
            }

            ECoilTestTimer.Stop();
            ECoilTestRing.Visibility = Visibility.Collapsed;
            TestRunning = VerificatorTests.None;
            ModbusReadState = ModbusReadStates.Idle;

            if ((AllTest) && (!AbortPending))
                StartIOTest();
            else
            {
                EnableConverterTest();               
                AbortPending = false;
            }
        }

        #endregion

        #region TEST I/O

        private static IOTestStep IO_TestStep;
        private static IOTestStep IO_TestStep_Mem;
        public enum IOTestStep
        {
            IO_None         = 0,
            IO_Init         = 1,
            IO_EnterTest    = 2,
            IO_ResetIO      = 3,
            IO_GP_IN_off    = 4,
            IO_GP_IN_on     = 5,
            IO_GP_OUT_off   = 6,
            IO_GP_OUT_on    = 7,
            IO_GP_PULSE_off = 8,
            IO_GP_PULSE_on  = 9,
            IO_GP_FREQ_Off  = 10,
            IO_GP_FREQ_on   = 11,
            IO_End          = 12
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

        private async void StartIOTest()
        {
            if (SensorSimulator.IsOpen)
                SensorSimulator.Close();

            if (MbConnection.IsOpen == false)
                await MbConnection.Open(ComSetup.ComPort608);

            TestInfo.Text = "Test IO Init...";
            IO_TestStep = IOTestStep.IO_Init;
            IO_TestStep_Mem = IOTestStep.IO_Init;
            
            ClearIOTestResults();
            IOTestTimer.Stop();
            DisableConverterTest();

            AbortPending = false;

            if (MC608.Release_HW.Versione == 6 && MC608.Release_HW.Revisione >= 3)
            {
                VAuxStatus.Fill = GPIO_Control.SetVAux();
            }
            else
            {
                VAuxStatus.Fill = GPIO_Control.ResVAux();
            }
            
            TestRunning = VerificatorTests.Conv_IO;
            IOTestTestRing.Visibility = Visibility.Visible;

            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.EN_WR_MODBUS, 0, 3);
            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
            NextStepReady = true;
        }

        private void ClearIOTestResults()
        {
            RisultatiTest.IO.GP_IN_off = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.IO.GP_IN_on = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.IO.GP_OUT_off = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.IO.GP_OUT_on = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.IO.GP_PULSE_off = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.IO.GP_PULSE_on = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.IO.GP_FREQ_Off = RisultatiTest.Esito.DaEseguire;
            RisultatiTest.IO.GP_FREQ_on = RisultatiTest.Esito.DaEseguire;

            ModulaLedsTest();

            TestIOResultBorder.Background = new SolidColorBrush(Colors.Transparent);
            TestIOResult.Text = "--";
            IOTestTestRing.Visibility = Visibility.Collapsed;
        }

        private void GetDigitalIOStatus()
        {
            NextStepWait = true;
            IOTestTimer.Stop();
            ModbusReadState = ModbusReadStates.ReadDigitalIO;
            MbConnection.ReadRegisters(ComSetup.Address, Map.Registri_CMD4.Digital_IO, Map.Registri_CMD4.Digital_IO, 3);
            MbConnection.ReadRegistersCompleted += MbConnection_ReadRegistersCompleted;
        }

        private DispatcherTimer IOTestTimer;
        private void InitIOTestTimer()
        {
            IOTestTimer = new DispatcherTimer();
            IOTestTimer.Interval = TimeSpan.FromMilliseconds(500);
            IOTestTimer.Tick += IOTestTimer_Tick; ;
            IOTestTimer.Stop();
        }

        private void IOTestTimer_Tick(object sender, object e)
        {
            if (AbortPending)
            {
                IO_TestStep = IOTestStep.IO_End;
            }

            switch (IO_TestStep)
            {
                case IOTestStep.IO_EnterTest:
                    IO_TestStep_Mem = IOTestStep.IO_None;                    
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 1, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    IOTestTimer.Stop();
                    NextStepReady = true;
                    break;
                case IOTestStep.IO_ResetIO:
                    IO_TestStep_Mem = IOTestStep.IO_None;
                    TestInfo.Text = "Enter Test Mode...";                    
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.SET_IO_MDB, 0, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    IOTestTimer.Stop();
                    NextStepReady = true;
                    break;
                case IOTestStep.IO_GP_IN_off:
                    IO_TestStep_Mem = IOTestStep.IO_None;
                    TestInfo.Text = "Reset IO...";                    
                    GPInStatus.Fill = GPIO_Control.ResGPIn();
                    GetDigitalIOStatus();                    
                    break;
                case IOTestStep.IO_GP_IN_on:
                    TestInfo.Text = "Prog In On...";
                    if (MC608.DIGITAL_IO.GP_IN == true)
                    { 
                        RisultatiTest.IO.GP_IN_off = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.IO.Test_GP_IN_off.Reading = "Off";
                    }
                    else
                    { 
                        RisultatiTest.IO.GP_IN_off = RisultatiTest.Esito.FAIL;
                        RisultatiTest.IO.Test_GP_IN_off.Reading = "On";
                    }

                    IO_TestStep_Mem = IOTestStep.IO_None;
                    
                    GPInStatus.Fill = GPIO_Control.SetGPIn();
                    GetDigitalIOStatus();
                    
                    break;
                case IOTestStep.IO_GP_OUT_off:
                    TestInfo.Text = "Prog In Off...";
                    if (MC608.DIGITAL_IO.GP_IN == false)
                    { 
                        RisultatiTest.IO.GP_IN_on = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.IO.Test_GP_IN_on.Reading = "On";
                    }
                    else
                    { 
                        RisultatiTest.IO.GP_IN_on = RisultatiTest.Esito.FAIL;
                        RisultatiTest.IO.Test_GP_IN_on.Reading = "Off";
                    }

                    IO_TestStep_Mem = IOTestStep.IO_GP_OUT_off;
                    GPInStatus.Fill = GPIO_Control.ResGPIn();                    
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.SET_IO_MDB, 0, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    
                    IOTestTimer.Stop();
                    NextStepReady = true;
                    break;
                case IOTestStep.IO_GP_OUT_on:
                    TestInfo.Text = "Prog Out On...";
                    ProgOutStatus.Fill = GPIO_Control.GetProgOut().color;
                    if ( GPIO_Control.GetProgOut().value == false )
                    { 
                        RisultatiTest.IO.GP_OUT_off = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.IO.Test_GP_OUT_off.Reading = "Off";
                    }
                    else
                    { 
                        RisultatiTest.IO.GP_OUT_off = RisultatiTest.Esito.FAIL;
                        RisultatiTest.IO.Test_GP_OUT_off.Reading = "On";
                    }
                    
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.SET_IO_MDB, (byte)Modbus.Functions.BitField.GP_out, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    RisultatiTest.IO.GP_OUT_on = RisultatiTest.Esito.PASS_Verify;
                    IOTestTimer.Stop();
                    NextStepReady = true;
                    break;
                case IOTestStep.IO_GP_PULSE_off:
                    TestInfo.Text = "Prog Out Off...";
                    ProgOutStatus.Fill = GPIO_Control.GetProgOut().color;
                    if (GPIO_Control.GetProgOut().value == true)
                    { 
                        RisultatiTest.IO.GP_OUT_on = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.IO.Test_GP_OUT_on.Reading = "On";
                    }
                    else
                    { 
                        RisultatiTest.IO.GP_OUT_on = RisultatiTest.Esito.FAIL;
                        RisultatiTest.IO.Test_GP_OUT_on.Reading = "Off";
                    }
                    
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.SET_IO_MDB, 0, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    RisultatiTest.IO.GP_PULSE_off = RisultatiTest.Esito.PASS_Verify;
                    IOTestTimer.Stop();
                    NextStepReady = true;
                    break;
                case IOTestStep.IO_GP_PULSE_on:
                    TestInfo.Text = "Pulse On...";
                    PulseOutStatus.Fill = GPIO_Control.GetPulseOut().color;
                    if (GPIO_Control.GetPulseOut().value == false)
                    { 
                        RisultatiTest.IO.GP_PULSE_off = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.IO.Test_PULSE_off.Reading = "Off";
                    }
                    else
                    { 
                        RisultatiTest.IO.GP_PULSE_off = RisultatiTest.Esito.FAIL;
                        RisultatiTest.IO.Test_PULSE_off.Reading = "On";
                    }
                    
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.SET_IO_MDB, (byte)Modbus.Functions.BitField.Pulse_Out, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    RisultatiTest.IO.GP_PULSE_on = RisultatiTest.Esito.PASS_Verify;
                    IOTestTimer.Stop();
                    NextStepReady = true;
                    break;
                case IOTestStep.IO_GP_FREQ_Off:
                    TestInfo.Text = "Pulse Off...";
                    PulseOutStatus.Fill = GPIO_Control.GetPulseOut().color;
                    if (GPIO_Control.GetPulseOut().value == true)
                    { 
                        RisultatiTest.IO.GP_PULSE_on = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.IO.Test_PULSE_on.Reading = "On";
                    }
                    else
                    { 
                        RisultatiTest.IO.GP_PULSE_on = RisultatiTest.Esito.FAIL;
                        RisultatiTest.IO.Test_PULSE_on.Reading = "Off";
                    }                    

                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.SET_IO_MDB, 0, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    RisultatiTest.IO.GP_FREQ_Off = RisultatiTest.Esito.PASS_Verify;
                    IOTestTimer.Stop();
                    NextStepReady = true;
                    break;
                case IOTestStep.IO_GP_FREQ_on:
                    TestInfo.Text = "Freq On...";
                    FreqOutStatus.Fill = GPIO_Control.GetFreqOut().color;

                    if (MC608.Bobine.I_Coil == (byte)MC608.ICoil.I_125mA)
                    {
                        if (GPIO_Control.GetFreqOut().value == false)
                        {
                            RisultatiTest.IO.GP_FREQ_Off = RisultatiTest.Esito.PASS_Verify;
                            RisultatiTest.IO.Test_GP_FREQ_Off.Reading = "On";
                        }
                        else
                        {
                            RisultatiTest.IO.GP_FREQ_Off = RisultatiTest.Esito.FAIL;
                            RisultatiTest.IO.Test_GP_FREQ_Off.Reading = "Off";
                        }
                    }
                    else
                    {
                        RisultatiTest.IO.GP_FREQ_Off = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.IO.Test_GP_FREQ_Off.Reading = "On";
                    }

                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.SET_IO_MDB, (byte)Modbus.Functions.BitField.Freq_Out, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    RisultatiTest.IO.GP_FREQ_on = RisultatiTest.Esito.PASS_Verify;
                    IOTestTimer.Stop();
                    NextStepReady = true;

                    break;
                case IOTestStep.IO_End:
                    TestInfo.Text = "Freq Off...";

                    FreqOutStatus.Fill = GPIO_Control.GetFreqOut().color;

                    if (MC608.Bobine.I_Coil == (byte)MC608.ICoil.I_125mA)
                    {
                        if (GPIO_Control.GetFreqOut().value == true)
                        {
                            RisultatiTest.IO.GP_FREQ_on = RisultatiTest.Esito.PASS_Verify;
                            RisultatiTest.IO.Test_GP_FREQ_on.Reading = "On";
                        }
                        else
                        {
                            RisultatiTest.IO.GP_FREQ_on = RisultatiTest.Esito.FAIL;
                            RisultatiTest.IO.Test_GP_FREQ_on.Reading = "Off";
                        }
                    }
                    else
                    {
                        RisultatiTest.IO.GP_FREQ_on = RisultatiTest.Esito.PASS_Verify;
                        RisultatiTest.IO.Test_GP_FREQ_on.Reading = "On";
                    }
                    TestInfo.Text = "";                    
                    VAuxStatus.Fill = GPIO_Control.ResVAux();
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 0, 3);
                    MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
                    RisultatiTestIO();
                    break;
            }
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
                RisultatiTest.Res_IO_608();


                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_GP_IN_off,
                                      "Off",
                                      RisultatiTest.IO.Test_GP_IN_off.Reading,
                                      RisultatiTest.IO.GP_IN_off);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_GP_IN_on,
                                      "On",
                                      RisultatiTest.IO.Test_GP_IN_on.Reading,
                                      RisultatiTest.IO.GP_IN_on);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_GP_OUT_off,
                                      "Off",
                                      RisultatiTest.IO.Test_GP_OUT_off.Reading,
                                      RisultatiTest.IO.GP_OUT_off);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_GP_OUT_on,
                                      "On",
                                      RisultatiTest.IO.Test_GP_OUT_on.Reading,
                                      RisultatiTest.IO.GP_OUT_on);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_PULSE_off,
                                      "Off",
                                      RisultatiTest.IO.Test_PULSE_off.Reading,
                                      RisultatiTest.IO.GP_PULSE_off);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_PULSE_on,
                                      "On",
                                      RisultatiTest.IO.Test_PULSE_on.Reading,
                                      RisultatiTest.IO.GP_PULSE_on);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_GP_FREQ_Off,
                                      "Off",
                                      RisultatiTest.IO.Test_GP_FREQ_Off.Reading,
                                      RisultatiTest.IO.GP_FREQ_Off);

                TestID = Notifica_Log(TestID,
                                     "Conv.",
                                      RisultatiTest.IO.Test_GP_FREQ_on,
                                      "On",
                                      RisultatiTest.IO.Test_GP_FREQ_on.Reading,
                                      RisultatiTest.IO.GP_FREQ_on);

                Refresh_Log();

                ModulaLedsTest();
                TestIOResultBorder.Background = RisultatiTest.IO.EsitoColor;
                TestIOResult.Text = RisultatiTest.IO.EsitoTxt;
            }

            IOTestTimer.Stop();
            IOTestTestRing.Visibility = Visibility.Collapsed;

            PulseOutStatus.Fill = new SolidColorBrush(Colors.Gray);
            ProgOutStatus.Fill = new SolidColorBrush(Colors.Gray);
            FreqOutStatus.Fill = new SolidColorBrush(Colors.Gray);

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

        private ICommand _openSensorSimulatorCOMcmd;
        public ICommand OpenSensorSimulatorCOMcmd
        {
            get
            {
                if (_openSensorSimulatorCOMcmd == null)
                {
                    _openSensorSimulatorCOMcmd = new RelayCommand(
                        param => OpenSensorSimulatorCom()
                            );
                }
                return _openSensorSimulatorCOMcmd;
            }
        }

        private async void OpenSensorSimulatorCom()
        {
            //Apro comunicazione con la scheda STM1
            if (ComSetup.SimulatorComPort.ID == null)
            {
                var dialog = new MessageDialog("SensorSimulator COM Port Not Found", "COM Port Error");
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
            if (MbConnection.IsOpen)
                MbConnection.Close();

            if (SensorSimulator.IsOpen == false)
            { 
                if( await SensorSimulator.Open() )
                    SimulatorState.Fill = new SolidColorBrush(Colors.LimeGreen);
            }

            ClearSensorResults();
            RisultatiTest.Sensore.dry = true;
            SensorTestRing.Visibility = Visibility.Visible;
            SensorSimulator.SendCommand(SensorSimulator.portHandler, SimulatorCOMPortManager.CMD.DryTest, 5);
            SensorSimulator.CommandCompleted += Sensor_CommandCompleted;
            STestStep = SensorTestStep.Start;
            DisableSensorTest();
            TestRunning = VerificatorTests.Sensor;
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
            if (MbConnection.IsOpen)
                MbConnection.Close();

            if (SensorSimulator.IsOpen == false)
            {
                if (await SensorSimulator.Open())
                    SimulatorState.Fill = new SolidColorBrush(Colors.LimeGreen);
            }

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

            if (SensorPingTimer.IsEnabled == false)
            {
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
                                SensorSimulator.IsReady = true;
                                SimulatorState.Fill = new SolidColorBrush(Colors.LimeGreen);
                                SensorSimulator.Close();
                                break;
                            case SensorTestStep.Error:
                                SensorSimulator.IsReady = false;
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

                    SensorPingTimer.Start();
                }
            }
        }

        private void SensorPingTimer_Tick(object sender, object e)
        {

            switch (STestStep)
            {
                case SensorTestStep.Ping:
                    //SensorPingTimer.Stop();
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

            TestID = Notifica_Log(TestID,
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

                TestID = Notifica_Log(TestID,
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

                TestID = Notifica_Log(TestID,
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

                TestID = Notifica_Log(TestID,
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

                TestID = Notifica_Log(TestID,
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

                TestID = Notifica_Log(TestID,
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

                TestID = Notifica_Log(TestID,
                                        "Sens.",
                                        RisultatiTest.Sensore.Test_RH_TC,
                                        ">" + RtoKohm(Parametri.TestSensore.RH_Min),
                                        RtoKohm(RisultatiTest.Sensore.R_TC),
                                        RisultatiTest.Sensore.RH_TC
                                      );

                //////////////////////////////////////////////////////////
            }

            Refresh_Log();

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

        string RtoKohm(float value)
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
            SensorTestRing.Visibility = Visibility.Collapsed;
        }

        private void SimulatorError()
        {
            RisultatiTest.TestStep ErrorTest = new RisultatiTest.TestStep();
            ErrorTest.ID = 99;
            ErrorTest.Description = "Simulator Error";
            ErrorTest.Reference = "Abort";
            ErrorTest.Result = "Error";
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
            SensorPingTimer = new DispatcherTimer();
            SensorPingTimer.Interval = TimeSpan.FromMilliseconds(2000);
            SensorPingTimer.Tick += SensorPingTimer_Tick;
            SensorPingTimer.Stop();
        }

        private byte[] SensorTestResultData = new byte[32];

        #endregion

        #region ADC

        private DispatcherTimer RefreshAnalogsTimer;
        private void RefreshAnalogsStart()
        {
            RefreshAnalogsTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
            RefreshAnalogsTimer.Tick += RefreshAnalogsTimer_Tick;
            RefreshAnalogsTimer.Start();
        }

        private bool Test420mABtnCanBeEnabled;

        private void RefreshAnalogsTimer_Tick(object sender, object e)
        {
            AnalogMeasures.AggiornaMisure();

            //Se tensione troppo bassa disabilito il test 4/20mA, in quanto potrebbe
            //dare falsi errori
            if ( AnalogMeasures.VAlimLow )
            { 
                Test420mABtn.IsEnabled = false;
                Test420mABtnCanBeEnabled = true;
            }
            else
            {
                if (Test420mABtnCanBeEnabled)
                { 
                    Test420mABtn.IsEnabled = true;
                    Test420mABtnCanBeEnabled = false;
                }
            }

            if (TestRunning == VerificatorTests.Conv_420mA)
            {
                if(Test420mA_Step == Test420mAStep.Test12mA)
                    Out420mA_Meas.Text = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].Misura2.ToString("#.00") + " mA";
                else
                    Out420mA_Meas.Text = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].Misura.ToString("#.00") + " mA";

                TestOffsetOut420mA.Text = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].TestOffset.ToString("#.00") + " mA";
            }
            else
            {
                Out420mA_Meas.Text = ".00 mA";
                TestOffsetOut420mA.Text = ".00 mA";
            }

            Icoil_Meas.Text = AnalogMeasures.Analogiche[AnalogsService.Icoil].Misura.ToString("#.00") + " mA";
            TestOffsetIcoil.Text = AnalogMeasures.Analogiche[AnalogsService.Icoil].TestOffset.ToString("#.00") + " mA";

            ValimInVoltBlock.Text = AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura.ToString("#.00") + " V";
        }


        #endregion

        #region Common Resources

        private bool NextSampleReady;
        private bool NextStepWait;
        private bool ReadTaraReady;
        private bool ReadBatteryModeReady;

        private async void MbConnection_ReadRegistersCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;
            if (cmd.ReadRegisters_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                MbConnection.MbConnectionStatus = MbCOMPortManager.MbConnectionStates.Working;
                switch (ModbusReadState)
                {
                    case ModbusReadStates.Idle:
                        break;
                    case ModbusReadStates.ReadParams:
                        ConncetionRing.Visibility = Visibility.Visible;
                        ModbusReadState = ModbusReadStates.ReadTara;
                        CheckDatabaseTest(true);
                        ModbusReader.Start();
                        ReadTaraReady = false;
                        ReadBatteryModeReady = false;
                        break;
                    case ModbusReadStates.ReadTara:
                        if (ReadTaraReady)
                        { 
                            ModbusReadState = ModbusReadStates.ReadBatteryMode;
                            ModbusReader.Start();
                        }
                        break;
                    case ModbusReadStates.ReadBatteryMode:
                        if(ReadBatteryModeReady)
                        {
                            ModbusReadState = ModbusReadStates.Idle;
                            EnableConverterTest();
                            ConverterState.Fill = new SolidColorBrush(Colors.LimeGreen);
                            ConncetionRing.Visibility = Visibility.Collapsed;
                            MbConnection.Close();
                            AggiornaInfo();
                        }
                        break;
                    case ModbusReadStates.ReadMeas:
                        if((NextSampleReady == false) && (AbortPending == false))
                        {
                            if(MeasCount > 0)
                            { 
                                bufferLO.Add(MC608_Device.Flow_ms);
                                SumAverage += MC608_Device.Flow_ms;
                                Portata_ms_Average = SumAverage / bufferLO.Count;
                            }
                            MeasCount++;
                            PortataMsReadTimer.Start();
                            NextSampleReady = true;
                        }
                        break;
                    case ModbusReadStates.ReadEmptyPipe:
                        if (AbortPending == false)
                        { 
                            if (Map.Registri_CMD4.res_EmptyPipeCk.LSB_Byte == 0)
                                EmptyPipeStatus = false;
                            else
                                EmptyPipeStatus = true;
                            SimTestStep = SimTestStep_Mem;
                            PortataMsReadTimer.Start();
                        }
                        break;
                    case ModbusReadStates.ReadDigitalIO:                        
                        if ((NextStepWait) && (AbortPending == false))
                        {
                            switch (IO_TestStep)
                            {
                                case IOTestStep.IO_GP_IN_off:
                                    IO_TestStep = IOTestStep.IO_GP_IN_on;                                
                                    break;
                                case IOTestStep.IO_GP_IN_on:
                                    IO_TestStep = IOTestStep.IO_GP_OUT_off;
                                    break;
                            }
                            IOTestTimer.Start();
                            NextStepWait = false;
                        }
                        break;
                }
            }
            else
            {
                var dialog = new MessageDialog(cmd.ReadRegisters_CommandResult.Message, "Connection Error");
                await dialog.ShowAsync();
                AbortAllTests();
            }
        }


        private async void MbConnection_SendCommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;

            if (cmd.SendCommand_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                switch (TestRunning)
                {
                    case VerificatorTests.Conv_420mA:
                        if (NextStepReady)
                        {
                            NextStepReady = false;
                            switch (Test420mA_Step)
                            {
                                case Test420mAStep.ResetTest:
                                    Test420mA_Step = Test420mAStep.TestOffset;
                                    Test420mAStabilizationTimer = 10;
                                    break;
                                case Test420mAStep.TestOffset:
                                    Test420mA_Step = Test420mAStep.Test4mA;
                                    Test420mAStabilizationTimer = 10;
                                    break;
                                case Test420mAStep.Test12mA:
                                    Test420mA_Step = Test420mAStep.Test20mA;
                                    Test420mAStabilizationTimer = 10;
                                    break;
                                case Test420mAStep.Test20mA:
                                    Test420mA_Step = Test420mAStep.TestResult;
                                    break;
                                case Test420mAStep.TestResult:
                                    Test420mA_Step = Test420mAStep.TestEnd;
                                    break;
                            }
                        }
                        Test420mATimer.Start();                        
                        break;
                    case VerificatorTests.Conv_Simulation:
                        if (SimTestStep == SimulationTestStep.Sim_Hi)
                            SimTestStep = SimulationTestStep.Sim_Change;
                        if(SimTestStep == SimulationTestStep.Sim_End)
                            SimTestStep = SimulationTestStep.CheckResults;
                        if (ModbusReadState == ModbusReadStates.SetEmptyPipe)
                            SimTestStep = SimulationTestStep.GetEmptyPipe;

                        PortataMsReadTimer.Start();
                        break;
                    case VerificatorTests.Conv_IO:
                        if ((NextStepReady)  && (AbortPending == false))
                        {
                            NextStepReady = false;

                            switch (IO_TestStep)
                            {
                                case IOTestStep.IO_Init:
                                    IO_TestStep = IOTestStep.IO_EnterTest;
                                    IOTestTimer.Start();
                                    break;
                                case IOTestStep.IO_EnterTest:
                                    IO_TestStep = IOTestStep.IO_ResetIO;
                                    IOTestTimer.Start();
                                    break;
                                case IOTestStep.IO_ResetIO:
                                    IO_TestStep = IOTestStep.IO_GP_IN_off;
                                    IOTestTimer.Start();
                                    break;
                                case IOTestStep.IO_GP_OUT_off:
                                    if (IO_TestStep_Mem == IOTestStep.IO_GP_OUT_off)
                                    {
                                        IO_TestStep_Mem = IOTestStep.IO_None;
                                        IO_TestStep = IOTestStep.IO_GP_OUT_on;
                                        IOTestTimer.Start();
                                    }
                                    break;
                                case IOTestStep.IO_GP_OUT_on:
                                    IO_TestStep = IOTestStep.IO_GP_PULSE_off;
                                    IOTestTimer.Start();
                                    break;
                                case IOTestStep.IO_GP_PULSE_off:
                                    IO_TestStep = IOTestStep.IO_GP_PULSE_on;
                                    IOTestTimer.Start();
                                    break;
                                case IOTestStep.IO_GP_PULSE_on:
                                    IO_TestStep = IOTestStep.IO_GP_FREQ_Off;
                                    IOTestTimer.Start();
                                    break;
                                case IOTestStep.IO_GP_FREQ_Off:
                                    IO_TestStep = IOTestStep.IO_GP_FREQ_on;
                                    IOTestTimer.Start();
                                    break;
                                case IOTestStep.IO_GP_FREQ_on:
                                    IO_TestStep = IOTestStep.IO_End;
                                    IOTestTimer.Start();
                                    break;
                                case IOTestStep.IO_End:
                                    MbConnection.Close();
                                    break;
                            }
                        }
                        break;
                    case VerificatorTests.Conv_EnergyCoil:
                        if (NextStepReady)
                        {
                            NextStepReady = false;
                            switch (ECoil_TestStep)
                            {
                                case ECoilTestStep.None:
                                    ECoil_TestStep = ECoilTestStep.ECoil_Init;
                                    break;
                                case ECoilTestStep.ECoil_Init:
                                    ECoil_TestStep = ECoilTestStep.ECoil_Read;
                                    ECoil_TestPhase = ECoilTestPhase.ECoil_Zero;
                                    Ripetizioni = 0;
                                    if (AbortPending == false)
                                        ECoilTestTimer.Start();
                                    break;
                                case ECoilTestStep.ECoil_Read:
                                    if ((ECoil_TestPhase == ECoilTestPhase.ECoil_Zero) &&
                                        (RisultatiTest.ICoil.Zero == RisultatiTest.Esito.FAIL))
                                        ECoil_TestStep = ECoilTestStep.ECoil_End;
                                    else if ((ECoil_TestPhase == ECoilTestPhase.ECoil_Neg) &&
                                        (RisultatiTest.ICoil.Neg == RisultatiTest.Esito.FAIL))
                                        ECoil_TestStep = ECoilTestStep.ECoil_End;
                                    else if ((ECoil_TestPhase == ECoilTestPhase.ECoil_Pos) &&
                                        (RisultatiTest.ICoil.Neg == RisultatiTest.Esito.FAIL))
                                        ECoil_TestStep = ECoilTestStep.ECoil_End;
                                    else if (ECoil_TestPhase == ECoilTestPhase.ECoil_End)
                                        ECoil_TestStep = ECoilTestStep.ECoil_End;
                                    if (AbortPending == false)
                                        ECoilTestTimer.Start();
                                    break;
                                case ECoilTestStep.ECoil_End:
                                    ECoil_TestStep = ECoilTestStep.CheckResults;
                                    if (AbortPending == false)
                                        ECoilTestTimer.Start();
                                    break;
                            }
                        }
                        break;
                    case VerificatorTests.Sensor:
                        break;
                }
            }
            else
            {
                var dialog = new MessageDialog(cmd.ReadRegisters_CommandResult.Message, "Connection Error");
                await dialog.ShowAsync();
                AbortAllTests();
            }
        }


        ParametriVerificator Parametri = new ParametriVerificator(); // Soglie e tolleranze test
        private static int TestID = 1;

        private int Notifica_LogSensore(RisultatiTest.TestStep Test, string description, string reference, string result, int ID)
        {
            Test.Device = "Sens.";
            Test.ID = ID++;
            Test.Description = description;
            Test.Reference = reference;
            Test.Result = result;
            Verificator.TestList.Add(Test);
            TestRunning = VerificatorTests.None;
            return ID;
        }

        private int Notifica_Log(int ID, string device, RisultatiTest.TestStep Test, string reference, string reading, RisultatiTest.Esito result)
        {
            Test.Device = device;
            Test.Reference = reference;
            Test.ID = ID++;
            Test.Reading = reading;
            switch (result)
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
            return ID;
        }

        private void Refresh_Log()
        {
            Verificator.TestView.Clear();
            Verificator.TestList.ForEach(p => Verificator.TestView.Add(p));
            STestStep = SensorTestStep.Ping;
        }

        Brush LedTest(RisultatiTest.Esito res)
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
            Check4mA.Fill   = LedTest(RisultatiTest.AnOut.I_4mA);
            Check12mA.Fill  = LedTest(RisultatiTest.AnOut.I_12mA);
            Check20mA.Fill  = LedTest(RisultatiTest.AnOut.I_20mA);

            ZeroEnergyCheck.Fill    = LedTest(RisultatiTest.ICoil.Zero);
            NegCheck.Fill           = LedTest(RisultatiTest.ICoil.Neg);
            PosCheck.Fill           = LedTest(RisultatiTest.ICoil.Pos);

            ZeroCheck.Fill      = LedTest(RisultatiTest.Simulazione.Zero);
            MidCheck.Fill       = LedTest(RisultatiTest.Simulazione.LO);
            HiCheck.Fill        = LedTest(RisultatiTest.Simulazione.Hi);            
            EPipeCheck.Fill     = LedTest(RisultatiTest.Simulazione.EpipeTest);

            ProgInCheck.Fill    = LedTest(RisultatiTest.IO.GP_IN_on);
            ProgOutCheck.Fill   = LedTest(RisultatiTest.IO.GP_OUT_on);
            PulseCheck.Fill     = LedTest(RisultatiTest.IO.GP_PULSE_on);
            FreqCheck.Fill      = LedTest(RisultatiTest.IO.GP_FREQ_on);

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
            if (auto)
            {
                Verificator.TestView.Clear();
                Verificator.TestList.Clear();
                ReportTestGrid.ItemsSource = null;
                ReportTestGrid.ItemsSource = Verificator.TestView;
            }
            else
            {
                var logdialog = new MessageDialog("Are you sure?", "Clear Logs");
                logdialog.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
                logdialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });
                var logres = await logdialog.ShowAsync();

                if ((int)logres.Id == 0)
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
            if (Verificator.TestList.Count != 0)
            {
                string LogLineStr;
                char[] LogLineArray;
                string FileName = "TestLog_" + MC608.Convertitore.Matricola + "_" + MC608.Sensore.Matricola + "_" + DateTime.Now.ToString("MM_dd_yyyy_hh_mm") + ".log";

                LogLineStr = MC608.Convertitore.Matricola + ";" + MC608.Sensore.Matricola + ";" + DateTime.Now.ToString("yyyy/MM/dd hh:mm") + "\n";
                LogLineArray = LogLineStr.ToCharArray();
                await BinaryStorage.Append(FileName, FileManager.CurrentFolder.Path, LogLineArray);

                for (int i = 0; i < Verificator.TestList.Count; i++)
                {
                    LogLineStr = Verificator.TestList[i].ID.ToString() + ";" +
                                    Verificator.TestList[i].Device + ";" +
                                    Verificator.TestList[i].Description + ";" +
                                    Verificator.TestList[i].Reference + ";" +
                                    Verificator.TestList[i].Reading + ";" +
                                    Verificator.TestList[i].Result + "\n";

                    LogLineArray = LogLineStr.ToCharArray();

                    await BinaryStorage.Append(FileName, FileManager.CurrentFolder.Path, LogLineArray);
                }

                var dialog = new MessageDialog("Log File saved", "Log File");
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

            NewReport.Data_Test             = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            NewReport.OperatoreTest         = OperatorName.Text;

            NewReport.Modello_Sensore = Customer.SensorModel(MC608.Sensore.Modello);
            NewReport.Matricola_Sensore = MC608.Sensore.Matricola;
            NewReport.Modello_Convertitore = Customer.ConverterModel(MC608.Convertitore.Modello);
            NewReport.Matricola_Convertitore = MC608.Convertitore.Matricola;
            NewReport.KA = MC608.Taratura.KA1_main.ToString("#.0");
            NewReport.FondoScala = MC608.Portata.Full_Scale_M3h.ToString("#.00") + " m3/h";
            NewReport.Impulsi = MC608.Impulsi.Volume_ml.ToString() + " ml";


            //Convertitore****************************************************
            NewReport.AnalogOut = VerifyTest(RisultatiTest.AnOut.Esito);

            NewReport.Simulation = VerifyTest(RisultatiTest.Simulazione.Esito);
            if ((RisultatiTest.Simulazione.Esito == RisultatiTest.Esito.PASS_Verify) || (RisultatiTest.Simulazione.Esito == RisultatiTest.Esito.PASS_Def))
            {
                NewReport.Zero_read = RisultatiTest.Simulazione.Zero_read;
                NewReport.Hi_read = RisultatiTest.Simulazione.Hi_read;
                NewReport.LO_read = RisultatiTest.Simulazione.LO_read;
            }
            else
            {
                NewReport.Zero_read = 0;
                NewReport.Hi_read = 0;
                NewReport.LO_read = 0;
            }

            NewReport.EmptyPype = VerifyTest(RisultatiTest.Simulazione.EmptyPype);

            NewReport.EnergyCoil = VerifyTest(RisultatiTest.ICoil.Esito);
            if ((RisultatiTest.ICoil.Esito == RisultatiTest.Esito.PASS_Verify) || (RisultatiTest.ICoil.Esito == RisultatiTest.Esito.PASS_Def))
            {
                NewReport.ICoil_Read = RisultatiTest.ICoil.Pos_I;
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
            if (RisultatiTest.Sensore.dry)
            {
                NewReport.IsolationTC = VerifyTest(RisultatiTest.Sensore.RH_TC);
                NewReport.IsolationDC = VerifyTest(RisultatiTest.Sensore.RH_DC);
                NewReport.IsolationEC = VerifyTest(RisultatiTest.Sensore.RH_EC);
                NewReport.TestType = "DRY";
            }
            else
            {
                NewReport.IsolationTC = "N.E.";
                NewReport.IsolationDC = "N.E.";
                NewReport.IsolationEC = "N.E.";
                NewReport.TestType = "WET";
            }
            //****************************************************************

            NewReport.Company = CompanyName.Text;
            NewReport.CompanyLocation = CompanyLocation.Text;
            NewReport.Customer = CustomerName.Text;
            NewReport.CustomerLocation = CustomerLocation.Text;
            NewReport.Note = Note.Text;
            NewReport.SN_Verificator = RAM_VerifConfiguration.SN_Verificator;
            NewReport.SW_Ver_Verificator = RAM_VerifConfiguration.SW_Ver_Verificator;
            NewReport.DataCalibrazione = RAM_VerifConfiguration.DataLastTaratura;
            NewReport.NuovaCalibrazione = RAM_VerifConfiguration.DataNextTaratura;

            Verificator.ReportList.Add(NewReport);

            DataAccess.AddData("MC_Suite_DataBase.db", NewReport);

            Verificator.ReportView.Clear();
            Verificator.ReportList.ForEach(p => Verificator.ReportView.Add(p));

            var dialog = new MessageDialog("Report #" + NewReport.ID.ToString() + " added to Database\n" +
                                           "Sensor ID: " + NewReport.Matricola_Sensore + "\n" +
                                           "Converter ID : " + NewReport.Matricola_Convertitore
                                           , "Report Saved");
            await dialog.ShowAsync();

            CheckDatabaseTest(false);
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

        private void SaveConfig()
        {
            //Salvo la Configurazione
            RAM_Configuration.Operator  = OperatorName.Text;
            RAM_Configuration.Company   = CompanyName.Text;
            RAM_Configuration.Company_Location = CompanyLocation.Text;
            RAM_Configuration.Customer  = CustomerName.Text;
            RAM_Configuration.Customer_Location = CustomerLocation.Text;
            RAM_Configuration.Note      = Note.Text;

            List<Configuration> NewCfg = new List<Configuration>();
            NewCfg.Add(RAM_Configuration);
            SerializableStorage<Configuration>.Save(FileManager.ConfigFile, FileManager.MainFolder.Path, NewCfg);
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
