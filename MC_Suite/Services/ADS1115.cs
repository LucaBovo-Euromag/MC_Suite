using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.I2c;
using Windows.Devices.Gpio;
using Windows.Devices.Enumeration;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;


//See: https://cdn-shop.adafruit.com/datasheets/ads1015.pdf

namespace MC_Suite.Services
{
    public class ADS1115 : INotifyPropertyChanged
    {
        private static ADS1115 _instance;
        public static ADS1115 Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ADS1115();
                return _instance;
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                if (value != _status)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        private bool _ready;
        public bool Ready
        {
            get { return _ready; }
            set
            {
                if (value != _ready)
                {
                    _ready = value;
                    OnPropertyChanged("Ready");
                }
            }
        }

        //Legge in modalità singola un canale dell'ADC
        public async Task<float> ReadSingleADC(byte Channel)
        {
            MuxConfig MuxChannel;
            switch(Channel)
            {
                case 0:
                    MuxChannel = MuxConfig.Single_0;
                    break;
                case 1:
                    MuxChannel = MuxConfig.Single_1;
                    break;
                case 2:
                    MuxChannel = MuxConfig.Single_2;
                    break;
                case 3:
                    MuxChannel = MuxConfig.Single_3;
                    break;
                default:
                    MuxChannel = MuxConfig.Single_0;
                    break;
            }

            return await ReadADC(OperatingMode.Single,
                                       MuxChannel,
                                       GainSetting.TwoThirds,
                                       ConversionMode.Single,
                                       SamplesRate.Sps_1600,
                                       ComparatorMode.Traditional,
                                       ComparatorPolarity.ActiveLow,
                                       LatchingComparator.NonLatching,
                                       ComparatorQueue.QueueNone);

        }


        private async Task<float> ReadADC(OperatingMode operatingMode, MuxConfig muxConfig, GainSetting gainSetting,
                             ConversionMode conversionMode, SamplesRate samplesRate, ComparatorMode comparatorMode, ComparatorPolarity comparatorPolarity,
                             LatchingComparator latchingComparator, ComparatorQueue comparatorQueue)
        {
            if (this.Device == null)
                return 0;
            else
            {
                uint OpMode     = Convert.ToUInt32(operatingMode);
                uint MuxConf    = Convert.ToUInt32(muxConfig);
                uint Gain       = Convert.ToUInt32(gainSetting);
                uint ConvMode   = Convert.ToUInt32(conversionMode);
                uint SampRate   = Convert.ToUInt32(samplesRate);
                uint CompMode   = Convert.ToUInt32(comparatorMode);
                uint CompPol    = Convert.ToUInt32(comparatorPolarity);
                uint LatchComp  = Convert.ToUInt32(latchingComparator);
                uint CompQueue  = Convert.ToUInt32(comparatorQueue);
                Config = OpMode + MuxConf + Gain + ConvMode + SampRate + CompMode + CompPol + LatchComp + CompQueue;

                byte[] CfgBuf = BitConverter.GetBytes(Config);
                
                ConfBuf[0] = Convert.ToByte(Registers.Config); 
                ConfBuf[1] = CfgBuf[1];
                ConfBuf[2] = CfgBuf[0];

                Exception = null;
                ExcepionStr = string.Empty;

                try
                { 
                    this.Device.Write(ConfBuf);

                    //Wait for conversion complete
                    await Task.Delay(TimeSpan.FromMilliseconds(ConversionDelay));

                    //Read Value
                    RegBuf[0] = Convert.ToByte(Registers.Conversion);
                    this.Device.WriteRead(RegBuf, ReadBuf);
                    this.Ready = true;
                }
                catch(Exception e)
                {
                    Exception = e;
                    ExcepionStr = "ADS1115 Error";
                    this.Ready = false;
                    return 0;
                }
                //Return value converted in mV
                return ConvertToVoltage(ReadBuf, VPS(gainSetting));
            }
        }


        public async void ResetADC()
        {
            byte[] ResetBuf = new byte[2];

            ResetBuf[0] = 0x00; //General Call Address
            ResetBuf[1] = 0x06; //Reset command

            try { 

                this.Device.Write(ResetBuf);

                //Wait for conversion complete
                await Task.Delay(TimeSpan.FromMilliseconds(ConversionDelay));
            }
            catch(Exception ex)
            {
                bool ok = true;
            }
        }

        public Exception Exception;

        private string _excepionStr;
        public string ExcepionStr
        {
            get { return _excepionStr; }
            set
            {
                if (value != _excepionStr)
                {
                    _excepionStr = value;
                    OnPropertyChanged("ExcepionStr");
                }
            }
        }


        //I2C Address
        public byte Address
        {
            get { return 0x48; }
        }

        //Conversion Delay [ms]
        private double ConversionDelay
        {
            get { return 20; }
        }

        //Bit Shift
        private byte BitShift
        {
            get { return 0; }
        }


        //AIN0
        private short _ain0;
        public short AIN0
        {
            get { return _ain0; }
            set
            {
                if (value != _ain0)
                {
                    _ain0 = value;
                    OnPropertyChanged("AIN0");
                }
            }
        }

        //AIN1
        private float _ain1;
        public float AIN1
        {
            get { return _ain1; }
            set
            {
                if (value != _ain1)
                {
                    _ain1 = value;
                    OnPropertyChanged("AIN1");
                }
            }
        }

        //AIN2
        private float _ain2;
        public float AIN2
        {
            get { return _ain2; }
            set
            {
                if (value != _ain2)
                {
                    _ain2 = value;
                    OnPropertyChanged("AIN2");
                }
            }
        }

        //AIN3
        private float _ain3;
        public float AIN3
        {
            get { return _ain3; }
            set
            {
                if (value != _ain3)
                {
                    _ain3 = value;
                    OnPropertyChanged("AIN3");
                }
            }
        }


        private float[] _rawMeasures;
        public float[] RawMeasures
        {
            get {
                if (_rawMeasures == null)
                    _rawMeasures = new float[4];
                return _rawMeasures;
            }
            set
            {
                if (value != _rawMeasures)
                {
                    _rawMeasures = value;
                    OnPropertyChanged("RawMeasures");
                }
            }
        }

        private float[] _filtMeasures;
        public float[] FiltMeasures
        {
            get
            {
                if (_filtMeasures == null)
                    _filtMeasures = new float[4];
                return _filtMeasures;
            }
            set
            {
                if (value != _filtMeasures)
                {
                    _filtMeasures = value;
                    OnPropertyChanged("FiltMeasures");
                }
            }
        }

        private float[] _covertIst;
        public float[] CovertIst
        {
            get
            {
                if (_covertIst == null)
                    _covertIst = new float[4];
                return _covertIst;
            }
            set
            {
                if (value != _covertIst)
                {
                    _covertIst = value;
                    OnPropertyChanged("CovertIst");
                }
            }
        }

        private float[] _covertIst2;
        public float[] CovertIst2
        {
            get
            {
                if (_covertIst2 == null)
                    _covertIst2 = new float[4];
                return _covertIst2;
            }
            set
            {
                if (value != _covertIst2)
                {
                    _covertIst2 = value;
                    OnPropertyChanged("CovertIst2");
                }
            }
        }

        //Calcola il fattore di conversione per le letture
        private float VPS(GainSetting Gain)
        {
            float Dividendo;
            float result;
            switch(Gain)
            {
                case GainSetting.TwoThirds:
                    Dividendo = 6144;
                    break;
                case GainSetting.One:
                    Dividendo = 4096;
                    break;                
                case GainSetting.Two:
                    Dividendo = 2048;
                    break;
                case GainSetting.Four:
                    Dividendo = 1028;
                    break;
                case GainSetting.Eight:
                    Dividendo = 512;
                    break;
                case GainSetting.Sixteen:
                    Dividendo = 128;
                    break;
                default:
                    Dividendo = 6144;
                    break;
            }

            result = Dividendo / 32768;
            return result;
        }

        private byte[] ReadBuf  = new byte[2];
        private byte[] ConfBuf  = new byte[3];
        private byte[] RegBuf   = new byte[1];

        private float ConvertToVoltage( byte[] RawMeas, float vps )
        {
            Int32 value = (RawMeas[0] * 256) + RawMeas[1];
            return value * vps;
        }

        public enum Registers : byte
        {
            Conversion = 0x00,  //Conversion register
            Config     = 0x01,  //Config Register
            Lo_thresh  = 0x02,  //Lo_thresh register
            Hi_thresh  = 0x03   //Hi_thresh register
        }

        public enum OperatingMode : uint
        {
            Single  = 0x8000,  // Write: Set to start a single-conversion
            Busy    = 0x0000,  // Read: Bit = 0 when conversion is in progress
            NotBusy = 0x8000   // Read: Bit = 1 when device is not performing a conversion
        }

        public enum MuxConfig: uint
        {
            Diff_0_1 = 0x0000,  // Differential P = AIN0, N = AIN1 (default)
            Diff_0_3 = 0x1000,  // Differential P = AIN0, N = AIN3
            Diff_1_3 = 0x2000,  // Differential P = AIN1, N = AIN3
            Diff_2_3 = 0x3000,  // Differential P = AIN2, N = AIN3
            Single_0 = 0x4000,  // Single-ended AIN0
            Single_1 = 0x5000,  // Single-ended AIN1
            Single_2 = 0x6000,  // Single-ended AIN2
            Single_3 = 0x7000   // Single-ended AIN3
        }

        public enum GainSetting : uint
        {
            TwoThirds  = 0x0000,   // +/-6.144V range = Gain 2/3
            One        = 0x0200,   // +/-4.096V range = Gain 1
            Two        = 0x0400,   // +/-2.048V range = Gain 2 (default)
            Four       = 0x0600,   // +/-1.024V range = Gain 4
            Eight      = 0x0800,   // +/-0.512V range = Gain 8
            Sixteen    = 0x0A00    // +/-0.256V range = Gain 16
        }

        public enum ConversionMode : uint
        {
            Continuous = 0x0000,  // Continuous conversion mode
            Single     = 0x0100  // Power-down single-shot mode (default)
        }

        public enum SamplesRate : uint
        {
            Sps_128  = 0x0000,  // 128 samples per second
            Sps_250  = 0x0020,  // 250 samples per second
            Sps_490  = 0x0040,  // 490 samples per second
            Sps_920  = 0x0060,  // 920 samples per second
            Sps_1600 = 0x0080,  // 1600 samples per second (default)
            Sps_2400 = 0x00A0,  // 2400 samples per second
            Ssp_3300 = 0x00C0  // 3300 samples per second
        }

        public enum ComparatorMode : uint
        {
            Traditional = 0x0000,  // Traditional comparator with hysteresis (default)
            Window      = 0x0010  // Window comparator
        }

        public enum ComparatorPolarity : uint
        {
            ActiveLow   = 0x0000,  // ALERT/RDY pin is low when active (default)
            ActiveHigh  = 0x0008   // ALERT/RDY pin is high when active
        }

        public enum LatchingComparator : uint
        {
            NonLatching = 0x0000,  // Non-latching comparator (default)
            Latching    = 0x0004  // Latching comparator
        }

        public enum ComparatorQueue : uint
        {
            QueueAfter1Convertion   = 0x0000,   // Assert ALERT/RDY after one conversions
            QueueAfter2Convertion   = 0x0001,   // Assert ALERT/RDY after two conversions
            QueueAfter4Convertion   = 0x0002,   // Assert ALERT/RDY after four conversions
            QueueNone               = 0x0003  // Disable the comparator and put ALERT/RDY in high state (default)
        }

        public void InitADS1115(GpioController GPIO)
        {
            ADS_PowerOn = GPIO.OpenPin(ADS_PowerOn_Pin);
            ADS_PowerOn.SetDriveMode(GpioPinDriveMode.Output);
            ADS_PowerOn.Write(GpioPinValue.High);

            ADS_DataReady = GPIO.OpenPin(ADS_DataReady_Pin);
            ADS_DataReady.SetDriveMode(GpioPinDriveMode.Input);
        }

        public Brush Set_ADS_Power_On()
        {
            Brush Color = new SolidColorBrush(Colors.LimeGreen);
            ADS_PowerOn.Write(GpioPinValue.High);
            return Color;
        }
        public Brush Set_ADS_Power_Off()
        {
            Brush Color = new SolidColorBrush(Colors.Gray);
            ADS_PowerOn.Write(GpioPinValue.Low);
            return Color;
        }

        public I2cDevice Device;
        public I2cConnectionSettings Settings;
        public uint Config;

        public const int ADS_PowerOn_Pin   = 23;   
        public const int ADS_DataReady_Pin = 4;     

        public GpioPin ADS_PowerOn;
        public GpioPin ADS_DataReady;


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
