using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using System.Windows.Input;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MC_Suite.Services;
using Windows.UI.Core;

namespace MC_Suite.Properties
{
    public class Settings : INotifyPropertyChanged
    {
        private static Settings _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settings();
                return _instance;
            }
        }

        #region Interface Settings

        public class Converter
        {
            public bool Connected { get; set; }
            public bool ConfigEnabled { get; set; }
        }

        public class ConverterModel
        {
            public Converter MC406;
            public Converter MC608;
        }

        private ConverterModel _converterSelection;
        public ConverterModel ConverterSelection
        {
            get { return _converterSelection; }
            set
            {
                if (value != _converterSelection)
                {
                    _converterSelection = value;
                    OnPropertyChanged("ConverterSelection");
                }
            }
        }

        public bool _converterSelectionChanged;
        public bool ConverterSelectionChanged
        {
            get { return _converterSelectionChanged; }
            set
            {
                if (value != _converterSelectionChanged)
                {
                    _converterSelectionChanged = value;
                    OnPropertyChanged("ConverterSelectionChanged");
                }
            }
        }

        private Visibility _mc406Visibility;
        public Visibility MC406Visibility
        {
            get { return _mc406Visibility; }
            set
            {
                if (value != _mc406Visibility)
                {
                    _mc406Visibility = value;
                    OnPropertyChanged("MC406Visibility");
                }
            }
        }

        private Visibility _mc608Visibility;
        public Visibility MC608Visibility
        {
            get { return _mc608Visibility; }
            set
            {
                if (value != _mc608Visibility)
                {
                    _mc608Visibility = value;
                    OnPropertyChanged("MC608Visibility");
                }
            }
        }

        private bool _configurationPageReady;
        public bool ConfigurationPageReady
        {
            get { return _configurationPageReady; }
            set
            {
                if (value != _configurationPageReady)
                {
                    _configurationPageReady = value;
                    OnPropertyChanged("ConfigurationPageReady");
                }
            }
        }

        private bool _verificator406PageReady;
        public bool Verificator406PageReady
        {
            get { return _verificator406PageReady; }
            set
            {
                if (value != _verificator406PageReady)
                {
                    _verificator406PageReady = value;
                    OnPropertyChanged("Verificator406PageReady");
                }
            }
        }

        private bool _verificator608PageReady;
        public bool Verificator608PageReady
        {
            get { return _verificator608PageReady; }
            set
            {
                if (value != _verificator608PageReady)
                {
                    _verificator608PageReady = value;
                    OnPropertyChanged("Verificator608PageReady");
                }
            }
        }

        private bool _updateRunning;
        public bool UpdateRunning
        {
            get { return _updateRunning; }
            set
            {
                if (value != _updateRunning)
                {
                    _updateRunning = value;
                    OnPropertyChanged("ConfigurationPageReady");
                }
            }
        }

        private bool _otherMeasuresChanged;
        public bool OtherMeasuresChanged
        {
            get { return _otherMeasuresChanged; }
            set
            {
                if (value != _otherMeasuresChanged)
                {
                    _otherMeasuresChanged = value;
                    OnPropertyChanged("OtherMeasuresChanged");
                }
            }
        }

        private bool _verificatorRunning;
        public bool VerificatorRunning
        {
            get { return _verificatorRunning; }
            set
            {
                if (value != _verificatorRunning)
                {
                    _verificatorRunning = value;
                    OnPropertyChanged("VerificatorRunning");
                }
            }
        }

        private bool _sensorOnly;
        public bool SensorOnly
        {
            get { return _sensorOnly; }
            set
            {
                if (value != _sensorOnly)
                {
                    _sensorOnly = value;
                    OnPropertyChanged("SensorOnly");
                }
            }
        }


        #endregion

        #region Com Port

        private byte _protocol;
        public byte Protocol
        {
            get { return _protocol; }
            set
            {
                if (value != _protocol)
                {
                    _protocol = value;
                    OnPropertyChanged("Protocol");
                }
            }
        }

        private byte _protocol608;
        public byte Protocol608
        {
            get { return _protocol608; }
            set
            {
                if (value != _protocol608)
                {
                    _protocol608 = value;
                    OnPropertyChanged("Protocol608");
                }
            }
        }

        private IDictionary<byte, String> _protocolList;
        public IDictionary<byte, String> ProtocolList
        {
            get
            {
                if (_protocolList == null)
                {
                    _protocolList = new Dictionary<byte, String>();
                    _protocolList.Add(0, "IrCOM");
                    _protocolList.Add(1, "Modbus");
                }
                return _protocolList;
            }
        }

        public class COMPortItem
        {
            public int Index { get; set; }
            public string ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public uint BaudRate { get; set; }
        }

        public ObservableCollection<Settings.COMPortItem> COMPorts = new ObservableCollection<Settings.COMPortItem>();
        public List<Settings.COMPortItem> COMPortsList = new List<Settings.COMPortItem>();

        private COMPortItem _comPort;
        public COMPortItem ComPort
        {
            get
            {
                if(_comPort == null)
                {
                    _comPort = new COMPortItem();
                }
                return _comPort;
            }
            set
            {
                if (value != _comPort)
                {
                    _comPort = value;
                }
            }
        }

        private COMPortItem _comPort608;
        public COMPortItem ComPort608
        {
            get
            {
                if (_comPort608 == null)
                {
                    _comPort608 = new COMPortItem();
                }
                return _comPort608;
            }
            set
            {
                if (value != _comPort608)
                {
                    _comPort608 = value;
                }
            }
        }

        private COMPortItem _simulatorComPort;
        public COMPortItem SimulatorComPort
        {
            get
            {
                if (_simulatorComPort == null)
                {
                    _simulatorComPort = new COMPortItem();
                }
                return _simulatorComPort;
            }
            set
            {
                if (value != _simulatorComPort)
                {
                    _simulatorComPort = value;
                }
            }
        }

        private COMPortItem _comPortSlave;
        public COMPortItem ComPortSlave
        {
            get
            {
                if (_comPortSlave == null)
                {
                    _comPortSlave = new COMPortItem();
                }
                return _comPortSlave;
            }
            set
            {
                if (value != _comPortSlave)
                {
                    _comPortSlave = value;
                }
            }
        }

        private bool _comPortUpdated;
        public bool ComPortUpdated
        {
            get
            {
                return _comPortUpdated;
            }
            set
            {
                if (value != _comPortUpdated)
                {
                    _comPortUpdated = value;
                }
            }
        }

        private string _portFilter;
        public string PortFilter
        {
            get
            {
                return _portFilter;
            }
            set
            {
                if (value != _portFilter)
                {
                    _portFilter = value;
                }
            }
        }

        private DeviceInformationCollection _devicePorts;
        public DeviceInformationCollection DevicePorts
        {
            get
            {
                return _devicePorts;
            }
            set
            {
                if (value != _devicePorts)
                {
                    _devicePorts = value;
                }
            }
        }

        public async void getPortNames()
        {        
            int i, j, k;

            PortFilter = SerialDevice.GetDeviceSelector();
            DevicePorts = await DeviceInformation.FindAllAsync(PortFilter);

            j = k = 0;

            COMPortsList.Clear();            
            SimulatorComPort = null;

            if (DevicePorts.Count > 0)
            {
                for (i = 0; i < DevicePorts.Count; i++)
                {
                    if (DevicePorts[i].Name.Contains("USB-RS485 Cable"))
                    {
                        COMPortsList.Add(new Properties.Settings.COMPortItem() { Index = 0, ID = DevicePorts[i].Id, Name = "COM-RS485-" + j.ToString(), Description = DevicePorts[i].Name });
                        j++;
                    }
                    else if (DevicePorts[i].Name.Contains("FT232R USB UART"))
                    {
                        COMPortsList.Add(new Properties.Settings.COMPortItem() { Index = 1, ID = DevicePorts[i].Id, Name = "COM-IR-" + k.ToString(), Description = DevicePorts[i].Name });
                        k++;
                    }
                    else if (DevicePorts[i].Name.Contains("TTL232R-3V3"))
                    {
                        SimulatorComPort = new Properties.Settings.COMPortItem() { Index = 2, ID = DevicePorts[i].Id, Name = "COM-TTL", Description = DevicePorts[i].Name };
                    }
                }
            }
        }       

        #endregion

        #region Parametri_ModBus

        private byte _address;
        public byte Address
        {
            get { return _address; }
            set
            {
                if (value != _address)
                {
                    _address = value;
                    OnPropertyChanged("Address");
                }
            }
        }

        private IDictionary<uint, String> _baudRateList;
        public IDictionary<uint, String> BaudRateList
        {
            get
            {
                if (_baudRateList == null)
                {
                    _baudRateList = new Dictionary<uint, String>();
                    _baudRateList.Add(2400, "2400 bps");
                    _baudRateList.Add(4800, "4800 bps");
                    _baudRateList.Add(9600, "9600 bps");
                    _baudRateList.Add(19200, "19200 bps");
                    _baudRateList.Add(38400, "38400 bps");
                    _baudRateList.Add(57600, "57600 bps");
                    _baudRateList.Add(115200, "115200 bps");
                }
                return _baudRateList;
            }
        }    

        private uint _baudRate;
        public uint BaudRate
        {
            get { return _baudRate; }
            set
            {
                if (value != _baudRate)
                {
                    _baudRate = value;
                    OnPropertyChanged("BaudRate");
                }
            }
        }

        private IDictionary<byte, String> _parityList;
        public IDictionary<byte, String> ParityList
        {
            get
            {
                if (_parityList == null)
                {
                    _parityList = new Dictionary<byte, String>();
                    _parityList.Add(0, "None");
                    _parityList.Add(1, "Odd");
                    _parityList.Add(2, "Even");
                    _parityList.Add(3, "Mark");
                    _parityList.Add(4, "Space");
                }
                return _parityList;
            }
        }

        private byte _parity;
        public byte Parity
        {
            get { return _parity; }
            set
            {
                if (value != _parity)
                {
                    _parity = value;
                    OnPropertyChanged("Parity");
                }
            }
        }

        private IDictionary<ushort, String> _dataBitsList;
        public IDictionary<ushort, String> DataBitsList
        {
            get
            {
                if (_dataBitsList == null)
                {
                    _dataBitsList = new Dictionary<ushort, String>();
                    _dataBitsList.Add(5, "5");
                    _dataBitsList.Add(6, "6");
                    _dataBitsList.Add(7, "7");
                    _dataBitsList.Add(8, "8");
                    _dataBitsList.Add(9, "9");
                }
                return _dataBitsList;
            }
        }

        private ushort _dataBits;
        public ushort DataBits
        {
            get { return _dataBits; }
            set
            {
                if (value != _dataBits)
                {
                    _dataBits = value;
                    OnPropertyChanged("DataBits");
                }
            }
        }

        private IDictionary<byte, String> _stopBitsList;
        public IDictionary<byte, String> StopBitsList
        {
            get
            {
                if (_stopBitsList == null)
                {
                    _stopBitsList = new Dictionary<byte, String>();
                    _stopBitsList.Add(0, "1");
                    _stopBitsList.Add(1, "1.5");
                    _stopBitsList.Add(2, "2");
                }
                return _stopBitsList;
            }
        }

        private byte _stopBits;
        public byte StopBits
        {
            get { return _stopBits; }
            set
            {
                if (value != _stopBits)
                {
                    _stopBits = value;
                    OnPropertyChanged("StopBits");
                }
            }
        }

        private TimeSpan _timeOut;
        public TimeSpan TimeOut
        {
            get { return _timeOut; }
            set
            {
                if (value != _timeOut)
                {
                    _timeOut = value;
                    OnPropertyChanged("TimeOut");
                }
            }
        }

        #endregion

        /// <summary>
        /// Report row to setup the PrintDocument
        /// </summary>
        private ReportLine _reportToPrint;
        public ReportLine ReportToPrint
        {
            get { return _reportToPrint; }
            set
            {
                if (value != _reportToPrint)
                {
                    _reportToPrint = value;
                }
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

    }
}
