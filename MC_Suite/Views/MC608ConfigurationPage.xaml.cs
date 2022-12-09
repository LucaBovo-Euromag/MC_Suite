using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using Windows.Devices.SerialCommunication;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MC_Suite.Properties;
using MC_Suite.Services;
using MC_Suite.Modbus;




// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MC608ConfigurationPage : Page, INotifyPropertyChanged
    {
        public MC608ConfigurationPage()
        {
            this.InitializeComponent();
            AggiornaInfo();
            ConnectionRing.Visibility = Visibility.Collapsed;
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

        private string _modbusStatus;
        public string ModbusStatus
        {
            get { return _modbusStatus; }
            set
            {
                if (value != _modbusStatus)
                {
                    _modbusStatus = value;
                    OnPropertyChanged("ModbusStatus");
                }
            }
        }

        private string _registerValueStr;
        public string RegisterValueStr
        {
            get { return _registerValueStr; }
            set
            {
                if (value != _registerValueStr)
                {
                    _registerValueStr = value;
                    OnPropertyChanged("RegisterValueStr");
                }
            }
        }
        

        private ICommand _readRegistercmd;
        public ICommand ReadRegistercmd
        {
            get
            {
                if (_readRegistercmd == null)
                {
                    _readRegistercmd = new RelayCommand(
                        param => ReadRegister()
                            );
                }
                return _readRegistercmd;
            }
        }

        private void ReadRegister()
        {
            if(MbConnection.IsOpen)
            { 
                ModbusStatus = "Reading...";
                ConnectionRing.Visibility = Visibility.Visible;
                MbConnection.ReadRegisters(ComSetup.Address, Map.Registri_CMD3_16.Release_ModBus, Map.Registri_CMD3_16.ResetTotN, 3);
                MbConnection.ReadRegistersCompleted += MbConnection_ReadRegistersCompleted;
            }
            else
                ModbusStatus = "No Connection";
        }

        private void MbConnection_ReadRegistersCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;
            if (cmd.ReadRegisters_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                ModbusStatus = cmd.ReadRegisters_CommandResult.Message;
                MbConnection.MbConnectionStatus = MbCOMPortManager.MbConnectionStates.Working;
                AggiornaInfo();
            }
            else
            {
                ModbusStatus = cmd.ReadRegisters_CommandResult.Message;
            }
            ConnectionRing.Visibility = Visibility.Collapsed;
        }

        private void AggiornaInfo()
        {
            //Parameters***************************************************************************************
            FlowVolumeBox.SelectedValue     = MC608_Device.FlowrateUnit;
            FlowTimebaseBox.SelectedValue   = MC608_Device.FlowrateTimebase;
            CountersVolumeBox.SelectedValue = MC608_Device.CountersUnit;
            PulseVolumeBox.SelectedValue    = MC608_Device.PulseOutUnit;
            PulseVolumeUnit.Text            = "[" + MC608_Device.UnitsList[MC608_Device.PulseOutUnit] + "]";
            PulseVolume.Text                = MC608_Device.PulseOutVolume.ToString();
            PulseWidth.Text                 = MC608_Device.PulseOutWidth.ToString();
            FreqOutFS.Text                  = MC608_Device.Frequenza_FS.ToString();
            ProgOutBox.SelectedValue        = MC608_Device.ProgOutSetup;
            BatchingVolumeBox.Text          = MC608_Device.BatchingVolume.ToString();
            BatchingVolumeUT.Text           = MC608_Device.UnitsList[MC608_Device.CountersUnit];
            ProgInBox.SelectedValue         = MC608_Device.ProgInSetup;
            ContrastValue.Text              = MC608_Device.LCDContrast.ToString() + "%";
            ContrastSlider.Value            = MC608_Device.LCDContrast;
            BacklightValue.Text             = MC608_Device.LCDBacklight.ToString() + "%";
            BacklightSlider.Value           = MC608_Device.LCDBacklight;
            LcdTimeoutBox.Text              = MC608_Device.LCDBacklightTimeout.ToString();
            LanguageBox.SelectedValue       = MC608_Device.Language;
            AutoPowerOffBox.Text            = MC608_Device.AutoPowerOff.ToString();

            if (MC608_Device.MinFlowAlarm == 255)
            { 
                MinFlowAlarmBox.Text        = "--";
                MinFlowAlarmBox.IsEnabled   = false;
                MinFlowAlarmSW.IsOn         = false;
            }
            else
            { 
                MinFlowAlarmBox.Text        = MC608_Device.MinFlowAlarm.ToString();
                MinFlowAlarmBox.IsEnabled   = true;
                MinFlowAlarmSW.IsOn         = true;
            }

            if (MC608_Device.MaxFlowAlarm == 255)
            { 
                MaxFlowAlarmBox.Text        = "--";
                MaxFlowAlarmBox.IsEnabled   = false;
                MaxFlowAlarmSW.IsOn         = false;
            }
            else
            { 
                MaxFlowAlarmBox.Text        = MC608_Device.MaxFlowAlarm.ToString();
                MaxFlowAlarmBox.IsEnabled   = true;
                MaxFlowAlarmSW.IsOn         = true;
            }

            PeakCutSlider.Value             = MC608_Device.PeakCut;
            PeakCutValue.Text               = PeakCutSlider.Value.ToString() + "%";

            CutOffSlider.Value              = MC608_Device.CutOff;
            CutOffValue.Text                = CutOffSlider.Value.ToString() + "%";

            ByPassSlider.Value              = MC608_Device.FilterBypass;
            ByPassValue.Text                = ByPassSlider.Value.ToString() + "%";

            AverageSlider.Maximum           = MC608_Device.Damping;
            AverageSlider.Value             = MC608_Device.Average;
            AverageValue.Text               = AverageSlider.Value.ToString();

            DampingSlider.Value             = MC608_Device.Damping;
            DampingValue.Text               = DampingSlider.Value.ToString();

            if (MC608_Device.LineFrequency == 60)
            {
                Freq50Hz.IsChecked = false;
                Freq60Hz.IsChecked = true;
            }
            else
            {
                Freq50Hz.IsChecked = true;
                Freq60Hz.IsChecked = false;
            }
        }

        private void UpdateParamValues()
        {
            MC608_Device.FlowrateUnit       = (ushort)  FlowVolumeBox.SelectedValue;
            MC608_Device.FlowrateTimebase   = (ushort)  FlowTimebaseBox.SelectedValue;
            MC608_Device.CountersUnit       = (ushort)  CountersVolumeBox.SelectedValue;
            MC608_Device.PulseOutUnit       = (ushort)  PulseVolumeBox.SelectedValue;
            MC608_Device.PulseOutVolume     = (float)   Convert.ToDouble(PulseVolume.Text);
            MC608_Device.PulseOutWidth      =           Convert.ToUInt16(PulseWidth.Text);
            MC608_Device.Frequenza_FS       =           Convert.ToUInt16(FreqOutFS.Text);
            MC608_Device.ProgOutSetup       = (ushort)  ProgOutBox.SelectedValue;
            MC608_Device.ProgInSetup        = (ushort)  ProgInBox.SelectedValue;
            MC608_Device.BatchingVolume     = (float)   Convert.ToDouble(BatchingVolumeBox.Text);
            MC608_Device.LCDContrast        = (ushort)  ContrastSlider.Value;
            MC608_Device.LCDBacklight       = (ushort)  BacklightSlider.Value;
            MC608_Device.LCDBacklightTimeout =          Convert.ToUInt16(LcdTimeoutBox.Text);
            MC608_Device.Language           = (ushort)  LanguageBox.SelectedValue;
            MC608_Device.AutoPowerOff       =           Convert.ToUInt16(AutoPowerOffBox.Text);

            if (MinFlowAlarmSW.IsOn)
                MC608_Device.MinFlowAlarm   =           Convert.ToUInt16(MinFlowAlarmBox.Text);

            if(MaxFlowAlarmSW.IsOn)
                MC608_Device.MaxFlowAlarm   =           Convert.ToUInt16(MaxFlowAlarmBox.Text);

            MC608_Device.PeakCut            = (ushort)  PeakCutSlider.Value;
            MC608_Device.CutOff             = (ushort)  CutOffSlider.Value;
            MC608_Device.FilterBypass       = (ushort)  ByPassSlider.Value;
            MC608_Device.Average            = (ushort)  AverageSlider.Value;
            MC608_Device.Damping            = (ushort)  DampingSlider.Value;

            if (Freq50Hz.IsChecked == true)
            {
                MC608_Device.LineFrequency = 50;
            }
            else
            {
                MC608_Device.LineFrequency = 60;
            }
        }

        private ICommand _writeRegistercmd;
        public ICommand WriteRegistercmd
        {
            get
            {
                if (_writeRegistercmd == null)
                {
                    _writeRegistercmd = new RelayCommand(
                        param => WriteRegistersConfirm()
                            );
                }
                return _writeRegistercmd;
            }
        }

        private async void WriteRegistersConfirm()
        {
            var messageDialog = new MessageDialog("Are you sure to write configuration?");

            messageDialog.Title = "Write Configuration";
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(WriteRegisterConfirm)));
            messageDialog.Commands.Add(new UICommand("Cancel"));
            messageDialog.DefaultCommandIndex = 0;
            await messageDialog.ShowAsync();
        }

        private void WriteRegisterConfirm(IUICommand command)
        {
            UpdateParamValues();
            WriteRegister();
        }

        private void WriteRegister()
        {
            if(MbConnection.IsOpen)
            { 
                ModbusStatus = "Writing...";
                ConnectionRing.Visibility = Visibility.Visible;
                MbConnection.WriteRegisters(ComSetup.Address, Map.Registri_CMD3_16.Reverse_Modbus, Map.Registri_CMD3_16.LOW_FLOW_ALARM, 30);
                MbConnection.WriteRegistersCompleted += MbConnection_WriteRegistersCompleted;
            }
            else
                ModbusStatus = "No Connection";
        }

        private void MbConnection_WriteRegistersCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;
            ModbusStatus = cmd.WriteRegisters_CommandResult.Message;
            ConnectionRing.Visibility = Visibility.Collapsed;
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

        private void FlowVolumeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void CountersVolumeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FlowTimebaseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PulseVolumeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PulseVolumeUnit.Text = "[" + MC608_Device.UnitsList[(ushort)PulseVolumeBox.SelectedValue] + "]";
        }

        private void ProgOutBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if( (ushort) ProgOutBox.SelectedValue == 5) //Batching, disabilita la selezione del tipo di ingresso
            {
                ProgInBox.IsEnabled = false;
                BatchingPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ProgInBox.IsEnabled = true;
                BatchingPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ContrastSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ContrastValue.Text = ContrastSlider.Value.ToString() + "%";
        }

        private void BacklightSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            BacklightValue.Text = BacklightSlider.Value.ToString() + "%";
        }

        private void MinFlowAlarmSW_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    MC608_Device.MinFlowAlarm   = 5;
                    MinFlowAlarmBox.Text        = MC608_Device.MinFlowAlarm.ToString();
                    MinFlowAlarmBox.IsEnabled   = true;
                    MinFlowAlarmSW.IsOn         = true;
                }
                else
                {
                    MC608_Device.MinFlowAlarm   = 255;
                    MinFlowAlarmBox.Text        = "--";
                    MinFlowAlarmBox.IsEnabled   = false;
                    MinFlowAlarmSW.IsOn         = false;
                }
            }
        }

        private void MaxFlowAlarmSW_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    MC608_Device.MaxFlowAlarm   = 100;
                    MaxFlowAlarmBox.Text        = MC608_Device.MaxFlowAlarm.ToString();
                    MaxFlowAlarmBox.IsEnabled   = true;
                    MaxFlowAlarmSW.IsOn         = true;
                }
                else
                {
                    MC608_Device.MaxFlowAlarm   = 255;
                    MaxFlowAlarmBox.Text        = "--";
                    MaxFlowAlarmBox.IsEnabled   = false;
                    MaxFlowAlarmSW.IsOn         = false;
                }
            }
        }

        private void PeakCutSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            PeakCutValue.Text = PeakCutSlider.Value.ToString() + "%";
        }

        private void CutOffSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            CutOffValue.Text = CutOffSlider.Value.ToString() + "%";
        }

        private void ByPassSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ByPassValue.Text = ByPassSlider.Value.ToString() + "%";
        }

        private void AverageSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            AverageValue.Text = AverageSlider.Value.ToString();
        }

        private void DampingSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DampingValue.Text = DampingSlider.Value.ToString();
            AverageSlider.Maximum = DampingSlider.Value;
        }

        private void Freq50Hz_Checked(object sender, RoutedEventArgs e)
        {
            Freq60Hz.IsChecked = false;
        }

        private void Freq60Hz_Checked(object sender, RoutedEventArgs e)
        {
            Freq50Hz.IsChecked = false;
        }
    }
}
