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

namespace MC_Suite.Services
{
    public class GPIO_Device
    {
        private static GPIO_Device _instance;
        public static GPIO_Device Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GPIO_Device();
                return _instance;
            }
        }

        private static GpioController GPIO_Controller;

        public struct GPinState
        {
            public bool value;
            public Brush color;
        }

        public SSM1_Com InterfacciaConv
        {
            get
            {
                return SSM1_Com.Instance;
            }
        }



        public SolidColorBrush InitGPIO()
        {
            if (this.Inizialized)
            {
                return new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                GPIO_Controller = GpioController.GetDefault();
                if (GPIO_Controller == null)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                else
                {
                    EmptyPype = GPIO_Controller.OpenPin(EmptyPype_Pin);
                    EmptyPype.SetDriveMode(GpioPinDriveMode.Output);
                    EmptyPype.Write(GpioPinValue.High);

                    VAux = GPIO_Controller.OpenPin(VAux_Pin);
                    VAux.SetDriveMode(GpioPinDriveMode.Output);
                    VAux.Write(GpioPinValue.Low);

                    GP_IN = GPIO_Controller.OpenPin(GP_IN_Pin);
                    GP_IN.SetDriveMode(GpioPinDriveMode.Output);
                    GP_IN.Write(GpioPinValue.Low);

                    RL_4_20mA = GPIO_Controller.OpenPin(RL_4_20mA_Pin);
                    RL_4_20mA.SetDriveMode(GpioPinDriveMode.Output);
                    RL_4_20mA.Write(GpioPinValue.Low);

                    LowBatteryOut = GPIO_Controller.OpenPin(LowBatteryOut_Pin);
                    LowBatteryOut.SetDriveMode(GpioPinDriveMode.Output);
                    LowBatteryOut.Write(GpioPinValue.Low);

                    ProgOut = GPIO_Controller.OpenPin(ProgOut_Pin);
                    ProgOut.SetDriveMode(GpioPinDriveMode.Input);

                    PulseOut = GPIO_Controller.OpenPin(PulseOut_Pin);
                    PulseOut.SetDriveMode(GpioPinDriveMode.Input);

                    PulseNegOut = GPIO_Controller.OpenPin(PulseNegOut_Pin);
                    PulseNegOut.SetDriveMode(GpioPinDriveMode.Input);

                    PulsePosOut = GPIO_Controller.OpenPin(PulsePosOut_Pin);
                    PulsePosOut.SetDriveMode(GpioPinDriveMode.Input);

                    FreqOut = GPIO_Controller.OpenPin(FreqOut_Pin);
                    FreqOut.SetDriveMode(GpioPinDriveMode.Input);

                    ChargingIn = GPIO_Controller.OpenPin(ChargingIn_Pin);
                    ChargingIn.SetDriveMode(GpioPinDriveMode.Input);

                    InterfacciaConv.InitPort(GPIO_Controller);

                    ADS1115.Instance.InitADS1115(GPIO_Controller);
                    this.Inizialized = true;

                    return new SolidColorBrush(Colors.LimeGreen);
                }
            }
        }

        private Boolean Inizialized = false;

        public bool Epipe_Status;
        public Brush SetEPipe()
        {
            Brush Color = new SolidColorBrush(Colors.LimeGreen);
            EmptyPype.Write(GpioPinValue.High);
            Epipe_Status = true;
            return Color;
        }
        public Brush ResEPipe()
        {
            Brush Color = new SolidColorBrush(Colors.Gray);
            EmptyPype.Write(GpioPinValue.Low);
            Epipe_Status = false;
            return Color;
        }
        public Brush SetVAux()
        {
            Brush Color = new SolidColorBrush(Colors.LimeGreen);
            VAux.Write(GpioPinValue.High);
            return Color;
        }
        public Brush ResVAux()
        {
            Brush Color = new SolidColorBrush(Colors.Gray);
            VAux.Write(GpioPinValue.Low);
            return Color;
        }
        public Brush SetGPIn()
        {
            Brush Color = new SolidColorBrush(Colors.LimeGreen);
            GP_IN.Write(GpioPinValue.High);
            return Color;
        }
        public Brush ResGPIn()
        {
            Brush Color = new SolidColorBrush(Colors.Gray);
            GP_IN.Write(GpioPinValue.Low);
            return Color;
        }
        public Brush SetRL_4_20mA()
        {
            Brush Color = new SolidColorBrush(Colors.LimeGreen);
            RL_4_20mA.Write(GpioPinValue.High);
            return Color;
        }
        public Brush ResRL_4_20mA()
        {
            Brush Color = new SolidColorBrush(Colors.Gray);
            RL_4_20mA.Write(GpioPinValue.Low);
            return Color;
        }

        public Brush SetLowBatteryOut()
        {
            Brush Color = new SolidColorBrush(Colors.LimeGreen);
            LowBatteryOut.Write(GpioPinValue.High);
            return Color;
        }
        public Brush ResLowBatteryOut()
        {
            Brush Color = new SolidColorBrush(Colors.Gray);
            LowBatteryOut.Write(GpioPinValue.Low);
            return Color;
        }

        public GPinState GetLowBatteryOutState()
        {
            GPinState State;
            if (LowBatteryOut.Read() == GpioPinValue.High)
            {
                State.value = true;
                State.color = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                State.value = false;
                State.color = new SolidColorBrush(Colors.Yellow);
            }
            return State;
        }

        public GPinState GetPulseOut()
        {
            GPinState State;
            if (PulseOut.Read() == GpioPinValue.High)
            {
                State.value = true;
                State.color = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                State.value = false;
                State.color = new SolidColorBrush(Colors.Yellow);
            }
            return State;
        }

        public GPinState GetPulseNegOut()
        {
            GPinState State;
            if (PulseNegOut.Read() == GpioPinValue.High)
            {
                State.value = true;
                State.color = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                State.value = false;
                State.color = new SolidColorBrush(Colors.Yellow);
            }
            return State;
        }

        public GPinState GetPulsePosOut()
        {
            GPinState State;
            if (PulsePosOut.Read() == GpioPinValue.High)
            {
                State.value = true;
                State.color = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                State.value = false;
                State.color = new SolidColorBrush(Colors.Yellow);
            }
            return State;
        }

        public GPinState GetProgOut()
        {
            GPinState State;
            if (ProgOut.Read() == GpioPinValue.High)
            {
                State.value = true;
                State.color = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                State.value = false;
                State.color = new SolidColorBrush(Colors.Yellow);
            }
            return State;
        }

        public GPinState GetFreqOut()
        {
            GPinState State;
            if (FreqOut.Read() == GpioPinValue.High)
            {
                State.value = true;
                State.color = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                State.value = false;
                State.color = new SolidColorBrush(Colors.Yellow);
            }
            return State;
        }

        public GPinState GetChargingIn()
        {
            GPinState State;
            if (ChargingIn.Read() == GpioPinValue.High)
            {
                State.value = true;
                State.color = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                State.value = false;
                State.color = new SolidColorBrush(Colors.Gray);
            }
            return State;
        }

        //******************************    Raspberry IO        USB-4702_interface        DSUB PIN

        public const int EmptyPype_Pin      = 10; // ------->   OUT3  ----------------->  15
        public const int VAux_Pin           = 9;  // ------->   OUT5  ----------------->  16 
        public const int GP_IN_Pin          = 11; // ------->   OUT6  ----------------->  35
        public const int RL_4_20mA_Pin      = 5;  // ------->   OUT2  ----------------->  33
        public const int LowBatteryOut_Pin  = 24; // ------->   OUT1  ----------------->  14

        public const int ProgOut_Pin        = 13; // ------->   IN4   ----------------->  30
        public const int PulseOut_Pin       = 19; // ------->   IN5   ----------------->  12
        public const int FreqOut_Pin        = 26; // ------->   IN6   ----------------->  31
        public const int ChargingIn_Pin     = 21; // ------->   IN3   ----------------->  11

        public const int PulseNegOut_Pin    = 20; // ------->   IN1   ----------------->  10
        public const int PulsePosOut_Pin    = 16; // ------->   IN0   ----------------->  28

        public GpioPin EmptyPype;
        public GpioPin VAux;
        public GpioPin GP_IN;
        public GpioPin RL_4_20mA;
        public GpioPin LowBatteryOut;

        public GpioPin ProgOut;
        public GpioPin PulseOut;
        public GpioPin FreqOut;
        public GpioPin ChargingIn;

        public GpioPin PulseNegOut;
        public GpioPin PulsePosOut;
    }
}
