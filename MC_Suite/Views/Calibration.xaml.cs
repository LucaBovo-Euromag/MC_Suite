using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
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

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class Calibration : Page
    {
        public Calibration()
        {
            this.InitializeComponent();

            InitADC();

            if (RAM_VerifConfiguration.TarMode == true)
            {
                FactoryCalibPivot.Visibility = Visibility.Visible;
                FactoryCalibPivot.Header = "Factory Calibration";
            }
            else
            { 
                FactoryCalibPivot.Visibility = Visibility.Collapsed;
                FactoryCalibPivot.Header = "";
            }

            SerialNumber.Text = RAM_VerifConfiguration.SN_Verificator;
            FSerialNumber.Text = RAM_VerifConfiguration.SN_Verificator;

            Autoset_0.IsEnabled = true;
            GetPoint1_0.IsEnabled = false;
            GetPoint2_0.IsEnabled = false;
            GetPoint3_0.IsEnabled = false;

            Autoset_1.IsEnabled = true;
            GetPoint1_1.IsEnabled = false;
            GetPoint2_1.IsEnabled = false;

            InitTest420mATimer();
            InitTestIcoilTimer();

            Gain_Coil_Bkup = RAM_VerifConfiguration.Icoil_Gain;
            Offset_CoilBkup = RAM_VerifConfiguration.Icoil_Offs;
            Gain_Valim_Bkup = RAM_VerifConfiguration.VAlim_Gain;
            Offset_Valim_Bkup = RAM_VerifConfiguration.VAlim_Offs;
            Vbattery100_Bkup = RAM_VerifConfiguration.Vbattery100;
            Vbattery0_Bkup = RAM_VerifConfiguration.Vbattery0;
            Gain_420mA_Bkup = RAM_VerifConfiguration.Out4_20mA_Gain;
            Offset_420mA_Bkup = RAM_VerifConfiguration.Out4_20mA_Offs;
            Gain_412mA_Bkup = RAM_VerifConfiguration.Out4_12mA_Gain;
            Offset_412mA_Bkup = RAM_VerifConfiguration.Out4_12mA_Offs;

            ConncetionRing.Visibility = Visibility.Collapsed;
            if (MbConnection.IsOpen)
            {
                ConverterState.Fill = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                ConverterState.Fill = new SolidColorBrush(Colors.Red);
            }

            NewTarIcoil.Content = "Start";
            FtarIcoilState = FtarIcoilStates.Start;

            ResetTar420mA();
        }

        private async void InitADC()
        {
            GPioState.Fill = GPIO_Control.InitGPIO();

            if (AnalogMeasures.ADC_ModuleIsReady == false)
                await AnalogMeasures.ADC_Module_Open();

            if (AnalogMeasures.ADC_Module.Ready)
            {
                AnalogMeasures.ADC_MeasuresStart();
                RefreshAnalogsStart();
            }
            else
                ADC_Message.Text = ADC_Module.Status;
        }


        public ADS1115 ADC_Module
        {
            get
            {
                return ADS1115.Instance;
            }
            set {; }
        }

        #region Istanze

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

        public Storage FileManager
        {
            get
            {
                return Storage.Instance;
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

        public Settings ComSetup
        {
            get
            {
                return Settings.Instance;
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

        #region Refresh Misure

        private DispatcherTimer RefreshAnalogsTimer;
        private void RefreshAnalogsStart()
        {
            RefreshAnalogsTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
            RefreshAnalogsTimer.Tick += RefreshAnalogsTimer_Tick;
            RefreshAnalogsTimer.Start();
        }

        private void RefreshAnalogsTimer_Tick(object sender, object e)
        {
            #region Quick

            Raw0.Text = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].MisuraRaw.ToString("#.00");
            Raw1.Text = AnalogMeasures.Analogiche[AnalogsService.Icoil].MisuraRaw.ToString("#.00");
            Raw2.Text = AnalogMeasures.Analogiche[AnalogsService.VAlim].MisuraRaw.ToString("#.00");

            ConvRaw0.Text = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].Misura.ToString("#.00");
            ConvRaw1.Text = AnalogMeasures.Analogiche[AnalogsService.Icoil].Misura.ToString("#.00");
            ConvRaw2.Text = AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura.ToString("#.00");

            Gain0.Text = RAM_VerifConfiguration.Out4_20mA_Gain.ToString("#.00");
            Gain1.Text = RAM_VerifConfiguration.Icoil_Gain.ToString("#.00");
            Gain2.Text = RAM_VerifConfiguration.VAlim_Gain.ToString("#.00");

            Offset0.Text = RAM_VerifConfiguration.Out4_20mA_Offs.ToString("#.00");
            Offset1.Text = RAM_VerifConfiguration.Icoil_Offs.ToString("#.00");
            Offset2.Text = RAM_VerifConfiguration.VAlim_Offs.ToString("#.00");

            #endregion

            #region Factory

            FRaw0.Text = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].MisuraRaw.ToString("#.00");
            FRaw1.Text = AnalogMeasures.Analogiche[AnalogsService.Icoil].MisuraRaw.ToString("#.00");
            FRaw2.Text = AnalogMeasures.Analogiche[AnalogsService.VAlim].MisuraRaw.ToString("#.00");

            FConvRaw0.Text = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].Misura.ToString("#.00");
            FConvRaw1.Text = AnalogMeasures.Analogiche[AnalogsService.Icoil].Misura.ToString("#.00");
            FConvRaw2.Text = AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura.ToString("#.00");

            FGain0.Text = RAM_VerifConfiguration.Out4_20mA_Gain.ToString("#.00");
            FGain1.Text = RAM_VerifConfiguration.Icoil_Gain.ToString("#.00");
            FGain2.Text = RAM_VerifConfiguration.VAlim_Gain.ToString("#.00");

            FOffset0.Text = RAM_VerifConfiguration.Out4_20mA_Offs.ToString("#.00");
            FOffset1.Text = RAM_VerifConfiguration.Icoil_Offs.ToString("#.00");
            FOffset2.Text = RAM_VerifConfiguration.VAlim_Offs.ToString("#.00");

            #endregion

        }

        #endregion

        #region Autotaratura Corrente Bobine

        private double X_Off, X_100mA, Y_Off, Y_100mA;
        private double Gain_Coil, Offset_Coil;
        private float Gain_Coil_Bkup, Offset_CoilBkup;
        private bool Point1_1_Ready, Point2_1_Ready;

        private static AutoTarICoilStep AutoTarICoil_Step;
        public enum AutoTarICoilStep
        {
            Start,
            SetON,
            SetOFF,
            Set53mA,
            Set100mA,
            SetTara,
            End
        }

        private DispatcherTimer AutoTarICoilTimer;
        private void InitTestIcoilTimer()
        {
            AutoTarICoilTimer = new DispatcherTimer();
            AutoTarICoilTimer.Interval = TimeSpan.FromMilliseconds(1000);
            AutoTarICoilTimer.Tick += AutoTarICoilTimer_Tick;
            AutoTarICoilTimer.Stop();
        }

        const byte INV_COIL = 0x01;
        const byte ON_COIL = 0x10;
        const byte OFF_COIL = 0x00;

        /*private void AutoTarICoilTimer_Tick(object sender, object e)
        {
            switch(AutoTarICoil_Step)
            {
                case AutoTarICoilStep.Start:
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 1, 3);
                    MbConnection.SendCommandCompleted += AutoTarICoil_SendCommandCompleted;
                    AutoTarICoilTimer.Stop();
                    NextStepIcoilReady = true;
                    break;
                case AutoTarICoilStep.SetOFF:
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ACT_COIL_set, OFF_COIL, 3);                    
                    MbConnection.SendCommandCompleted += AutoTarICoil_SendCommandCompleted;
                    AutoTarICoilTimer.Stop();
                    NextStepIcoilReady = true;
                    break;
                case AutoTarICoilStep.Set100mA:
                    X_Off = AnalogMeasures.Analogiche[AnalogsService.Icoil].MisuraRaw;
                    
                    if (Point1_1_Ready)
                    {
                        Y_Off = Convert.ToDouble(Target1.Text);
                        MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ACT_COIL_set, ON_COIL + INV_COIL, 3);
                        MbConnection.SendCommandCompleted += AutoTarICoil_SendCommandCompleted;
                        AutoTarICoilTimer.Stop();
                        NextStepIcoilReady = true;                        
                    }
                    break;
                case AutoTarICoilStep.SetTara:
                    X_100mA = AnalogMeasures.Analogiche[AnalogsService.Icoil].MisuraRaw;
                    
                    if (Point2_1_Ready)
                    {
                        Y_100mA = Convert.ToDouble(Target1.Text);

                        Gain_Coil = (Y_100mA - Y_Off) / (X_100mA - X_Off);
                        Offset_Coil = ((X_Off * Y_100mA) - (X_100mA * Y_Off)) / (X_100mA - X_Off);

                        RAM_VerifConfiguration.Icoil_Gain = (float)Gain_Coil;
                        RAM_VerifConfiguration.Icoil_Offs = (float)Offset_Coil;
                        AutoTarICoil_Step = AutoTarICoilStep.End;
                    }
                    break;
                case AutoTarICoilStep.End:
                    GetPoint1_1.IsEnabled = false;
                    GetPoint2_1.IsEnabled = false;
                    Autoset_1.IsEnabled   = true;
                    AnalogMeasures.AggiornaGainOffset();
                    MbConnection.SendCommandCompleted -= AutoTarICoil_SendCommandCompleted;
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 0, 3);
                    AutoTarICoilTimer.Stop();
                    break;
            }
        }*/

        private void AutoTarICoilTimer_Tick(object sender, object e)
        {
            switch(AutoTarICoil_Step)
            {
                case AutoTarICoilStep.Start:
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 1, 3);
                    MbConnection.SendCommandCompleted += AutoTarICoil_SendCommandCompleted;
                    AutoTarICoilTimer.Stop();
                    NextStepIcoilReady = true;
                    break;
                case AutoTarICoilStep.SetON:
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ACT_COIL_set, ON_COIL + INV_COIL, 3);
                    MbConnection.SendCommandCompleted += AutoTarICoil_SendCommandCompleted;
                    AutoTarICoilTimer.Stop();
                    NextStepIcoilReady = true;
                    break;
            }
        }

        private bool NextStepIcoilReady;
        /*private void AutoTarICoil_SendCommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;

            if (cmd.SendCommand_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                if (NextStepIcoilReady)
                {
                    NextStepIcoilReady = false;
                    switch (AutoTarICoil_Step)
                    {
                        case AutoTarICoilStep.Start:
                            AutoTarICoil_Step = AutoTarICoilStep.SetOFF;
                            break;
                        case AutoTarICoilStep.SetOFF:
                            AutoTarICoil_Step = AutoTarICoilStep.Set100mA;
                            Target1.Text = "0.0";
                            GetPoint1_1.IsEnabled = true;
                            break;
                        case AutoTarICoilStep.Set100mA:
                            AutoTarICoil_Step = AutoTarICoilStep.SetTara;
                            Target1.Text = "100.0";
                            GetPoint2_1.IsEnabled = true;
                            break;
                        case AutoTarICoilStep.SetTara:
                            AutoTarICoil_Step = AutoTarICoilStep.End;
                            Target1.Text = "";
                            break;
                        case AutoTarICoilStep.End:
                            break;
                    }
                    AutoTarICoilTimer.Start();
                }
            }
        }*/

        private void AutoTarICoil_SendCommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;

            if (cmd.SendCommand_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                if (NextStepIcoilReady)
                {
                    NextStepIcoilReady = false;
                    switch (AutoTarICoil_Step)
                    {
                        case AutoTarICoilStep.Start:
                            AutoTarICoil_Step = AutoTarICoilStep.SetON;
                            AutoTarICoilTimer.Start();
                            break;
                        case AutoTarICoilStep.SetON:
                            Target1.Text = "0.00";
                            GetPoint1_1.IsEnabled = true;
                            GetPoint2_1.IsEnabled = true;
                            break;                       
                    }                    
                }
            }
        }

        private void GetPoint1_1_Click(object sender, RoutedEventArgs e)
        {
            Autoset_1.IsEnabled = true;
            GetPoint1_1.IsEnabled = false;
            GetPoint2_1.IsEnabled = false;
            SetIcoil.IsEnabled = true;

            X_Off = AnalogMeasures.Analogiche[AnalogsService.Icoil].MisuraRaw;
            Y_Off = Convert.ToDouble(Target1.Text);
            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 0, 3);
        }

        private void GetPoint2_1_Click(object sender, RoutedEventArgs e)
        {
            Autoset_1.IsEnabled = true;
            GetPoint1_1.IsEnabled = false;
            GetPoint2_1.IsEnabled = false;
            SetIcoil.IsEnabled = true;

            X_100mA = AnalogMeasures.Analogiche[AnalogsService.Icoil].MisuraRaw;
            Y_100mA = Convert.ToDouble(Target1.Text);
            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 0, 3);
        }

        private void SetIcoil_Click(object sender, RoutedEventArgs e)
        {
            GetPoint1_1.IsEnabled = false;
            GetPoint2_1.IsEnabled = false;

            Gain_Coil = (Y_100mA - Y_Off) / (X_100mA - X_Off);
            Offset_Coil = ((X_Off * Y_100mA) - (X_100mA * Y_Off)) / (X_100mA - X_Off);

            RAM_VerifConfiguration.Icoil_Gain = (float)Gain_Coil;
            RAM_VerifConfiguration.Icoil_Offs = (float)Offset_Coil;
            AnalogMeasures.AggiornaGainOffset();
            SetIcoil.IsEnabled = false;
        }

        private void Autoset_1_Click(object sender, RoutedEventArgs e)
        {
            Point1_1_Ready = false;
            Point2_1_Ready = false;

            AutoTarICoil_Step = AutoTarICoilStep.Start;            

            AutoTarICoilTimer.Start();

            Autoset_1.IsEnabled     = false;
            GetPoint1_1.IsEnabled   = false;
            GetPoint2_1.IsEnabled   = false;
            SetIcoil.IsEnabled      = false;
        }

        #endregion

        #region Autotaratura 4-20 mA

        private double X_4mA, X_12mA, X_20mA, Y_4mA, Y_12mA, Y_20mA;
        private double Gain_420mA, Offset_420mA, Gain_412mA, Offset_412mA;
        private float Gain_420mA_Bkup, Offset_420mA_Bkup, Gain_412mA_Bkup, Offset_412mA_Bkup;
        private bool Point1_0_Ready, Point2_0_Ready, Point3_0_Ready;

        private static AutoTar420mAStep AutoTar420mA_Step;
        public enum AutoTar420mAStep
        {
            Start   = 0,
            Set4mA  = 1,
            Set12mA = 2,
            Set20mA = 3,
            SetTara = 4,
            End     = 5
        }

        private DispatcherTimer AutoTar420mATimer;
        private void InitTest420mATimer()
        {
            AutoTar420mATimer = new DispatcherTimer();
            AutoTar420mATimer.Interval = TimeSpan.FromMilliseconds(1000);
            AutoTar420mATimer.Tick += AutoTar420mATimer_Tick;
            AutoTar420mATimer.Stop();
        }

        private void GetPoint1_0_Click(object sender, RoutedEventArgs e)
        {
            Point1_0_Ready = true;
            GetPoint1_0.IsEnabled = false;
        }

        private void GetPoint2_0_Click(object sender, RoutedEventArgs e)
        {
            Point2_0_Ready = true;
            GetPoint2_0.IsEnabled = false;
        }

        private void GetPoint3_0_Click(object sender, RoutedEventArgs e)
        {
            Point3_0_Ready = true;
            GetPoint3_0.IsEnabled = false;
        }

        private void RestartBtn_Click(object sender, RoutedEventArgs e)
        {
            AnalogMeasures.ADC_Restart();
        }

        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            //Chiudo connessione precedente
            if (MbConnection.IsOpen)
            {
                MbConnection.Close();
                ConverterState.Fill = new SolidColorBrush(Colors.Red);
            }
            //Riapro connessione
            if (await MbConnection.Open(ComSetup.ComPort608))
            {
                MC608.Reset();
                MC608.Tipo_Connessione = MC608.Comunicazione.Connesso_ModBus;
                ConverterState.Fill = new SolidColorBrush(Colors.Yellow);
                ConncetionRing.Visibility = Visibility.Visible;
                ReadRegister();
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

        private void MbConnection_ReadRegistersCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;
            if (cmd.ReadRegisters_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                ConverterState.Fill = new SolidColorBrush(Colors.LimeGreen);
                ConncetionRing.Visibility = Visibility.Collapsed;
            }
            ConncetionRing.Visibility = Visibility.Collapsed;
        }

        private void Autoset_0_Click(object sender, RoutedEventArgs e)
        {
            if (MC608.Release_HW.Versione == 6 && MC608.Release_HW.Revisione >= 3)
            {
                GPIO_Control.SetVAux();
                GPIO_Control.ResRL_4_20mA();
            }
            else
            {
                GPIO_Control.ResVAux();
                GPIO_Control.SetRL_4_20mA();
            }

            AutoTar420mA_Step = AutoTar420mAStep.Start;
            Point1_0_Ready = false;
            Point2_0_Ready = false;
            Point3_0_Ready = false;
            AutoTar420mATimer.Start();

            Autoset_0.IsEnabled = false;
            GetPoint1_0.IsEnabled = false;
            GetPoint2_0.IsEnabled = false;
            GetPoint3_0.IsEnabled = false;
        }

        private void AutoTar420mATimer_Tick(object sender, object e)
        {
            switch (AutoTar420mA_Step)
            {
                case AutoTar420mAStep.Start:
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.TEST_MODE, 0, 3);
                    MbConnection.SendCommandCompleted += AutoTar420mA_SendCommandCompleted;
                    AutoTar420mATimer.Stop();
                    NextStep420mAReady = true;
                    break;
                case AutoTar420mAStep.Set4mA:
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.ENABLE_SIMUL, 0, 3);
                    MbConnection.SendCommandCompleted += AutoTar420mA_SendCommandCompleted;
                    AutoTar420mATimer.Stop();
                    NextStep420mAReady = true;
                    break;
                case AutoTar420mAStep.Set12mA:
                    X_4mA = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].MisuraRaw;

                    if (Point1_0_Ready)
                    {
                        Y_4mA = Convert.ToDouble(Target0.Text);
                        MbConnection.SendCommand(ComSetup.Address, Map.Comandi.ENABLE_SIMUL, 50, 3);
                        MbConnection.SendCommandCompleted += AutoTar420mA_SendCommandCompleted;
                        AutoTar420mATimer.Stop();
                        NextStep420mAReady = true;
                        GetPoint1_0.IsEnabled = false;
                    }
                    break;
                case AutoTar420mAStep.Set20mA:
                    X_12mA = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].MisuraRaw;

                    if (Point2_0_Ready)
                    {
                        Y_12mA = Convert.ToDouble(Target0.Text);

                        Gain_412mA = (Y_12mA - Y_4mA) / (X_12mA - X_4mA);
                        Offset_412mA = ((X_4mA * Y_12mA) - (X_12mA * Y_4mA)) / (X_12mA - X_4mA);

                        RAM_VerifConfiguration.Out4_12mA_Gain = (float)Gain_412mA;
                        RAM_VerifConfiguration.Out4_12mA_Offs = (float)Offset_412mA;

                        MbConnection.SendCommand(ComSetup.Address, Map.Comandi.ENABLE_SIMUL, 100, 3);
                        MbConnection.SendCommandCompleted += AutoTar420mA_SendCommandCompleted;
                        AutoTar420mATimer.Stop();
                        NextStep420mAReady = true;
                        GetPoint2_0.IsEnabled = false;
                    }
                    break;
                case AutoTar420mAStep.SetTara:
                    X_20mA = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].MisuraRaw;

                    GetPoint3_0.IsEnabled = true;

                    if (Point3_0_Ready)
                    {
                        Y_20mA = Convert.ToDouble(Target0.Text);

                        Gain_420mA = (Y_20mA - Y_4mA) / (X_20mA - X_4mA);
                        Offset_420mA = ((X_4mA * Y_20mA) - (X_20mA * Y_4mA)) / (X_20mA - X_4mA);

                        RAM_VerifConfiguration.Out4_20mA_Gain = (float)Gain_420mA;
                        RAM_VerifConfiguration.Out4_20mA_Offs = (float)Offset_420mA;
                        AutoTar420mA_Step = AutoTar420mAStep.End;
                        NextStep420mAReady = true;
                        GetPoint3_0.IsEnabled = false;
                    }
                    break;
                case AutoTar420mAStep.End:
                    AnalogMeasures.AggiornaGainOffset();
                    AutoTar420mATimer.Stop();
                    Autoset_0.IsEnabled = true;
                    MbConnection.SendCommandCompleted -= AutoTar420mA_SendCommandCompleted;
                    MbConnection.SendCommand(ComSetup.Address, Map.Comandi.DISABLE_SIMUL, 0, 3);
                    break;
            }
        }

        private bool NextStep420mAReady;
        private void AutoTar420mA_SendCommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;

            if (cmd.SendCommand_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                if (NextStep420mAReady)
                {
                    NextStep420mAReady = false;
                    switch (AutoTar420mA_Step)
                    {
                        case AutoTar420mAStep.Start:
                            AutoTar420mA_Step = AutoTar420mAStep.Set4mA;
                            break;
                        case AutoTar420mAStep.Set4mA:
                            AutoTar420mA_Step = AutoTar420mAStep.Set12mA;
                            GetPoint1_0.IsEnabled = true;
                            Target0.Text = "4.0";
                            break;
                        case AutoTar420mAStep.Set12mA:
                            AutoTar420mA_Step = AutoTar420mAStep.Set20mA;
                            GetPoint2_0.IsEnabled = true;
                            Target0.Text = "12.0";
                            break;
                        case AutoTar420mAStep.Set20mA:
                            AutoTar420mA_Step = AutoTar420mAStep.SetTara;
                            GetPoint3_0.IsEnabled = true;
                            Target0.Text = "20.0";
                            break;
                        case AutoTar420mAStep.SetTara:
                            AutoTar420mA_Step = AutoTar420mAStep.End;
                            break;
                        case AutoTar420mAStep.End:
                            break;
                    }
                    AutoTar420mATimer.Start();
                }
            }
        }

        #endregion

        #region Taratura Tensione Alimentazione

        private double X_5V, X_12V, Y_5V, Y_12V;
        private double Gain_Valim, Offset_Valim;

        private float Gain_Valim_Bkup, Offset_Valim_Bkup, Vbattery100_Bkup, Vbattery0_Bkup;
        private void GetPoint1_2_Click(object sender, RoutedEventArgs e)
        {
            GetPoint1_2.Content = AnalogMeasures.Analogiche[AnalogsService.VAlim].MisuraRaw.ToString("#.00");
            Y_5V = Convert.ToDouble(Target2.Text);
            X_5V = AnalogMeasures.Analogiche[AnalogsService.VAlim].MisuraRaw;
        }
         
        private void GetPoint2_2_Click(object sender, RoutedEventArgs e)
        {
            GetPoint2_2.Content = AnalogMeasures.Analogiche[AnalogsService.VAlim].MisuraRaw.ToString("#.00");
            Y_12V = Convert.ToDouble(Target2.Text);
            X_12V = AnalogMeasures.Analogiche[AnalogsService.VAlim].MisuraRaw;
        }

        private void Autoset_2_Click(object sender, RoutedEventArgs e)
        {
            Gain_Valim = (Y_12V - Y_5V) / (X_12V - X_5V);
            Offset_Valim = ((X_5V * Y_12V) - (X_12V * Y_5V)) / (X_12V - X_5V);

            RAM_VerifConfiguration.VAlim_Gain = (float)Gain_Valim;
            RAM_VerifConfiguration.VAlim_Offs = (float)Offset_Valim;

            AnalogMeasures.AggiornaGainOffset();
        }

        #endregion

        #region Factory Calibration

        private enum FtarIcoilStates
        {
            Start,
            Set52mA,
            Set103mA,
            End
        }
        private FtarIcoilStates FtarIcoilState;

        private void NewTarIcoil_Click(object sender, RoutedEventArgs e)
        {
            switch(FtarIcoilState)
            {
                case FtarIcoilStates.Start:
                    NewTarIcoil.Content = "Set 52mA";
                    FtarIcoilState = FtarIcoilStates.Set52mA;
                    break;
                case FtarIcoilStates.Set52mA:
                    NewTarIcoil.Content = "Set 103mA";
                    FtarIcoilState = FtarIcoilStates.Set103mA;
                    break;
                case FtarIcoilStates.Set103mA:
                    NewTarIcoil.Content = "Save";
                    FtarIcoilState = FtarIcoilStates.End;
                    break;
                case FtarIcoilStates.End:
                    NewTarIcoil.Content = "Start";
                    FtarIcoilState = FtarIcoilStates.Start;
                    break;
            }
        }

        private void NewTarValim_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResetTar420mA()
        {
            X_4mA = 0;
            Y_4mA = 0;
            Get4mA.IsEnabled = true;
            Get20mA.IsEnabled = false;
            SetGainOffs420mA.IsEnabled = false;
            FTarget0.Text = "4.0";
        }

        private void NewTar420mA_Click(object sender, RoutedEventArgs e)
        {
            ResetTar420mA();
        }

        private void Get4mA_Click(object sender, RoutedEventArgs e)
        {
            X_4mA = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].MisuraRaw;
            Y_4mA = Convert.ToDouble(FTarget0.Text);
            Get4mA.IsEnabled    = false;
            Get20mA.IsEnabled   = true;
            FTarget0.Text = "20.0";
        }

        private void Get20mA_Click(object sender, RoutedEventArgs e)
        {
            X_20mA = AnalogMeasures.Analogiche[AnalogsService.Out4_20mA].MisuraRaw;
            Y_20mA = Convert.ToDouble(FTarget0.Text);
            Get20mA.IsEnabled = false;
            SetGainOffs420mA.IsEnabled = true;
        }

        private void SetGainOffs420mA_Click(object sender, RoutedEventArgs e)
        {
            Gain_420mA = (Y_20mA - Y_4mA) / (X_20mA - X_4mA);
            Offset_420mA = Y_4mA - (Gain_420mA * X_4mA);
            RAM_VerifConfiguration.Out4_20mA_Gain = (float)Gain_420mA;
            RAM_VerifConfiguration.Out4_20mA_Offs = (float)Offset_420mA;
            ResetTar420mA();
        }

        private double X_52mA, X_103mA, Y_52mA, Y_103mA;
        private void Get5mA_Click(object sender, RoutedEventArgs e)
        {
            X_52mA = AnalogMeasures.Analogiche[AnalogsService.Icoil].MisuraRaw;
            Y_52mA = Convert.ToDouble(FTarget1.Text);
        }

        private void Get125mA_Click(object sender, RoutedEventArgs e)
        {
            X_103mA = AnalogMeasures.Analogiche[AnalogsService.Icoil].MisuraRaw;
            Y_103mA = Convert.ToDouble(FTarget1.Text);
        }

        private void SetGainOffsIcoil_Click(object sender, RoutedEventArgs e)
        {
            Gain_Coil = (Y_103mA - Y_52mA) / (X_103mA - X_52mA);
            Offset_Coil = ((X_52mA * Y_103mA) - (X_103mA * Y_52mA)) / (X_103mA - X_52mA);
            RAM_VerifConfiguration.Icoil_Gain = (float)Gain_Coil;
            RAM_VerifConfiguration.Icoil_Offs = (float)Offset_Coil;
            AnalogMeasures.AggiornaGainOffset();
        }

        private void Get5V_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Get12V_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetGainOffsValim_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        private async void SaveConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            List<VerificatorConfig> ConfigList = new List<VerificatorConfig>();

            RAM_VerifConfiguration.DataLastTaratura = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            RAM_VerifConfiguration.DataNextTaratura = DateTime.Now.AddYears(1).ToString("dd/MM/yyyy HH:mm");
            RAM_VerifConfiguration.TarMode          = false;
            RunningConfiguration.Instance.QuickTarEnabled = false;
            RAM_VerifConfiguration.SN_Verificator   = FSerialNumber.Text;

            RAM_VerifConfiguration.Vbattery100 = AnalogMeasures.Analogiche[AnalogsService.VAlim].Misura;
            RAM_VerifConfiguration.Vbattery0 = RAM_VerifConfiguration.Vbattery100 - 1.0f;

            ConfigList.Add(RAM_VerifConfiguration);

            SerializableStorage<VerificatorConfig>.Save(FileManager.VerificatorConfigFile, FileManager.MainFolder.Path, ConfigList);

            ContentDialog dialog = new ContentDialog()
            {
                Title = "Calibration",
                Content = "Calibration succesfully saved",
                CloseButtonText = "OK",
            };            
            await dialog.ShowAsync();
        }

        private async void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            RAM_VerifConfiguration.TarMode = false;
            RunningConfiguration.Instance.QuickTarEnabled = false;
            SerialNumber.Text = RAM_VerifConfiguration.SN_Verificator;
            RAM_VerifConfiguration.Out4_12mA_Gain = Gain_412mA_Bkup;
            RAM_VerifConfiguration.Out4_12mA_Offs = Offset_412mA_Bkup;
            RAM_VerifConfiguration.Out4_20mA_Gain = Gain_420mA_Bkup;
            RAM_VerifConfiguration.Out4_20mA_Offs = Offset_420mA_Bkup;
            RAM_VerifConfiguration.Icoil_Gain = Gain_Coil_Bkup;
            RAM_VerifConfiguration.Icoil_Offs = Offset_CoilBkup;
            RAM_VerifConfiguration.VAlim_Gain = Gain_Valim_Bkup;
            RAM_VerifConfiguration.VAlim_Offs = Offset_Valim_Bkup;
            RAM_VerifConfiguration.Vbattery100 = Vbattery100_Bkup;            

            ContentDialog dialog = new ContentDialog()
            {
                Title = "Calibration",
                Content = "Calibration not saved, parameters restored",
                CloseButtonText = "OK",
            };
            await dialog.ShowAsync();
        }

    }
}
