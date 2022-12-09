namespace MC_Suite.Services
{
    using System;
    using System.Collections.Generic;
    using Windows.Devices.SerialCommunication;
    using System.Threading.Tasks;
    using System.Linq;

    public class commPortHandler
    {
        /// Create a new port handler bound to to given port.       
        public commPortHandler(string portID, uint baudRate, byte parity, ushort dataBits, byte stopBits, TimeSpan timeOut)
        {
            SerialParity _parity;
            switch(parity)
            {
                case (0):
                    _parity = SerialParity.None;
                    break;
                case (1):
                    _parity = SerialParity.Odd;
                    break;
                case (2):
                    _parity = SerialParity.Even;
                    break;
                case (3):
                    _parity = SerialParity.Mark;
                    break;
                case (4):
                    _parity = SerialParity.Space;
                    break;
                default:
                    _parity = SerialParity.None;
                    break;
            }

            SerialStopBitCount _stopBits;
            switch(stopBits)
            {
                case (0):
                    _stopBits = SerialStopBitCount.One;
                    break;
                case (1):
                    _stopBits = SerialStopBitCount.OnePointFive;
                    break;
                case (2):
                    _stopBits = SerialStopBitCount.Two;
                    break;
                default:
                    _stopBits = SerialStopBitCount.One;
                    break;
            }

            _port = new Services.SerialPort(portID, baudRate, _parity, dataBits, _stopBits, timeOut);
            _port.ReadTimeout = timeOut;

            sendedBytes = new List<byte>();
        }
       
        private List<byte> sendedBytes;

        public Services.SerialPort SerialPortHandler
        {
            get 
            {
                return _port;
            }
        }

        public string PortID
        {
            get { return _port.PortID;  }
        }

        /// <summary>
        /// Opens serial port
        /// </summary>
        public async Task<bool> open()
        {
            await _port.Open();
            return _port.IsOpen;
        }

        /// <summary>
        /// Closes serial port
        /// </summary>
        public void close()
        {
            _port.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return _port.IsOpen;
            }
        }

        public bool TransfertCompleted
        {
            get
            {
                return _port.completed;
            }
        }

        /// <summary>
        /// Sets Or Gets timeot (ms) for reading operation on port.
        /// </summary>
        public TimeSpan ReadTimeout
        {
            get
            {
                return _port.ReadTimeout;
            }

            set
            {
                _port.ReadTimeout = value;
            }

        }

        /// <summary>
        /// Sends bytes on serial port
        /// </summary>
        /// <param name="outBuff">List of bytes to be sent out</param>
        public void sendData(List<Byte> outBuff)
        {
            _port.completed = false;
            sendedBytes.AddRange(outBuff);
            //_port.WriteReadAsync(outBuff.ToArray());
            _port.WriteAsync(outBuff.ToArray());
        }

        public byte[] ReadBuffer;
        private void GetReadBuffer(Int32 size)
        {
            _port.Read(ReadBuffer, 0, size);
        }



        /// <summary>
        /// Wait for data on serial port till all arrived or 
        /// @ReadTimeout expires
        /// </summary>
        /// <param name="answerSize">Size in bytes of the answer to wait for</param>
        /// <returns>A list of received byte, should be @answersize long</returns>
        /// <exception cref="TimeoutException"></exception>

        public List<Byte> decodeData(Int32 answerSize)
        {
            if (answerSize == 0)
                return null;

            byte[] buff = new byte[answerSize];

            _port.Read(buff, _port.ReadOffset, answerSize);
            _port.ReadOffset += answerSize;

            return new List<byte>(buff);
        }

        public async Task<bool> receiveData( SerialPort.ReadMode Mode)
        {
            return await _port.ReadAsync( Mode );
        }


        public List<Byte> receiveDataFromByteList(Int32 answerSize)
        {
            if (answerSize == 0)
                return null;

            byte[] buff = new byte[answerSize];

             _port.Read(buff, 0, answerSize);     

            return new List<byte>(buff);
        }


        // PRIVATE SECTION
        private Services.SerialPort _port;
    }
}
