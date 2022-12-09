using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using MC_Suite.Properties;
using MC_Suite.Modbus;


namespace MC_Suite.Services
{
    public class MbCOMPortManager : INotifyPropertyChanged
    {
        private static MbCOMPortManager _instance;
        public static MbCOMPortManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MbCOMPortManager();
                return _instance;
            }
        }

        private commPortHandler _portHandler;
        public commPortHandler portHandler
        {
            get { return _portHandler; }
            set
            {
                if (value != _portHandler)
                {
                    _portHandler = value;
                }
            }
        }

        private uint _tries;
        public uint Tries
        {
            get { return _tries; }
            set
            {
                if (value != _tries)
                {
                    _tries = value;
                }
            }
        }

        #region Connection Status

        public enum MbConnectionStates
        {
            Stop,
            Ready,
            Working
        }

        private MbConnectionStates _MbConnectionStatus;
        public MbConnectionStates MbConnectionStatus
        {
            get { return _MbConnectionStatus; }
            set
            {
                if (value != _MbConnectionStatus)
                {
                    _MbConnectionStatus = value;
                    OnPropertyChanged("MbConnectionStatus");
                }
            }
        }

        #endregion

        #region Open/Close Connection

        public async Task<bool> Open( Settings.COMPortItem Port)
        {
            if ((Port == null) || (Port.ID == String.Empty))
            {
                var dialog = new MessageDialog("Please select COM port: OPTIONS->Settings->Connection", "COM Port not selected");
                await dialog.ShowAsync();
                return false;
            }
            else if ((portHandler != null) && (portHandler.IsOpen))
            {                
                return true;
            }
            else
            {
                portHandler = new commPortHandler(Port.ID, Settings.Instance.BaudRate,
                                                  Settings.Instance.Parity, Settings.Instance.DataBits,
                                                  Settings.Instance.StopBits, Settings.Instance.TimeOut);
                if (await portHandler.open())
                {
                    MbConnectionStatus = MbConnectionStates.Ready;
                    return true;
                }
                else
                {
                    var dialog = new MessageDialog("Error Opening " + Port.Name + " Port");
                    await dialog.ShowAsync();
                    return false;
                }
            }
        }

        public bool Close()
        {
            if (this.IsOpen)
            {
                portHandler.close();
                MbConnectionStatus = MbConnectionStates.Stop;
                return true;
            }
            return false;
        }

        public bool IsOpen
        {
            get
            {
                if (portHandler != null)
                    return portHandler.IsOpen;
                else
                    return false;
            }
        }

        #endregion

        #region Read Registers

        public void ReadRegisters(byte Address, ModbusRegister First, ModbusRegister Last, uint Tries)
        {
            Protocol.Instance.ModbusCOMMAND_3_4(portHandler, Address, First, Last, Tries);
            Protocol.Instance.CommandCompleted += ReadRegisters_CommandCompleted;
        }

        private void ReadRegisters_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            ReadRegisters_CommandResult = Protocol.Instance.CommandResult;
        }

        private Protocol.ModbusCommandResult _readRegisters_CommandResult;
        public Protocol.ModbusCommandResult ReadRegisters_CommandResult
        {
            get { return _readRegisters_CommandResult; }
            set
            {
                if (value != _readRegisters_CommandResult)
                {
                    _readRegisters_CommandResult = value;
                    OnReadCommandCompleted("ReadRegisters_CommandResult");
                }
            }
        }

        public event PropertyChangedEventHandler ReadRegistersCompleted;

        private void OnReadCommandCompleted(string name)
        {
            ReadRegistersCompleted?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Write Registers

        public void WriteRegisters(byte Address, ModbusRegister First, ModbusRegister Last, uint Tries)
        {
            Protocol.Instance.ModbusCOMMAND_16(portHandler, Address, First, Last, Tries);
            Protocol.Instance.CommandCompleted += WriteRegisters_CommandCompleted;
        }

        private void WriteRegisters_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            WriteRegisters_CommandResult = Protocol.Instance.CommandResult;
        }

        private Protocol.ModbusCommandResult _writeRegisters_CommandResult;
        public Protocol.ModbusCommandResult WriteRegisters_CommandResult
        {
            get { return _writeRegisters_CommandResult; }
            set
            {
                if (value != _writeRegisters_CommandResult)
                {
                    _writeRegisters_CommandResult = value;
                    OnWriteCommandCompleted("WriteRegisters_CommandResult");
                }
            }
        }

        public event PropertyChangedEventHandler WriteRegistersCompleted;

        private void OnWriteCommandCompleted(string name)
        {
            WriteRegistersCompleted?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Send Command

        public void SendCommand(byte Address, Command_mdb Command, byte valore, byte ripetizioni)
        {
            Command.Valore = valore;
            ushort[] cmd_mdb = new ushort[1];
            cmd_mdb[0] = Command.reg_value;

            Protocol.Instance.ModbusCOMMAND_16(portHandler, Address, Command.address, (ushort)cmd_mdb.Length, cmd_mdb, (uint)ripetizioni);
            Protocol.Instance.CommandCompleted += SendCommand_CommandCompleted;
        }

        private void SendCommand_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            SendCommand_CommandResult = Protocol.Instance.CommandResult;
        }

        private Protocol.ModbusCommandResult _sendCommand_CommandResult;
        public Protocol.ModbusCommandResult SendCommand_CommandResult
        {
            get { return _sendCommand_CommandResult; }
            set
            {
                if (value != _sendCommand_CommandResult)
                {
                    _sendCommand_CommandResult = value;
                    OnSendCommandCompleted("SendCommand_CommandResult");
                }
            }
        }

        public event PropertyChangedEventHandler SendCommandCompleted;

        private void OnSendCommandCompleted(string name)
        {
            SendCommandCompleted?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private bool Updated;
        private DispatcherTimer UpdateTimer;
        public void InitUpdateTimer()
        {
            UpdateTimer = new DispatcherTimer();
            UpdateTimer.Interval = TimeSpan.FromSeconds(1);
            UpdateTimer.Tick += UpdateTimer_Tick; ;
            UpdateTimer.Stop();
            ParametersReaded = false;
            Updated = true;
        }

        public void UpdateTimerStartStop()
        {
            if (UpdateTimer.IsEnabled == false)
            {
                UpdateTimer.Start();
                Updated = true;
            }
            else
            {
                UpdateTimer.Stop();
                Updated = false;
            }
        }

        public void UpdateTimerStart()
        {
            if (UpdateTimer.IsEnabled == false)
            {
                UpdateTimer.Start();
                Updated = true;
            }
        }

        public void UpdateTimerStop()
        {
            if (UpdateTimer.IsEnabled == true)
            {
                UpdateTimer.Stop();
                Updated = false;
            }
        }

        private bool _parametersReaded;
        public bool ParametersReaded
        {
            get { return _parametersReaded; }
            set
            {
                _parametersReaded = value;
                OnPropertyChanged("ParametersReaded");
            }
        }

        private void UpdateTimer_Tick(object sender, object e)
        {
            Update();
        }

        private void Update()
        {
            if (Updated)
            {
                if (ParametersReaded)
                    ReadMeasures();
                else
                    ReadRegisters();

                Updated = false;
            }
        }

        private bool ReadMeasures()
        {
            if (this.IsOpen)
            {
                this.ReadRegisters(Settings.Instance.Address, Map.Registri_CMD4.Measure_m_s, Map.Registri_CMD4.ExpPression, 3);
                this.ReadRegistersCompleted += UpdateCompleted;
                return true;
            }
            else
                return false;
        }

        private bool ReadRegisters()
        {
            if (this.IsOpen)
            {
                this.ReadRegisters(Settings.Instance.Address, Map.Registri_CMD3_16.Release_ModBus, Map.Registri_CMD3_16.ResetTotN, 3);
                this.ReadRegistersCompleted += UpdateCompleted;
                return true;
            }
            else
                return false;
        }

        private void UpdateCompleted(object sender, PropertyChangedEventArgs e)
        {
            MbCOMPortManager cmd = sender as MbCOMPortManager;
            if (cmd.ReadRegisters_CommandResult.Result == Protocol.ModbusTransferResult.success)
            {
                UpdateCompleted_CommandResult = true;
                ParametersReaded = true;
            }
            else
                UpdateCompleted_CommandResult = false;

            Updated = true;
        }

        private bool _updateCompleted_CommandResult;
        public bool UpdateCompleted_CommandResult
        {
            get { return _updateCompleted_CommandResult; }
            set
            {
                _updateCompleted_CommandResult = value;
                OnUpdateCommandCompleted("UpdateCompleted_CommandResult");
            }
        }

        public event PropertyChangedEventHandler UpdateCommandCompleted;

        private void OnUpdateCommandCompleted(string name)
        {
            UpdateCommandCompleted?.Invoke(this, new PropertyChangedEventArgs(name));
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
