using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Popups;

namespace MC_Suite.Services
{
    public class SerialPort
    {
        /// <summary>
        /// Create the specified COM port to make a connection
        /// </summary>
        /// <param name="PortID">ID of port to open</param>
        /// <param name="baudRate">Baud rate of COM port</param>
        /// <param name="parity">Type of data parity</param>
        /// <param name="dataBits">Number of data bits</param>
        /// <param name="stopBits">Number of stop bits</param>
        ///

        public SerialPort(string PortID, uint baudRate,
            SerialParity parity, ushort dataBits,
            SerialStopBitCount stopBits, TimeSpan readTimeout)
        {
            this.PortID         = PortID;
            this.baudRate       = baudRate;
            this.dataBits       = dataBits;
            this.stopBits       = stopBits;
            this.ReadTimeout    = readTimeout;
            this.IsOpen         = false;
        }

        private string _PortID;
        public string PortID
        {
            get { return _PortID; }
            set
            {
                if (value != _PortID)
                {
                    _PortID = value;
                }
            }
        }

        private uint _baudRate;
        public uint baudRate
        {
            get { return _baudRate; }
            set
            {
                if (value != _baudRate)
                {
                    _baudRate = value;
                }
            }
        }

        private SerialParity _parity;
        public SerialParity parity
        {
            get { return _parity; }
            set
            {
                if (value != _parity)
                {
                    _parity = value;
                }
            }
        }

        private ushort _dataBits;
        public ushort dataBits
        {
            get { return _dataBits; }
            set
            {
                if (value != _dataBits)
                {
                    _dataBits = value;
                }
            }
        }

        private SerialStopBitCount _stopBits;
        public SerialStopBitCount stopBits
        {
            get { return _stopBits; }
            set
            {
                if (value != _stopBits)
                {
                    _stopBits = value;
                }
            }
        }

        private TimeSpan _readTimeout;
        public TimeSpan ReadTimeout
        {
            get { return _readTimeout; }
            set
            {
                if (value != _readTimeout)
                {
                    _readTimeout = value;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////
        public async Task<bool> Open()
        {
            // Close open port
            //Close();
            if(!this.IsOpen)
            {
                this.serialDevice = null;                

                //do
                //{
                    //this.serialDevice = await SerialDevice.FromIdAsync(deviceInfo.Id);
                    this.serialDevice = await SerialDevice.FromIdAsync(this.PortID);
                //} while (this.serialDevice == null);

                    // If serial device is valid...
                if (this.serialDevice != null)
                {
                    // Setup serial port configuration
                    this.serialDevice.StopBits = this.stopBits;
                    this.serialDevice.Parity = this.parity;
                    this.serialDevice.BaudRate = this.baudRate;
                    this.serialDevice.DataBits = this.dataBits;
                    this.serialDevice.ReadTimeout = this.ReadTimeout;

                    // Create a single device writer for this port connection
                    this.dataWriterObject = new DataWriter(this.serialDevice.OutputStream);

                    // Create a single device reader for this port connection
                    this.dataReaderObject = new DataReader(this.serialDevice.InputStream);

                    // Allow partial reads of the input stream
                    this.dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

                    // Port is now open
                    this.IsOpen = true;
                }
            }

            return this.IsOpen;
        }

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Flag that indicates if COM port is open
        /// </summary>
        public bool IsOpen { get; private set; }

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Close the open port connection to the current device
        /// </summary>
        public void Close()
        {
            if(this.IsOpen)
            { 
                // If serial device defined...
                if (this.serialDevice != null)
                {
                    try
                    { 
                        // Dispose and clear device
                        this.serialDevice.Dispose();
                        this.serialDevice = null;
                    }
                    catch { }
                }

                // If data reader defined...
                if (this.dataReaderObject != null)
                {
                    try
                    {
                        // Detatch reader stream
                        //this.dataReaderObject.DetachStream();

                        // Dispose and clear data reader
                        this.dataReaderObject.Dispose();
                        this.dataReaderObject = null;
                    }
                    catch { }
                }

                // If data writer defined...
                if (this.dataWriterObject != null)
                {
                    try
                    {
                        // Detatch reader stream
                        //this.dataWriterObject.DetachStream();

                        // Dispose and clear data reader
                        this.dataWriterObject.Dispose();
                        this.dataWriterObject = null;
                    }
                    catch { }
                }

                // Port now closed
                this.IsOpen = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Write data to the open COM port
        /// </summary>
        /// <param name="data">Array of data byes to be written</param>

        public bool dataSent { get; set; }
        public bool dataReceived { get; set; }
        public bool completed { get; set; }

        private int wTries;
        public async void WriteAsync(byte[] data)
        {
            UInt32 bytesWritten;
            this.dataReceived = false;
            bool complete = false;
            wTries = 20;
            IBuffer SendArray = data.AsBuffer();
            bytesWritten = 0;

            while (wTries > 0)
            { 
                try
                {
                    this.dataWriterObject.WriteBuffer(SendArray);
                    // Transfer data to the serial device now
                    bytesWritten = await this.dataWriterObject.StoreAsync();
                    if (bytesWritten > 0)
                    {
                        complete = true;
                        wTries = 0;
                        break;
                    }
                }
                catch
                {
                    wTries--;
                }
            }
            this.completed = complete;
        }

        private uint UnreadedBytes;
        public async void WriteReadAsync(byte[] data)
        {
            UInt32 bytesWritten;

            this.dataSent = false;
            this.dataReceived = false;
            this.completed = false;

            IBuffer SendArray = data.AsBuffer();
            try
            {
                
                this.dataWriterObject.WriteBuffer(SendArray);
                // Transfer data to the serial device now
                bytesWritten = await this.dataWriterObject.StoreAsync();
            }
            catch
            {
                this.completed = false;
                return;
            }

            if(bytesWritten > 0)
            { 
                this.dataSent = true;

                //Reading answer*****************************************************************************************                
                Task<UInt32> loadAsyncTask;
                IBuffer ReceiveArray;
                UInt32 bytesRead = 0;
                try
                {
                    uint ReadBufferLength = 524;
                    uint Tries = 20;

                    while (Tries > 0)
                    {
                        loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask();

                        bytesRead = await loadAsyncTask;

                        UnreadedBytes = dataReaderObject.UnconsumedBufferLength;

                        if (bytesRead > 0)
                        {
                            ReceiveArray = dataReaderObject.ReadBuffer(bytesRead);

                            ReadDataBuffer = new byte[bytesRead];
                            ReadDataBuffer = ReceiveArray.ToArray();
                            if (ReadDataBuffer[0] != 0xA5)
                            {
                                this.dataReceived = false;
                                for (int i = 0; i < bytesRead; i++)
                                {
                                    if (ReadDataBuffer[i] == 0xA5)
                                    {
                                        ReadOffset = i;
                                        this.completed = true;
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                ReadOffset = 0;
                                this.completed = true;
                                return;
                            }
                        }
                        else
                        {
                            ReadDataBuffer = null;
                            this.completed = false;
                            return;
                        }
                        Tries--;
                    }

                }
                catch
                {
                    this.completed = true;
                    return;
                }
            }
            ReadDataBuffer = null;
            this.completed = false;
            return;
        }


        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Read data to the ReadDataBuffer
        /// </summary>
        /// 

        public void Read(byte[] buffer, int offset, int count)
        {
            if (ReadDataBuffer != null)
            {
                if ((offset + count) > ReadDataBuffer.Length)
                {
                    return;
                }
                else
                {
                    if (buffer.Length < count)
                    {
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            buffer[i] = ReadDataBuffer[offset + i];
                        }
                        return;
                    }
                }
            }
            else
                return;
        }

        public enum ReadMode
        {
            IrMode,
            ModbusMode,
            SimulatorMode
        }

        /// <summary>
        /// Read data to the open COM port
        /// </summary>
        /// 
        public byte[] ReadDataBuffer;
        public int ReadOffset;
        public async Task<bool> ReadAsync(ReadMode Mode)
        {
            Task<UInt32> loadAsyncTask;
            UInt32 bytesRead = 0;
            double Timeout;
            try
            {
                uint ReadBufferLength;
                switch (Mode)
                {
                    case ReadMode.IrMode:
                        ReadBufferLength = 524;
                        Timeout = 1;
                        break;
                    case ReadMode.ModbusMode:
                        ReadBufferLength = 256;
                        Timeout = 10;
                        break;
                    case ReadMode.SimulatorMode:
                        ReadBufferLength = 38;
                        Timeout = 10;
                        break;
                    default:
                        ReadBufferLength = 524;
                        Timeout = 1;
                        break;
                }
                                          
                var timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(Timeout));

                loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(timeoutSource.Token);
                bytesRead = await loadAsyncTask;

                UnreadedBytes = dataReaderObject.UnconsumedBufferLength;
              
                if (bytesRead > 0)
                {
                    ReadDataBuffer = new byte[bytesRead];

                    dataReaderObject.ReadBytes(ReadDataBuffer);

                    byte FirstChar;
                    switch(Mode)
                    {
                        case ReadMode.IrMode:
                            FirstChar = 0xA5;
                            break;
                        case ReadMode.ModbusMode:
                            FirstChar = Properties.Settings.Instance.Address;
                            break;
                        case ReadMode.SimulatorMode:
                            FirstChar = 1;
                            break;
                        default:
                            FirstChar = 0xA5;
                            break;
                    }

                    if (ReadDataBuffer[0] != FirstChar)
                    {
                        this.dataReceived = false;
                        for (int i = 0; i < bytesRead; i++)
                        {
                            if (ReadDataBuffer[i] == FirstChar)
                            {
                                ReadOffset = i;
                                this.completed = true;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        ReadOffset = 0;
                        this.completed = true;
                        return true;
                    }
                }
                else
                {
                    ReadDataBuffer = null;
                    this.completed = false;
                    return false;
                }
            }
            catch (TaskCanceledException)
            {
                ReadDataBuffer = null;
                this.completed = false;
                return false;
            }
            return false;
        }


        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The serial device used to interface to the COM port
        /// </summary>
        private SerialDevice serialDevice;

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The data reader used to read data from the COM port
        /// </summary>
        private DataReader dataReaderObject;

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The data writer used to send data to the COM port
        /// </summary>
        private DataWriter dataWriterObject;

    }
}
