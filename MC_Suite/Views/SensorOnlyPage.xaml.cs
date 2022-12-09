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

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class SensorOnlyPage : Page
    {
        public SensorOnlyPage()
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

            ConncetionRing.Visibility = Visibility.Collapsed;
            InitSensorPingTimer();
            RisultatiTest.Init_Sensore();
            DisableSensorTest();

            if (MbConnection.IsOpen)
                MbConnection.Close();

            if (Simulator.IsOpen)
            {
                SimulatorState.Fill = new SolidColorBrush(Colors.LimeGreen);
                EnableSensorTest();
            }
            else
            {
                OpenSimulatorCom();
            }            
        }

        private async void OpenSimulatorCom()
        {
            //Apro comunicazione con il Simulatore
            if (ComSetup.SimulatorComPort.ID == null)
            {
                var dialog = new MessageDialog("Simulator COM Port Not Found", "COM Port Error");
                await dialog.ShowAsync();
                DisableSensorTest();
            }
            else
            {
                if (await Simulator.Open())
                {
                    Simulator.SendCommand(Simulator.portHandler, SimulatorCOMPortManager.CMD.Ping, 5);
                    Simulator.CommandCompleted += Sensor_CommandCompleted;
                    STestStep = SensorTestStep.Ping;
                    EnableSensorTest();
                }
            }
        }

        private enum VerificatorTests
        {
            None,
            Conv_Simulation,
            Conv_EnergyCoil,
            Conv_IO,
            Sensor
        }

        #region Abilitazioni

        private void DisableSensorTest()
        {
            TestDryBtn.IsEnabled = false;
            TestWetBtn.IsEnabled = false;
            AbortSensTestBtn.IsEnabled = true;
        }

        private void EnableSensorTest()
        {
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

        public TargetVariablesFields Fields
        {
            get
            {
                return TargetVariablesFields.Instance;
            }
        }

        public SimulatorCOMPortManager Simulator
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

        public MbCOMPortManager MbConnection
        {
            get
            {
                return MbCOMPortManager.Instance;
            }
        }

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
                        param => OpenSimulatorCOM()
                            );
                }
                return _openSimulatorCOMcmd;
            }
        }

        private async void OpenSimulatorCOM()
        {
            if (ComSetup.SimulatorComPort.ID == null)
            {
                var dialog = new MessageDialog("Simulator COM Port Not Found", "COM Port Error");
                await dialog.ShowAsync();
            }
            else
            {
                if (await Simulator.Open())
                {
                    var dialog = new MessageDialog("Simulator COM Port Opened", "COM Port State");
                    await dialog.ShowAsync();

                    Simulator.SendCommand(Simulator.portHandler, SimulatorCOMPortManager.CMD.Ping, 5);
                    Simulator.CommandCompleted += Sensor_CommandCompleted;
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

        private void TestSensorDry()
        {
            ClearSensorResults();
            RisultatiTest.Sensore.dry = true;
            SensorTestRing.Visibility = Visibility.Visible;
            Simulator.SendCommand(Simulator.portHandler, SimulatorCOMPortManager.CMD.DryTest, 5);
            Simulator.CommandCompleted += Sensor_CommandCompleted;
            STestStep = SensorTestStep.Start;
            DisableSensorTest();
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

        private void TestSensorWet()
        {
            ClearSensorResults();
            RisultatiTest.Sensore.dry = false;
            SensorTestRing.Visibility = Visibility.Visible;
            Simulator.SendCommand(Simulator.portHandler, SimulatorCOMPortManager.CMD.DryTest, 5);
            Simulator.CommandCompleted += Sensor_CommandCompleted;
            STestStep = SensorTestStep.Start;
            DisableSensorTest();
        }

        private void StartDryTest()
        {
            Simulator.SendCommand(Simulator.portHandler, SimulatorCOMPortManager.CMD.DryTest, 5);
            Simulator.CommandCompleted += Sensor_CommandCompleted;
        }

        private void SensorWait()
        {
            Simulator.SendCommand(Simulator.portHandler, SimulatorCOMPortManager.CMD.Ping, 5);
            Simulator.CommandCompleted += Sensor_CommandCompleted;
        }

        private void SensorGetResults()
        {
            Simulator.SendCommand(Simulator.portHandler, SimulatorCOMPortManager.CMD.ReadResults, 5);
            Simulator.CommandCompleted += Sensor_CommandCompleted;
        }

        private void Sensor_CommandCompleted(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SimulatorCOMPortManager CMD = sender as SimulatorCOMPortManager;

            if (SensorPingTimer.IsEnabled == false)
            {
                if (CMD.CommandResult.Result == SimulatorCOMPortManager.SimCommandResult.Success)
                {
                    ConncetionRing.Visibility = Visibility.Collapsed;

                    if (CMD.CommandResult.Answer == SimulatorCOMPortManager.ANSWER.Test_Error)
                        STestStep = SensorTestStep.Error;
                    else
                    {
                        switch (STestStep)
                        {
                            case SensorTestStep.Ping:
                                //STestStep = SensorTestStep.Start;
                                EnableSensorTest();
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
            /*Raw0.Text = AnalogMeasures.ADC_Module.RawMeasures[AnalogsService.Out4_20mA].ToString("#.00");
            Raw1.Text = AnalogMeasures.ADC_Module.RawMeasures[AnalogsService.Icoil].ToString("#.00");
            Raw2.Text = AnalogMeasures.ADC_Module.RawMeasures[AnalogsService.VAlim].ToString("#.00");

            Media0.Text = AnalogMeasures.ADC_Module.RawMedia[AnalogsService.Out4_20mA].ToString("#.00");
            Media1.Text = AnalogMeasures.ADC_Module.RawMedia[AnalogsService.Icoil].ToString("#.00");
            Media2.Text = AnalogMeasures.ADC_Module.RawMedia[AnalogsService.VAlim].ToString("#.00");

            ConvRaw0.Text = AnalogMeasures.ADC_Module.CovertIst[AnalogsService.Out4_20mA].ToString("#.00");
            ConvRaw1.Text = AnalogMeasures.ADC_Module.CovertIst[AnalogsService.Icoil].ToString("#.00");
            ConvRaw2.Text = AnalogMeasures.ADC_Module.CovertIst[AnalogsService.VAlim].ToString("#.00");

            ConvMedia0.Text = AnalogMeasures.ADC_Module.CovertMedia[AnalogsService.Out4_20mA].ToString("#.00");
            ConvMedia1.Text = AnalogMeasures.ADC_Module.CovertMedia[AnalogsService.Icoil].ToString("#.00");
            ConvMedia2.Text = AnalogMeasures.ADC_Module.CovertMedia[AnalogsService.VAlim].ToString("#.00");*/
        }

        #endregion

        #region Common Resources

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
                        param => ClearLog()
                            );
                }
                return _clearLogCmd;
            }
        }

        private async void ClearLog()
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
                string FileName = "TEST_LOG_" + Fields.ConverterId.ValAsString + "_" + Fields.SensorId.ValAsString + DateTime.Now.ToString("yyyyMMddhhmmss") + ".log";

                LogLineStr = Fields.ConverterId.ValAsString + ";" + Fields.SensorId.ValAsString + ";" + DateTime.Now.ToString("yyyy/MM/dd hh:mm") + "\n";
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

            NewReport.Data_Test = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            NewReport.OperatoreTest = OperatorName.Text;

            NewReport.Modello_Sensore = ManSensorModel.Text;
            NewReport.Matricola_Sensore = ManSensorId.Text;
            NewReport.Modello_Convertitore = "--";
            NewReport.Matricola_Convertitore = "--";
            NewReport.KA = "--";
            NewReport.FondoScala = "--";
            NewReport.Impulsi = "--";

            //Convertitore****************************************************
            NewReport.AnalogOut = "--";
            NewReport.Simulation = VerifyTest(RisultatiTest.Simulazione.Esito);
            if (RisultatiTest.Simulazione.Esito == RisultatiTest.Esito.PASS_Verify)
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
            if (RisultatiTest.ICoil.Esito == RisultatiTest.Esito.PASS_Verify)
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

            if (RisultatiTest.Sensore.RH_AC == RisultatiTest.Esito.DaEseguire)
            {
                NewReport.IsolationTC = "N.E.";
                NewReport.IsolationDC = "N.E.";
                NewReport.IsolationEC = "N.E.";
                NewReport.TestType = "--";
            }
            else if (RisultatiTest.Sensore.dry)
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
            RAM_Configuration.Operator = OperatorName.Text;
            RAM_Configuration.Company = CompanyName.Text;
            RAM_Configuration.Company_Location = CompanyLocation.Text;
            RAM_Configuration.Customer = CustomerName.Text;
            RAM_Configuration.Customer_Location = CustomerLocation.Text;
            RAM_Configuration.Note = Note.Text;

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

