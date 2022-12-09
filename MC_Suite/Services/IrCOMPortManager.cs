using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MC_Suite.Properties;
using MC_Suite.Euromag.Protocols;
using MC_Suite.Euromag.Protocols.StdCommands;

namespace MC_Suite.Services
{
    public class IrCOMPortManager : INotifyPropertyChanged
    {
        private DispatcherTimer PingTimer;

        public TargetVariablesFields Fields
        {
            get
            {
                return TargetVariablesFields.Instance;
            }
        }

        public IrCOMPortManager()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            PingTimer = dispatcherTimer;
            PingTimer.Interval = TimeSpan.FromMilliseconds(1000);
            PingTimer.Tick += PingTimer_Tick;
            PingTimer.Stop();
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
                    OnPropertyChanged("portHandler");
                }
            }
        }

        public List<StdCommand> CommandList;

        public enum IrConnectionStates
        {
            Stop,
            Ping,
            Retry,
            Working,
            Ready,
            Connected
        }

        public enum CommandState
        {
            ToSend,
            Sended,
            Retry,
            WaitForNew
        }

        public enum ReadingStateBundle
        {
            None,
            RamMeasBundleCmd,
            RamOthersBundleCmd,
            RamT1T2PressBundleCmd,
            EepParamsPageBundleCmd,
            EepInfoPageBundleCmd,
            EepRegisterPageBundleCmd,
            EepCalibPageBundleCmd,
            EepCustomizedDeviceInfoCmd,
            EepReadPulseVolume,
            EepReadPulseTechVolume,
            IoParamsBundleCmd,
            IoVariablesBundleCmd
        }

        private IDictionary<byte, String> _bundlesList;
        public IDictionary<byte, String> BundlesList
        {
            get
            {
                if (_bundlesList == null)
                {
                    _bundlesList = new Dictionary<byte, String>();
                    _bundlesList.Add(0, "--");
                    _bundlesList.Add(1, "Measures");
                    _bundlesList.Add(2, "FW & Log Info");
                    _bundlesList.Add(3, "Temperature & Pressure");
                    _bundlesList.Add(4, "Parameters");
                    _bundlesList.Add(5, "Converter & Sensor Info");
                    _bundlesList.Add(6, "Registers");
                    _bundlesList.Add(7, "Calibration");
                    _bundlesList.Add(8, "Custom Info");
                    _bundlesList.Add(9, "I/O Parameters");
                    _bundlesList.Add(10, "I/O Variables");
                }
                return _bundlesList;
            }
        }

        private static IrCOMPortManager _instance;
        public static IrCOMPortManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IrCOMPortManager();
                return _instance;
            }
        }

        private IrConnectionStates _IrConnectionStatus;
        public IrConnectionStates IrConnectionStatus
        {
            get { return _IrConnectionStatus; }
            set
            {
                if (value != _IrConnectionStatus)
                {
                    _IrConnectionStatus = value;
                    OnPropertyChanged("IrConnectionStatus");
                }
            }
        }

        private uint _connectionTimeout;
        public uint ConnectionTimeout
        {
            get { return _connectionTimeout; }
            set
            {
                if (value != _connectionTimeout)
                {
                    _connectionTimeout = value;
                    OnPropertyChanged("ConnectionTimeout");
                }
            }
        }

        private uint _timeoutCounter;
        public uint TimeoutCounter
        {
            get { return _timeoutCounter; }
            set
            {
                if (value != _timeoutCounter)
                {
                    _timeoutCounter = value;
                    OnPropertyChanged("TimeoutCounter");
                }
            }
        }

        private uint _successCounter;
        public uint SuccessCounter
        {
            get { return _successCounter; }
            set
            {
                if (value != _successCounter)
                {
                    _successCounter = value;
                    OnPropertyChanged("SuccessCounter");
                }
            }
        }

        private CommandState _commandCompleted;
        public CommandState CommandCompleted
        {
            get { return _commandCompleted; }
            set
            {
                if (value != _commandCompleted)
                {
                    _commandCompleted = value;
                    OnPropertyChanged("CommandCompleted");
                }
            }
        }

        private CommandState _extCommandCompleted;
        public CommandState ExtCommandCompleted
        {
            get { return _extCommandCompleted; }
            set
            {
                if (value != _extCommandCompleted)
                {
                    _extCommandCompleted = value;
                    OnPropertyChanged("ExtCommandCompleted");
                }
            }
        }    

        private ReadingStateBundle _readStateBundle;
        public ReadingStateBundle ReadStateBundle
        {
            get { return _readStateBundle; }
            set
            {
                if (value != _readStateBundle)
                {
                    _readStateBundle = value;
                    OnPropertyChanged("ReadStateBundle");
                }
            }
        }
        private ReadingStateBundle newReadStateBundle;

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

        public enum ComMode
        {
            Auto,
            Manual,
            Restart
        }

        public async void Open(Settings.COMPortItem Port,  ComMode Mode)
        {
            if ((Port == null) || (Port.ID == String.Empty))
            {
                var dialog = new MessageDialog("Please select COM port: OPTIONS->Settings->Connection", "COM Port not selected");
                await dialog.ShowAsync();
            }
            else
            {
                portHandler = new commPortHandler(Port.ID, 76800, 0, 8, 0, TimeSpan.FromMilliseconds(500));
                if (await portHandler.open())
                {
                    switch(Mode)
                    {
                        case ComMode.Auto:
                            Start();
                            break;
                        case ComMode.Manual:
                            IrConnectionStatus = IrConnectionStates.Ready;
                            break;
                        case ComMode.Restart:
                            IrConnectionStatus = IrConnectionStates.Working;
                            break;
                    }
                }
                else
                {
                    var dialog = new MessageDialog("Error Opening " + Port.Name + " Port");
                    await dialog.ShowAsync();
                }
            }
        }

        public void Close()
        {
            if ((IrConnectionStatus == IrConnectionStates.Ping) || (IrConnectionStatus == IrConnectionStates.Working) ||
                (IrConnectionStatus == IrConnectionStates.Connected))
            {
                //Stop();
                portHandler.close();
            }
        }

        public void Run()
        {
            if(ComRunning)
            {
                if(IrConnectionStatus ==  IrConnectionStates.Working)
                { 
                    if(CommandCompleted == CommandState.ToSend)
                    { 
                        switch(ReadStateBundle)
                        {
                            case ReadingStateBundle.None:
                                ReadStateBundle = ReadingStateBundle.None;
                                break;
                            case ReadingStateBundle.RamMeasBundleCmd:
                                Fields.RamMeasBundleCmd.setPortHandler(portHandler);
                                Fields.RamMeasBundleCmd.CommandCompleted += Cmd_CommandCompleted;
                                Fields.RamMeasBundleCmd.send();

                                if(newReadStateBundle < ReadingStateBundle.EepCustomizedDeviceInfoCmd)
                                    newReadStateBundle++;
                                else
                                    newReadStateBundle = ReadingStateBundle.RamOthersBundleCmd;

                                ReadStateBundle = newReadStateBundle;
                                break;
                            case ReadingStateBundle.RamOthersBundleCmd:
                                Fields.RamOthersBundleCmd.setPortHandler(portHandler);
                                Fields.RamOthersBundleCmd.CommandCompleted += Cmd_CommandCompleted;
                                Fields.RamOthersBundleCmd.send();
                                ReadStateBundle = ReadingStateBundle.RamMeasBundleCmd;
                                break;
                            case ReadingStateBundle.RamT1T2PressBundleCmd:
                                Fields.RamT1T2PressBundleCmd.setPortHandler(portHandler);
                                Fields.RamT1T2PressBundleCmd.CommandCompleted += Cmd_CommandCompleted;
                                Fields.RamT1T2PressBundleCmd.send();
                                ReadStateBundle = ReadingStateBundle.RamMeasBundleCmd;
                                break;
                            case ReadingStateBundle.EepInfoPageBundleCmd:
                                Fields.EepInfoPageBundleCmd.setPortHandler(portHandler);
                                Fields.EepInfoPageBundleCmd.CommandCompleted += Cmd_CommandCompleted;
                                Fields.EepInfoPageBundleCmd.send();
                                ReadStateBundle = ReadingStateBundle.RamMeasBundleCmd;
                                break;
                            case ReadingStateBundle.EepParamsPageBundleCmd:
                                Fields.EepParamsPageBundleCmd.setPortHandler(portHandler);
                                Fields.EepParamsPageBundleCmd.CommandCompleted += Cmd_CommandCompleted;
                                Fields.EepParamsPageBundleCmd.send();
                                ReadStateBundle = ReadingStateBundle.RamMeasBundleCmd;
                                break;
                            case ReadingStateBundle.EepCalibPageBundleCmd:
                                Fields.EepCalibPageBundleCmd.setPortHandler(portHandler);
                                Fields.EepCalibPageBundleCmd.CommandCompleted += Cmd_CommandCompleted;
                                Fields.EepCalibPageBundleCmd.send();
                                ReadStateBundle = ReadingStateBundle.RamMeasBundleCmd;
                                break;
                            case ReadingStateBundle.EepCustomizedDeviceInfoCmd:
                                Fields.EepCustomizedDeviceInfoCmd.setPortHandler(portHandler);
                                Fields.EepCustomizedDeviceInfoCmd.CommandCompleted += Cmd_CommandCompleted;
                                Fields.EepCustomizedDeviceInfoCmd.send();
                                ReadStateBundle = ReadingStateBundle.RamMeasBundleCmd;
                                break;
                            case ReadingStateBundle.EepRegisterPageBundleCmd:
                                Fields.EepRegisterPageBundleCmd.setPortHandler(portHandler);
                                Fields.EepRegisterPageBundleCmd.CommandCompleted += Cmd_CommandCompleted;
                                Fields.EepRegisterPageBundleCmd.send();
                                ReadStateBundle = ReadingStateBundle.RamMeasBundleCmd;
                                break;
                        }
                        CommandCompleted = CommandState.Sended;
                    }                    
                }
            }
        }

        #region Read Bundle

        public void ReadBundle(byte BundleIndex)
        {
            ReadingStateBundle Index = (ReadingStateBundle)BundleIndex;
            switch (Index)
            {
                case ReadingStateBundle.None:
                    break;
                case ReadingStateBundle.RamMeasBundleCmd:
                    Fields.RamMeasBundleCmd.setPortHandler(portHandler);
                    Fields.RamMeasBundleCmd.CommandCompleted += ReadBundle_CommandCompleted;
                    Fields.RamMeasBundleCmd.send();
                    break;
                case ReadingStateBundle.RamOthersBundleCmd:
                    Fields.RamOthersBundleCmd.setPortHandler(portHandler);
                    Fields.RamOthersBundleCmd.CommandCompleted += ReadBundle_CommandCompleted;
                    Fields.RamOthersBundleCmd.send();
                    break;
                case ReadingStateBundle.RamT1T2PressBundleCmd:
                    Fields.RamT1T2PressBundleCmd.setPortHandler(portHandler);
                    Fields.RamT1T2PressBundleCmd.CommandCompleted += ReadBundle_CommandCompleted;
                    Fields.RamT1T2PressBundleCmd.send();
                    break;
                case ReadingStateBundle.EepParamsPageBundleCmd:
                    Fields.EepParamsPageBundleCmd.setPortHandler(portHandler);
                    Fields.EepParamsPageBundleCmd.CommandCompleted += ReadBundle_CommandCompleted;
                    Fields.EepParamsPageBundleCmd.send();
                    break;
                case ReadingStateBundle.EepInfoPageBundleCmd:
                    Fields.EepInfoPageBundleCmd.setPortHandler(portHandler);
                    Fields.EepInfoPageBundleCmd.CommandCompleted += ReadBundle_CommandCompleted;
                    Fields.EepInfoPageBundleCmd.send();
                    break;
                case ReadingStateBundle.EepRegisterPageBundleCmd:
                    Fields.EepRegisterPageBundleCmd.setPortHandler(portHandler);
                    Fields.EepRegisterPageBundleCmd.CommandCompleted += ReadBundle_CommandCompleted;
                    Fields.EepRegisterPageBundleCmd.send();
                    break;
                case ReadingStateBundle.EepCalibPageBundleCmd:
                    Fields.EepCalibPageBundleCmd.setPortHandler(portHandler);
                    Fields.EepCalibPageBundleCmd.CommandCompleted += ReadBundle_CommandCompleted;
                    Fields.EepCalibPageBundleCmd.send();
                    break;
                case ReadingStateBundle.EepCustomizedDeviceInfoCmd:
                    Fields.EepCustomizedDeviceInfoCmd.setPortHandler(portHandler);
                    Fields.EepCustomizedDeviceInfoCmd.CommandCompleted += ReadBundle_CommandCompleted;
                    Fields.EepCustomizedDeviceInfoCmd.send();
                    break;
                case ReadingStateBundle.EepReadPulseVolume:
                    //Leggo qui perchè fuori dal bundle
                    ReadPulse = new ReadEEPROM(IrCOMPortManager.Instance.portHandler);
                    ReadPulse.Variable = Fields.PulseOutputVolume;
                    ReadPulse.CommandCompleted += ReadPulse_CommandCompleted;
                    ReadPulse.send();
                    break;
                case ReadingStateBundle.EepReadPulseTechVolume:
                    //Leggo qui perchè fuori dal bundle
                    ReadPulse = new ReadEEPROM(IrCOMPortManager.Instance.portHandler);
                    ReadPulse.Variable = Fields.PulseOutputTechUnit;
                    ReadPulse.CommandCompleted += ReadPulse_CommandCompleted;
                    ReadPulse.send();
                    break;
            }
        }

        private void ReadPulse_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            ReadEEPROM cmd = sender as ReadEEPROM;
            ReadBundle_CommandResult = cmd.Result;
            if (cmd.Result.Outcome == CommandResultOutcomes.CommandSuccess)
                IrConnectionStatus = IrConnectionStates.Connected;
        }

        private ReadEEPROM ReadPulse;

        private void ReadBundle_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            ReadVarBundle cmd = sender as ReadVarBundle;
            ReadBundle_CommandResult = cmd.Result;
            if (cmd.Result.Outcome == CommandResultOutcomes.CommandSuccess)
                IrConnectionStatus = IrConnectionStates.Connected;
        }

        private CommandResult _readBundle_CommandResult;
        public CommandResult ReadBundle_CommandResult
        {
            get { return _readBundle_CommandResult; }
            set
            {
                if (value != _readBundle_CommandResult)
                {
                    _readBundle_CommandResult = value;
                    OnReadBundleCompleted("ReadRegisters_CommandResult");
                }
            }
        }

        public event PropertyChangedEventHandler ReadBundleCompleted;

        private void OnReadBundleCompleted(string name)
        {
            ReadBundleCompleted?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Write Param

        public void WriteParam(IEEPROMvariable variable)
        {
            WriteEEPROM Cmd = new WriteEEPROM( portHandler );
            Cmd.Variable = variable;
            Cmd.CommandCompleted += WriteParam_CommandCompleted;
            Cmd.send();
        }

        private void WriteParam_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            WriteEEPROM cmd = sender as WriteEEPROM;
            WriteParam_CommandResult = cmd.Result;
        }

        private CommandResult _writeParam_CommandResult;
        public CommandResult WriteParam_CommandResult
        {
            get { return _writeParam_CommandResult; }
            set
            {
                if (value != _writeParam_CommandResult)
                {
                    _writeParam_CommandResult = value;
                    OnWriteParamCompleted("ReadRegisters_CommandResult");
                }
            }
        }

        public event PropertyChangedEventHandler WriteParamCompleted;

        private void OnWriteParamCompleted(string name)
        {
            WriteParamCompleted?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private void EepromCmd_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            ReadEEPROM cmd = sender as ReadEEPROM;
            if (cmd.Result != null)
            {
                //if(cmd.completed.Outcome == CommandResultOutcomes.CommandSuccess)
                CommandCompleted = CommandState.WaitForNew;
            }
        }

        private void RamCmd_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            ReadRAM cmd = sender as ReadRAM;
            if (cmd.Result != null)
            {
                //if(cmd.completed.Outcome == CommandResultOutcomes.CommandSuccess)
                CommandCompleted = CommandState.WaitForNew;
            }
        }

        private void Cmd_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            ReadVarBundle cmd = sender as ReadVarBundle;
            if (cmd.Result != null)
            {
                //if (cmd.completed.Outcome == CommandResultOutcomes.CommandSuccess)
                    CommandCompleted = CommandState.WaitForNew;
            }
        }

        private void Cmd_Completed(object sender, CommandCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess)
                {
                    CommandCompleted = CommandState.WaitForNew;
                }
            }
        }

        private void Ping()
        {
            ConnectionTimeout   = 0;
            IrConnectionStatus    = IrConnectionStates.Ping;
            PingTimer.Interval  = TimeSpan.FromMilliseconds(1000);            
            ReadRamParamCmd(Fields.FwVersion);            
        }

        private void Retry()
        {
            IrConnectionStatus = IrConnectionStates.Retry;
            PingTimer.Interval = TimeSpan.FromMilliseconds(1000);
            ReadRamParamCmd(Fields.FwVersion);
        }

        public void Start()
        {
            if (IrConnectionStatus == IrConnectionStates.Stop)
            {
                //Start Connection
                CommandCompleted    = CommandState.ToSend;
                ExtCommandCompleted = CommandState.WaitForNew;
                PingTimer.Interval  = TimeSpan.FromMilliseconds(1000);
                PingTimer.Start();
                TimeoutCounter = 0;
                SuccessCounter = 0;
                ComRunning  = true;
                CommandList = new List<StdCommand>();
                Ping();
            }
        }

        public void Stop()
        {
            if ((IrConnectionStatus == IrConnectionStates.Ping) || (IrConnectionStatus == IrConnectionStates.Working) ||
                (IrConnectionStatus == IrConnectionStates.Connected))
            {
                IrConnectionStatus              = IrConnectionStates.Stop;
                ComRunning                      = false;
                Settings.Instance.UpdateRunning = false;

                if (PingTimer != null)
                    PingTimer.Stop();
                if(CommandList != null)
                    CommandList.Clear();
            }
        }

        void ReadRamParamCmd(ITargetVariable var)
        {
            ReadRAM ReadRamParam = new ReadRAM(portHandler);
            ReadRamParam.Variable = var;
            ReadRamParam.CommandCompleted += ReadRamParam_CommandCompleted;
            CommandCompleted = CommandState.Sended;
            ReadRamParam.send();
        }

        private void ReadRamParam_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            ReadRAM cmd = sender as ReadRAM;
            if( cmd.Result != null )
            {
                if(cmd.Result.Outcome == CommandResultOutcomes.CommandSuccess)
                    CommandCompleted = CommandState.WaitForNew;
                else
                    CommandCompleted = CommandState.Retry;
            }
        }

        private void PingTimer_Tick(object sender, object e)
        {
            Run();

            switch (IrConnectionStatus)
            {
                case IrConnectionStates.Ping:
                    if (CommandCompleted == CommandState.WaitForNew)
                    {
                        ConnectionTimeout = 0;
                        ReadStateBundle = ReadingStateBundle.RamMeasBundleCmd;
                        IrConnectionStatus = IrConnectionStates.Working;
                        PingTimer.Interval = TimeSpan.FromMilliseconds(500);
                        CommandCompleted = CommandState.ToSend;
                        ExtCommandCompleted = CommandState.WaitForNew;
                        CommandList.Clear();
                    }
                    else if(CommandCompleted == CommandState.Retry)
                    {                        
                        Ping();
                    }
                    else if (ConnectionTimeout++ >= 5)
                    {
                        Ping();
                    }
                    break;
                case IrConnectionStates.Retry:
                    if (CommandCompleted == CommandState.WaitForNew)
                    {
                        ConnectionTimeout = 0;
                        IrConnectionStatus = IrConnectionStates.Working;
                        PingTimer.Interval = TimeSpan.FromMilliseconds(500);
                        CommandCompleted = CommandState.ToSend;
                        ExtCommandCompleted = CommandState.WaitForNew;
                    }
                    else if(CommandCompleted == CommandState.Retry)
                    {
                        Retry();
                        ConnectionTimeout++;
                    }
                    else
                        ConnectionTimeout++;

                    if (ConnectionTimeout >= 5)
                    {
                        Ping();
                        TimeoutCounter += 1;
                    }
                    break;
                case IrConnectionStates.Working:
                    if(ReadStateBundle != ReadingStateBundle.None)
                    {
                        if (CommandCompleted == CommandState.WaitForNew)
                        {
                            if (CommandList.Count != 0)
                            {
                                if (ExtCommandCompleted == CommandState.WaitForNew)
                                {
                                    StdCommand cmd = CommandList.Last();
                                    cmd.send();
                                    ExtCommandCompleted = CommandState.Sended;
                                    SuccessCounter += 1;
                                    ConnectionTimeout = 0;
                                }
                                else
                                    ConnectionTimeout++;
                            }
                            else
                            { 
                                SuccessCounter += 1;
                                ConnectionTimeout = 0;
                                CommandCompleted = CommandState.ToSend;
                            }
                        }
                        else
                            ConnectionTimeout++;
                    }
                    else
                        ConnectionTimeout = 0;

                    if (ConnectionTimeout >= 10)
                    {
                        Retry();
                        TimeoutCounter += 1;
                    }
                    break;
                case IrConnectionStates.Stop:
                    ConnectionTimeout = 0;
                    break;
            }

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

        private const uint Timeout = 50;
        private bool ComRunning;
    }
}
