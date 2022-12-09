using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using MC_Suite.Services;
using MC_Suite.Properties;
using MC_Suite.Modbus;


// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class IOpage : Page, INotifyPropertyChanged
    {
        public IOpage()
        {
            this.InitializeComponent();

            GPIOstatus.Fill = GPIO_Control.InitGPIO();

            ValueToSend.Text = "43690";

            Unloaded += IOpage_Unloaded;
        }

        #region Istanze

        public SSM1_Com InterfacciaConv
        {
            get
            {
                return SSM1_Com.Instance;
            }
        }

        public GPIO_Device GPIO_Control
        {
            get
            {
                return GPIO_Device.Instance;
            }
        }

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

        #endregion

        private string _gpIOStatus;
        public string GpIOStatus
        {
            get { return _gpIOStatus; }
            set
            {
                if (value != _gpIOStatus)
                {
                    _gpIOStatus = value;
                    OnPropertyChanged("GpIOStatus");
                }
            }
        }

        private void IOpage_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        #region SSM1

        
        private ICommand _setVerificationCmd;
        public ICommand SetVerificationCmd
        {
            get
            {
                if (_setVerificationCmd == null)
                {
                    _setVerificationCmd = new RelayCommand(
                        param => SetVerification()
                            );
                }
                return _setVerificationCmd;
            }
        }

        private void SetVerification()
        {
            MbConnection.SendCommand(ComSetup.Address, Map.Comandi.CMD_ENTER_VERIFICATION, 0, 3);
            MbConnection.SendCommandCompleted += MbConnection_SendCommandCompleted;
        }

        private void MbConnection_SendCommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;
            if (cmd.ReadRegisters_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                //SendValueToSSM1.Visibility = Visibility.Visible;
                //SendManualToSSM1.Visibility = Visibility.Visible;
            }
        }

        
        private ICommand _sendManualCmd;
        public ICommand SendManualCmd
        {
            get
            {
                if (_sendManualCmd == null)
                {
                    _sendManualCmd = new RelayCommand(
                        param => SendManual()
                            );
                }
                return _sendManualCmd;
            }
        }

        private void SendManual()
        {
            ushort Value = Convert.ToUInt16(ValueToSend.Text);
            InterfacciaConv.Write(Value);
        }

        private ICommand _sendCmd;
        public ICommand SendCmd
        {
            get
            {
                if (_sendCmd == null)
                {
                    _sendCmd = new RelayCommand(
                        param => SendValue()
                            );
                }
                return _sendCmd;
            }
        }

        private void SendValue()
        {
            ushort Value = (ushort)DAC_Value.SelectedValue;
            InterfacciaConv.Write(Value);   
        }

        ParametriVerificator Parametri = new ParametriVerificator(); // Soglie e tolleranze test

        private IDictionary<ushort, String> _dacValues;
        public IDictionary<ushort, String> DacValues
        {
            get
            {
                if (_dacValues == null)
                {
                    _dacValues = new Dictionary<ushort, String>();
                    _dacValues.Add(Parametri.Simul.DAC_Zero,    "Zero");
                    _dacValues.Add(Parametri.Simul.DAC_Low_406, "Low 406");
                    _dacValues.Add(Parametri.Simul.DAC_Hi_406,  "Hi 406");
                    _dacValues.Add(Parametri.Simul.DAC_Low_608, "Low 608");
                    _dacValues.Add(Parametri.Simul.DAC_Hi_608,  "Hi 608");
                }
                return _dacValues;
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

        private void ToggleEmptyPype_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    GPIO_Control.SetEPipe();
                }
                else
                {
                    GPIO_Control.ResEPipe();
                }
            }
        }

        private void ToggleVAux_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    GPIO_Control.SetVAux();
                }
                else
                {
                    GPIO_Control.ResVAux();
                }
            }
        }

        private void ToggleGP_IN_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    GPIO_Control.SetGPIn();
                }
                else
                {
                    GPIO_Control.ResGPIn();
                }
            }
        }

        private void ToggleRL_4_20mA_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    GPIO_Control.SetRL_4_20mA();
                }
                else
                {
                    GPIO_Control.ResRL_4_20mA();
                }
            }
        }

        private void DAC_Value_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ToggleCS_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    SSM1_Com.DAC_CS.Write(GpioPinValue.High);
                }
                else
                {
                    SSM1_Com.DAC_CS.Write(GpioPinValue.Low);
                }
            }
        }

        private void ToggleCK_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    SSM1_Com.DAC_CK.Write(GpioPinValue.High);
                }
                else
                {
                    SSM1_Com.DAC_CK.Write(GpioPinValue.Low);
                }
            }
        }

        private void ToggleDAT_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    SSM1_Com.DAC_DAT.Write(GpioPinValue.High);
                }
                else
                {
                    SSM1_Com.DAC_DAT.Write(GpioPinValue.Low);
                }
            }
        }

        private void ToggleADS_Power_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    ADS1115.Instance.Set_ADS_Power_On();
                }
                else
                {
                    ADS1115.Instance.Set_ADS_Power_Off();
                }
            }
        }

        private void ToggleBatteryLow_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    GPIO_Control.SetLowBatteryOut();
                }
                else
                {
                    GPIO_Control.ResLowBatteryOut();
                }
            }
        }
    }
}
