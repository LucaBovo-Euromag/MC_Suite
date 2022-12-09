using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using MC_Suite.Services;
using MC_Suite.Properties;
using Windows.UI.Popups;
using Windows.Devices.SerialCommunication;
using Windows.UI.Xaml;


namespace MC_Suite.Modbus
{
    public class Protocol
    {
        //public SerialPort sp = new SerialPort();
        public string modbusStatus;
        public DispatcherTimer TimeoutTimer { get; set; }

        private static Protocol _instance;
        public static Protocol Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Protocol();
                return _instance;
            }
        }

        public static ushort[,] Registers = new ushort[17, 1792]; // registri cmd, value reg

        public void Clear()
        {
            Array.Clear(Registers, 0, Registers.Length);
        }

        #region Exceptions_Codes

        public enum ModBus_Exception_Codes
        {
            ILLEGAL_FUNCTION = 1,       //The function code received in the query is not an allowable action for the server (or slave). This
                                        //may be because the function code is only applicable to newer devices, and was not
                                        //implemented in the unit selected. It could also indicate that the server (or slave) is in the wrong
                                        //state to process a request of this type, for example because it is unconfigured and is being
                                        //asked to return register values.

            ILLEGAL_DATA_ADDRESS = 2,   //The data address received in the query is not an allowable address for the server (or slave). More
                                        //specifically, the combination of reference number and transfer length is invalid. For a controller with
                                        //100 registers, the PDU addresses the first register as 0, and the last one as 99. If a request
                                        //is submitted with a starting register address of 96 and a quantity of registers of 4, then this request
                                        //will successfully operate (address-wise at least) on registers 96, 97, 98, 99. If a request is
                                        //submitted with a starting register address of 96 and a quantity of registers of 5, then this request
                                        //will fail with Exception Code 0x02 “Illegal Data Address” since it attempts to operate on registers
                                        //96, 97, 98, 99 and 100, and there is no register with address 100.

            ILLEGAL_DATA_VALUE = 3,     //A value contained in the query data field is not an allowable value for server (or slave). This
                                        //indicates a fault in the structure of the remainder of a complex request, such as that the implied
                                        //length is incorrect. It specifically does NOT mean that a data item submitted for storage in a register
                                        //has a value outside the expectation of the application program, since the MODBUS protocol
                                        //is unaware of the significance of any particular value of any particular register.

            SLAVE_DEVICE_FAILURE = 4,   //An unrecoverable error occurred while the server (or slave) was attempting to perform the requested action.

            ACKNOWLEDGE = 5,            //Specialized use in conjunction with programming commands.
                                        //The server (or slave) has accepted the request and is processing it, but a long duration of time
                                        //will be required to do so. This response is returned to prevent a timeout error from occurring
                                        //in the client (or master). The client (or master) can next issue a Poll Program Complete message
                                        //to determine if processing is completed.

            SLAVE_DEVICE_BUSY = 6,      //Specialized use in conjunction with programming commands.
                                        //The server (or slave) is engaged in processing a long–duration program command. The client (or
                                        //master) should retransmit the message later when the server (or slave) is free.

            MEMORY_PARITY_ERROR = 8,    //Specialized use in conjunction with function codes 20 and 21 and reference type 6, to indicate that
                                        //the extended file area failed to pass a consistency check.
                                        //The server (or slave) attempted to read record file, but detected a parity error in the memory.
                                        //The client (or master) can retry the request, but service may be required on the server (or slave) device.

            GATEWAY_PATH_UNAVAILABLE = 0x0A,// Specialized use in conjunction with gateways,indicates that the gateway was unable to allocate
                                            //an internal communication path from the input
                                            //port to the output port for processing the request.
                                            //Usually means that the gateway is misconfigured
                                            //or overloaded.

            GATEWAY_TARGET_DEVICE = 0x0B   //FAILED TO RESPOND
                                           //Specialized use in conjunction with gateways,
                                           //indicates that no response was obtained from the
                                           //target device. Usually means that the device is
                                           //not present on the network.

        }

        #endregion

        #region CRC Computation
        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
        #endregion

        #region Build Message

        /// <summary>
        /// Prepara il Buffer del messaggio da spedire
        /// </summary>
        /// <param name="address">Indirizzo Slave</param>
        /// <param name="cmd">Codice Comando</param>
        /// <param name="start">Registro di partenza</param>
        /// <param name="n_registers">Numero registri</param>
        /// <param name="message">Buffer con il messaggio restituito</param>
        private void BuildMessage(byte address, byte cmd, ushort start, ushort n_registers, ref byte[] message)
        {
            //Array to receive CRC bytes:
            byte[] CRC = new byte[2];

            message[0] = address;
            message[1] = cmd;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)start;
            message[4] = (byte)(n_registers >> 8);
            message[5] = (byte)n_registers;
            
            GetCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
        }
        #endregion

        #region Check Response
        public bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check:
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }
        #endregion

        #region Get Response
        private bool GetResponse(ref byte[] response)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.

            //response = MBconnection.portHandler.decodeData(response.Length).ToArray();

            for (int i = 0; i < response.Length; i++)
            {
                if ((i >= 4) && (response[1] >= 0x80)) // se ho un messaggio di errore come risposta 0x80+cmd, error code
                {
                    Array.Resize<byte>(ref response, 5);
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Function 16 - Write Multiple Registers

        public async void ModbusCOMMAND_16(commPortHandler ComPort, byte Address, ModbusRegister First, ModbusRegister Last, uint Tries)
        {
            try
            {
                CommandRunning = true;

                while (CommandRunning)
                {
                    ushort N_Registri = Last.address_end;
                    N_Registri -= First.address;
                    N_Registri += 1;

                    //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                    byte[] message = new byte[9 + 2 * N_Registri];
                    //Function 16 response is fixed at 8 bytes
                    byte[] response = new byte[8];

                    for (int i = 0; i < N_Registri; i++)
                    {
                        message[7 + 2 * i] = (byte)(Registers[First.cmd, First.address + i] >> 8);
                        message[8 + 2 * i] = (byte)(Registers[First.cmd, First.address + i]);
                    }

                    message[6] = (byte)(N_Registri * 2);

                    BuildMessage(Address, Map.CMD16, First.address, N_Registri, ref message);

                    uint tries = Tries;

                    while (tries != 0)
                    {
                        ComPort.sendData(message.ToList());

                        if (await ComPort.receiveData(Services.SerialPort.ReadMode.ModbusMode))
                        {
                            try
                            {
                                response = ComPort.decodeData(response.Length).ToArray();

                                if (GetResponse(ref response))
                                {
                                    if (CheckResponse(response))
                                    {
                                        CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.success, Message = "Write Success" };
                                        CommandRunning = false;
                                        Map.error_mdb = false;
                                        break;
                                    }
                                    else
                                    {
                                        CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "CRC error" };
                                        CommandRunning = false;
                                        Map.error_mdb = true;
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                if (--tries == 0)
                                {
                                    CommandRunning = false;
                                    CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "Write COM Error" };
                                    Map.error_mdb = true;
                                }
                            }
                        }
                        else
                        {
                            if (--tries == 0)
                            {
                                CommandRunning = false;
                                CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "No Response From Slave" };
                                Map.error_mdb = true;
                            }
                        }
                    }
                }
            }
            catch
            {
                CommandRunning = false;
                Map.error_mdb = true;
                CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "Write COM Error" };
            }
        }

        public async void ModbusCOMMAND_16(commPortHandler ComPort, byte Address, ushort start, ushort registers, ushort[]values, uint Tries)
        {
            try
            {
                CommandRunning = true;

                while (CommandRunning)
                {
                    //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                    byte[] message = new byte[9 + 2 * registers];
                    //Function 16 response is fixed at 8 bytes
                    byte[] response = new byte[8];

                    //Add bytecount to message:
                    message[6] = (byte)(registers * 2);
                    //Put write values into message prior to sending:
                    for (int i = 0; i < registers; i++)
                    {
                        message[7 + 2 * i] = (byte)(values[i] >> 8);
                        message[8 + 2 * i] = (byte)(values[i]);
                    }

                    BuildMessage(Address, Map.CMD16, start, registers, ref message);

                    uint tries = Tries;

                    while (tries != 0)
                    {
                        ComPort.sendData(message.ToList());

                        if (await ComPort.receiveData(Services.SerialPort.ReadMode.ModbusMode))
                        {
                            try
                            {
                                response = ComPort.decodeData(response.Length).ToArray();

                                if (GetResponse(ref response))
                                {
                                    if (CheckResponse(response))
                                    {
                                        CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.success, Message = "Write Success" };
                                        CommandRunning = false;
                                        Map.error_mdb = false;
                                        break;
                                    }
                                    else
                                    {
                                        CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "CRC error" };
                                        CommandRunning = false;
                                        Map.error_mdb = true;
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                if (--tries == 0)
                                {
                                    CommandRunning = false;
                                    CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "Write COM Error" };
                                    Map.error_mdb = true;
                                }
                            }
                        }
                        else
                        {
                            if (--tries == 0)
                            {
                                CommandRunning = false;
                                CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "No Response From Slave" };
                                Map.error_mdb = true;
                            }
                        }
                    }
                }
            }
            catch
            {
                CommandRunning = false;
                Map.error_mdb = true;
                CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "Write COM error"  };
            }
        }

        #endregion       

        #region Funcion 3-4 Read Registers/Inputs

        /// <summary>
        /// Lettura Registri/Inputs (Comandi 3 e 4)
        /// </summary>
        public async void ModbusCOMMAND_3_4( commPortHandler ComPort, byte Address, ModbusRegister First, ModbusRegister Last, uint Tries)
        { 
            try
            { 
                CommandRunning = true;

                while(CommandRunning)
                {
                    ushort N_Registri = Last.address_end;
                    N_Registri -= First.address;
                    N_Registri += 1;

                    byte[] message = new byte[8];
                    byte[] response = new byte[5 + N_Registri*2];

                    BuildMessage(Address, First.cmd, First.address, N_Registri, ref message);

                    uint tries = Tries;

                    while (tries != 0)
                    {
                        ComPort.sendData(message.ToList());
                           
                        if (await ComPort.receiveData( Services.SerialPort.ReadMode.ModbusMode ))
                        {
                            try
                            {
                                response = ComPort.decodeData(response.Length).ToArray();

                                if( GetResponse(ref response) )
                                {
                                    if (CheckResponse(response))
                                    {
                                        for (int i = 0; i < (response.Length - 5) / 2; i++)
                                        {
                                            Registers[First.cmd, First.address + i] = response[2 * i + 3];
                                            Registers[First.cmd, First.address + i] <<= 8;
                                            Registers[First.cmd, First.address + i] += response[2 * i + 4];
                                        }
                                        CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.success, Message = "Read Success"  };
                                        CommandRunning = false;
                                        Map.error_mdb = false;
                                        break;
                                    }
                                    else
                                    {
                                        CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "CRC error" };
                                        CommandRunning = false;
                                        Map.error_mdb = true;
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                if(--tries == 0)
                                {
                                    CommandRunning = false;
                                    CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "Read COM Error" };
                                    Map.error_mdb = true;
                                }
                            }
                        }
                        else
                        { 
                            if(--tries == 0)
                            {
                                CommandRunning = false;
                                CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "No Response From Slave" };
                                Map.error_mdb = true;
                            }
                        }
                    }
                }

            }
            catch
            {
                CommandRunning = false;
                Map.error_mdb = true;
                CommandResult = new ModbusCommandResult() { Result = ModbusTransferResult.failed, Message = "Read COM error" };
            }
        }

        #endregion

        #region CommandCompleted

        public enum ModbusTransferResult
        {
            success,
            failed
        }

        public class ModbusCommandResult
        {
            public ModbusTransferResult Result;
            public string Message;
        }

        private ModbusCommandResult _commandResult;
        public ModbusCommandResult CommandResult
        {
            get { return _commandResult; }
            set
            {
                if (value != _commandResult)
                {
                    _commandResult = value;
                    OnPropertyChanged("CommandResult");
                }
            }
        }

        private bool _commandRunning;
        public bool CommandRunning
        {
            get { return _commandRunning; }
            set
            {
                if (value != _commandRunning)
                {
                    _commandRunning = value;
                }
            }
        }        

        public event PropertyChangedEventHandler CommandCompleted;

        private void OnPropertyChanged(string name)
        {
            CommandCompleted?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Function 20 - Lettura flash DataLogger ** fuori standard modbus **

        /// <summary>
        /// Lettura DataLogger MC608_6
        /// </summary>
        /// <param name="address">Indirizzo del dispositivo</param>
        /// <param name="start_row_x10">Riga di partenza multiplo di 10</param>
        /// <returns>True se esito trasmissione OK</returns>
        public bool SendFc20(byte address, ushort start_row_x10)
        {
            /*if (!MBconnection.IsOpen)
            {
                try
                {
                    await MBconnection.Open();
                    byte[] message = new byte[8];
                    BuildMessage(address, (byte)20, start_row_x10, (ushort)(start_row_x10 + 10), ref message); // message contiene la sequenza di byte
                    
                    List<byte> MessageList = message.OfType<byte>().ToList();
                    MBconnection.portHandler.sendData(MessageList);
                    MBconnection.Close();
                    return true;

                }
                catch (Exception err)
                {
                    modbusStatus = "Read error: " + err.Message;
                    MBconnection.Close();
                    return false;
                }
 
            }
            else
            {
                modbusStatus = "Serial port is Buisy";
                return false;
            }*/
            return false;
        }
        /// <summary>
        /// INVIO comando 20 lettura flash log, pagine da 528byte
        /// </summary>
        /// <param name="address">Indirizzo del dispositivo</param>
        /// <param name="start">Pagina di partenza</param>
        /// <param name="n_page">numero di pagine da leggere</param>
        /// <param name="values">out lettura</param>
        /// <returns>valori short</returns>
        public bool SendFc20(byte address, ushort start, ushort n_page)
        {
            //Ensure port is open:
            /*if (MBconnection.IsOpen)
            {
                try
                {
                    //sp.DiscardOutBuffer();
                    //sp.DiscardInBuffer();
                    byte[] message = new byte[8];
                    BuildMessage(address, (byte)20, start, n_page, ref message); // message contiene la sequenza di byte
                    List<byte> MessageList = message.OfType<byte>().ToList();
                    MBconnection.portHandler.sendData(MessageList);
                    return true;

                }
                catch (Exception err)
                {
                    modbusStatus = "Read error: " + err.Message;
                    return false;
                }
 
            }
            else
            {
                modbusStatus = "Serial port not open";
                return false;
            }*/
            return false;
        }

        #endregion
    }
}
