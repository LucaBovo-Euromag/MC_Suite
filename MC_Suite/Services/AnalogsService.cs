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
using System.Threading.Tasks;
using MC_Suite.Modbus;

namespace MC_Suite.Services
{
    public class AnalogsService
    {
        private static AnalogsService _instance;
        public static AnalogsService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AnalogsService();
                return _instance;
            }
        }

        public ADS1115 ADC_Module
        {
            get
            {
                return ADS1115.Instance;
            }
            set {; }
        }

        public VerificatorConfig RAM_VerifConfiguration
        {
            get
            {
                return VerificatorConfig.Instance;
            }
            set {; }
        }               

        private DispatcherTimer I2C_PollingTimer;
        public async Task<bool> ADC_Module_Open()
        {
            string i2cDeviceSelector = I2cDevice.GetDeviceSelector();
            IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);

            ADC_Module.Set_ADS_Power_On();

            if ((devices != null) && (devices.Count != 0))
            {
                if (ADC_Module.Device == null)
                {
                    ADC_Module.Settings = new I2cConnectionSettings(ADC_Module.Address);
                    ADC_Module.Device = await I2cDevice.FromIdAsync(devices[0].Id, ADC_Module.Settings);
                }                

                try
                {                    
                    ADC_Module.ResetADC();

                    // Start the polling timer.
                    I2C_PollingTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(200) };
                    I2C_PollingTimer.Tick += I2C_PollingTimer_Tick;
                    I2C_PollingTimer.Stop();

                    ReadingChannel = 0;

                    InitAnalogiche();

                    ADC_Module.Ready  = true;
                    ADC_Module.Status = "I2C Device Ready";
                }
                catch
                {
                    ADC_Module.Ready  = false;
                    ADC_Module.Status = "ADC Module Error";
                }              
            }
            else
            {
                ADC_Module.Ready  = false;
                ADC_Module.Status = "I2C Device Not Found";

                ADC_Module.Set_ADS_Power_Off();
            }

            return ADC_Module.Ready;
        }

        public bool ADC_ModuleIsReady
        {
            get { return ADC_Module.Ready; }
        }

        public void ADC_MeasuresStop()
        {
            I2C_PollingTimer.Stop();
        }

        public void ADC_MeasuresStart()
        {
            //ADC_Module.Set_ADS_Power_On();
            I2C_PollingTimer.Start();
        }

        public async void ADC_MeasuresReset()
        {
            //ADC_Module.Set_ADS_Power_Off();
            await Task.Delay(TimeSpan.FromSeconds(1));
            //ADC_Module.Set_ADS_Power_On();
            await Task.Delay(TimeSpan.FromSeconds(1));

            ADC_Module.ResetADC();

            StartNewMeasure(AN_Free);
            StartNewMeasure(Out4_20mA);
            StartNewMeasure(Icoil);
            StartNewMeasure(VAlim);
        }

        public void ADC_Restart()
        {
            ADC_MeasuresStop();
            ADC_MeasuresReset();
            ADC_MeasuresStart();
        }

        public void StartNewMeasure(byte channel)
        {            
            ADC_Module.RawMeasures[channel]  = 0;
            ADC_Module.CovertIst[channel]    = 0;
        }

        private bool _valimLow;
        public bool VAlimLow
        {
            get { return _valimLow; }
            set
            {
                if (value != _valimLow)
                {
                    _valimLow = value;
                    OnPropertyChanged("VAlimLow");
                }
            }
        }

        private bool _powerMonitorOn;
        public bool PowerMonitorOn
        {
            get { return _powerMonitorOn; }
            set
            {
                if (value != _powerMonitorOn)
                {
                    _powerMonitorOn = value;
                    OnPropertyChanged("PowerMonitorOn");
                }
            }
        }

        private byte ReadingChannel;

        private async void I2C_PollingTimer_Tick(object sender, object e)
        {
            if (ReadingChannel < 3)
                ReadingChannel++;
            else
                ReadingChannel = 0;

            ADC_Module.RawMeasures[ReadingChannel] = await ADC_Module.ReadSingleADC(ReadingChannel);

            if (ADC_Module.RawMeasures[ReadingChannel] > 5500.0f)
                ADC_Module.RawMeasures[ReadingChannel] = 0;

            ADC_Module.CovertIst[ReadingChannel] = GainOffset(ADC_Module.RawMeasures[ReadingChannel], Analogiche[ReadingChannel].Gain, Analogiche[ReadingChannel].Offset);
            ADC_Module.CovertIst2[ReadingChannel] = GainOffset(ADC_Module.RawMeasures[ReadingChannel], Analogiche[ReadingChannel].Gain2, Analogiche[ReadingChannel].Offset2);
        }

        private float GainOffset(float Measure, float Gain, float Offset)
        {
            float MeasureOut;

            if (Measure == 0)
                return 0;

            MeasureOut = (Measure * Gain) - Offset;

            return MeasureOut;
        }

        public float VAlimInVolt()
        {
            return ADC_Module.CovertIst[VAlim];
        }


        public Analogica[] Analogiche = new Analogica[4];
        public class Analogica
        {
            public float Misura { get; set; }
            public float Misura2 { get; set; }
            public float Gain { get; set; }
            public float Gain2 { get; set; }
            public float Offset { get; set; }
            public float Offset2 { get; set; }
            public float MisuraRaw;
            public float TestOffset;
        }

        private void InitAnalogiche()
        {
            Analogiche[Out4_20mA] = new Analogica();
            Analogiche[Out4_20mA].Misura = 0;
            Analogiche[Out4_20mA].Misura2 = 0;
            Analogiche[Out4_20mA].Gain = RAM_VerifConfiguration.Out4_20mA_Gain;
            Analogiche[Out4_20mA].Offset = RAM_VerifConfiguration.Out4_20mA_Offs;
            Analogiche[Out4_20mA].Gain2 = RAM_VerifConfiguration.Out4_12mA_Gain;
            Analogiche[Out4_20mA].Offset2 = RAM_VerifConfiguration.Out4_12mA_Offs;
            Analogiche[Out4_20mA].MisuraRaw = 0;            
            Analogiche[Out4_20mA].TestOffset = 0;

            Analogiche[Icoil] = new Analogica();
            Analogiche[Icoil].Misura = 0;
            Analogiche[Icoil].Misura2 = 0;
            Analogiche[Icoil].Gain = RAM_VerifConfiguration.Icoil_Gain;
            Analogiche[Icoil].Offset = RAM_VerifConfiguration.Icoil_Offs;
            Analogiche[Icoil].Gain2 = RAM_VerifConfiguration.Icoil_Gain;
            Analogiche[Icoil].Offset2 = RAM_VerifConfiguration.Icoil_Offs;
            Analogiche[Icoil].MisuraRaw = 0;
            Analogiche[Icoil].TestOffset = 0;

            Analogiche[VAlim] = new Analogica();
            Analogiche[VAlim].Misura = 0;
            Analogiche[VAlim].Misura2 = 0;
            Analogiche[VAlim].Gain = RAM_VerifConfiguration.VAlim_Gain;
            Analogiche[VAlim].Offset = RAM_VerifConfiguration.VAlim_Offs;
            Analogiche[VAlim].Gain2 = RAM_VerifConfiguration.VAlim_Gain;
            Analogiche[VAlim].Offset2 = RAM_VerifConfiguration.VAlim_Offs;
            Analogiche[VAlim].MisuraRaw = 0;
            Analogiche[VAlim].TestOffset = 0;

            Analogiche[AN_Free] = new Analogica();
            Analogiche[AN_Free].Misura = 0;
            Analogiche[AN_Free].Misura2 = 0;
            Analogiche[AN_Free].Gain = 1.0f;
            Analogiche[AN_Free].Offset = 0.0f;
            Analogiche[AN_Free].Gain2 = 1.0f;
            Analogiche[AN_Free].Offset2 = 0.0f;
            Analogiche[AN_Free].MisuraRaw = 0;
            Analogiche[AN_Free].TestOffset = 0;
        }

        public void AggiornaMisure()
        {
            Analogiche[Out4_20mA].Misura = ADC_Module.CovertIst[Out4_20mA] + Analogiche[Out4_20mA].TestOffset;
            Analogiche[Out4_20mA].Misura2 = ADC_Module.CovertIst2[Out4_20mA] + Analogiche[Out4_20mA].TestOffset;
            Analogiche[Out4_20mA].MisuraRaw = ADC_Module.RawMeasures[Out4_20mA];

            Analogiche[Icoil].Misura = ADC_Module.CovertIst[Icoil] + Analogiche[Icoil].TestOffset;
            Analogiche[Icoil].MisuraRaw = ADC_Module.RawMeasures[Icoil];

            Analogiche[VAlim].Misura = ADC_Module.CovertIst[VAlim] + Analogiche[VAlim].TestOffset;
            Analogiche[VAlim].MisuraRaw  = ADC_Module.RawMeasures[VAlim];

            Analogiche[AN_Free].Misura = ADC_Module.CovertIst[AN_Free] + Analogiche[AN_Free].TestOffset;
            Analogiche[AN_Free].MisuraRaw = ADC_Module.RawMeasures[AN_Free];
        }

        public void AggiornaGainOffset()
        {
            Analogiche[Out4_20mA].Gain = RAM_VerifConfiguration.Out4_20mA_Gain;
            Analogiche[Out4_20mA].Offset = RAM_VerifConfiguration.Out4_20mA_Offs;

            Analogiche[Out4_20mA].Gain2 = RAM_VerifConfiguration.Out4_12mA_Gain;
            Analogiche[Out4_20mA].Offset2 = RAM_VerifConfiguration.Out4_12mA_Offs;

            Analogiche[Icoil].Gain = RAM_VerifConfiguration.Icoil_Gain;
            Analogiche[Icoil].Offset = RAM_VerifConfiguration.Icoil_Offs;

            Analogiche[VAlim].Gain = RAM_VerifConfiguration.VAlim_Gain;
            Analogiche[VAlim].Offset = RAM_VerifConfiguration.VAlim_Offs;

            Analogiche[AN_Free].Gain = 1.0f;
            Analogiche[AN_Free].Offset = 0.0f;
        }

        //Canali ADC**********************************************************************
        //***********************  Canali ADS115      USB-4702 Interface       DSUB Pin  *
        public const int Out4_20mA = 3; // ---------> AN0  ------------------> 23        * 
        public const int Icoil     = 2; // ---------> AN2  ------------------> 22        *  
        public const int VAlim     = 1; // ---------> AN4  ------------------> 21        * 
        public const int AN_Free   = 0; // ---------> AN6  ------------------> 20        *   //Nelle versioni < 1.0.17 Legge la Freq_Out

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
