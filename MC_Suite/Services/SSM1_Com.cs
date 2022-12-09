using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace MC_Suite.Services
{
    public class SSM1_Com
    {
        private static SSM1_Com _instance;
        public static SSM1_Com Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SSM1_Com();
                return _instance;
            }
        }

        public void InitPort( GpioController GPIO )
        {
            DAC_CS = GPIO.OpenPin(DAC_CS_Pin);
            DAC_CS.SetDriveMode(GpioPinDriveMode.Output);
            DAC_CS.Write(GpioPinValue.High);

            DAC_CK = GPIO.OpenPin(DAC_CK_Pin);
            DAC_CK.SetDriveMode(GpioPinDriveMode.Output);
            DAC_CK.Write(GpioPinValue.Low);

            DAC_DAT = GPIO.OpenPin(DAC_DAT_Pin);
            DAC_DAT.SetDriveMode(GpioPinDriveMode.Output);
            DAC_DAT.Write(GpioPinValue.Low);
        }


        int BitDelay = 1;
        public void Write( ushort valore )
        {
            DAC_CS.Write(GpioPinValue.Low);
            Thread.Sleep(BitDelay);
            DAC_CK.Write(GpioPinValue.Low);
            Thread.Sleep(BitDelay);

            for (int i = 15; i >= 0; i--)
            {
                DAC_CK.Write(GpioPinValue.Low);

                if ((valore & (1 << i)) > 0)
                    DAC_DAT.Write(GpioPinValue.High);
                else
                    DAC_DAT.Write(GpioPinValue.Low);

                Thread.Sleep(BitDelay);
                DAC_CK.Write(GpioPinValue.High);
                Thread.Sleep(BitDelay);
            }
            DAC_DAT.Write(GpioPinValue.Low);
            Thread.Sleep(BitDelay);
            DAC_CK.Write(GpioPinValue.Low);
            Thread.Sleep(BitDelay);
            DAC_CS.Write(GpioPinValue.High);
        }

        public void Dispose()
        {
            DAC_CS.Dispose();
            DAC_CK.Dispose();
            DAC_DAT.Dispose();
        }

        //******************************    Raspberry IO        USB-4702_interface        DSUB PIN
        private const int DAC_CS_Pin        = 17; // ---------> OUT4 -------------------> 34
        private const int DAC_CK_Pin        = 22; // ---------> DAC0 -------------------> 26
        private const int DAC_DAT_Pin       = 27; // ---------> DAC1 -------------------> 8

        public static GpioPin DAC_CS;
        public static GpioPin DAC_CK;
        public static GpioPin DAC_DAT;
    }
}
