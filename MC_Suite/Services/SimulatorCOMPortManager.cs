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
using Windows.UI.Xaml.Controls;


namespace MC_Suite.Services
{
    public class SimulatorCOMPortManager
    {
        private static SimulatorCOMPortManager _instance;
        public static SimulatorCOMPortManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SimulatorCOMPortManager();
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

        #region Protocol

        private static int FRAME_BASE = 6;
        private int DataLenght;

        static public byte[] DataRX;
        static public byte[] DataRX_noCRC;
        static public byte[] DataTX;

        public enum CMD
        {
            Ping = 1,
            DryTest = 2,
            WetTest = 3,
            ReadResults = 4
        }
        public enum ANSWER
        {
            StandBy = 0,
            Test_In_progress = 1,
            Test_Completed = 2,
            Test_Error = 3
        }
        public enum SimCommandResult
        {
            Failed = 0,
            Success = 1
        }

        //metodo
        public static bool DataRX_buf(byte[] n_byte_rx)
        {
            DataRX = new byte[n_byte_rx.Length];
            DataRX_noCRC = new byte[n_byte_rx.Length - 2];
            Array.Copy(n_byte_rx, DataRX, DataRX.Length);
            Array.Copy(n_byte_rx, DataRX_noCRC, DataRX.Length - 2);
            byte[] CRC_result = new byte[2];
            GetCRC(DataRX_noCRC, ref CRC_result);
            if (DataRX[n_byte_rx.Length - 2] == CRC_result[1] && DataRX[n_byte_rx.Length - 1] == CRC_result[0])
                return true;
            else
                return false;
        }

        // Propietà
        public static byte ID_rx
        {
            get
            {
                return DataRX[0];   //ID nella posizione 0
            }
        }
        public static byte CMD_rx
        {
            get
            {
                return DataRX[1];   //CMD nella posizione 1
            }
        }
        /// <summary>
        /// Crea Array di trasmissione
        /// </summary>
        /// <param name="ID">ID device</param>
        /// <returns></returns>
        static public byte[] CMD_Ping(byte ID)
        {
            byte[] TX_ping = { ID, (byte)CMD.Ping, 0, 0 };
            byte[] CRC_result = new byte[2];

            GetCRC(TX_ping, ref CRC_result);
            byte[] TX_pingCRC = { ID, (byte)CMD.Ping, 0, 0, CRC_result[1], CRC_result[0] };
            return TX_pingCRC;
        }
        static public byte[] CMD_Start_DryTest(byte ID)
        {
            byte[] DryTest = { ID, (byte)CMD.DryTest, 0, 0 };
            byte[] CRC_result = new byte[2];

            GetCRC(DryTest, ref CRC_result);
            byte[] DryTestCRC = { ID, (byte)CMD.DryTest, 0, 0, CRC_result[1], CRC_result[0] };
            return DryTestCRC;
        }
        static public byte[] CMD_Start_WetTest(byte ID)
        {
            byte[] WetTest = { ID, (byte)CMD.WetTest, 0, 1, 100, 101 };
            // calcolo crc16
            return WetTest;
        }
        static public byte[] CMD_Get_Results(byte ID)
        {
            byte[] TestResult = { ID, (byte)CMD.ReadResults, 0, 0 };
            byte[] CRC_result = new byte[2];
            GetCRC(TestResult, ref CRC_result);
            byte[] TestResultCRC = { ID, (byte)CMD.ReadResults, 0, 0, CRC_result[1], CRC_result[0] };
            return TestResultCRC;
        }

        static public void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < message.Length; i++)
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

        #region Open/Close Connection

        public async Task<bool> Open()
        {
            if ((Settings.Instance.SimulatorComPort == null) || (Settings.Instance.SimulatorComPort.ID == String.Empty))
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "COM Port not selected",
                    Content = "Please select COM port: OPTIONS->Settings->Connection",
                    CloseButtonText = "OK",
                };
                await dialog.ShowAsync();
                return false;
            }
            else if ((portHandler != null) && (portHandler.IsOpen))
            {
                return true;
            }
            else
            {
                portHandler = new commPortHandler(Settings.Instance.SimulatorComPort.ID, 57600, 0, 8, 0, TimeSpan.FromMilliseconds(500));
                try
                {
                    if (await portHandler.open())
                    {
                        return true;
                    }
                    else
                    {
                        ContentDialog dialog = new ContentDialog()
                        {
                            Title = "COM Port Error",
                            Content = "Error Opening " + Settings.Instance.SimulatorComPort.Name + " Port",
                            CloseButtonText = "OK",
                        };
                        await dialog.ShowAsync();
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "COM Port Error",
                        Content = "Error Opening " + Settings.Instance.SimulatorComPort.Name + " Port\n\r" + ex.Message,
                        CloseButtonText = "OK",
                    };
                    await dialog.ShowAsync();
                    return false;
                }
            }
        }

        public bool Close()
        {
            if (this.IsOpen)
                portHandler.close();

            if(portHandler != null)
                portHandler = null;

            return true;
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


        private bool _isReady;
        public bool IsReady
        {
            get { return _isReady; }
            set
            {
                if (value != _isReady)
                {
                    _isReady = value;
                }
            }
        }

        #endregion

        #region Send Command

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

        public async void SendCommand(commPortHandler ComPort, CMD Command, uint Tries )
        {
            try
            {
                CommandRunning = true;

                while (CommandRunning)
                {
                    byte[] message;

                    switch (Command)
                    {
                        case CMD.DryTest:
                            message = CMD_Start_DryTest(1);
                            DataLenght = 1;
                            break;
                        case CMD.Ping:
                            message = CMD_Ping(1);
                            DataLenght = 1;
                            break;
                        case CMD.ReadResults:
                            message = CMD_Get_Results(1);
                            DataLenght = 32;
                            break;
                        case CMD.WetTest:
                            message = CMD_Start_WetTest(1);
                            DataLenght = 1;
                            break;
                        default:
                            message = CMD_Ping(1);
                            DataLenght = 1;
                            break;
                    }

                    byte[] response = new byte[FRAME_BASE + DataLenght];

                    uint tries = Tries;

                    while (tries != 0)
                    {
                        ComPort.sendData(message.ToList());

                        if (await ComPort.receiveData(Services.SerialPort.ReadMode.SimulatorMode))
                        {
                            try
                            {
                                response = ComPort.decodeData(response.Length).ToArray();

                                bool esito = DataRX_buf(response);
                                if (esito)
                                {
                                    ANSWER msg_answer;
                                    string msg_text;

                                    if(response.Length <= 7)
                                    { 
                                        switch (response[4])
                                        {
                                            case 0:
                                                msg_answer = ANSWER.StandBy;
                                                msg_text = "Simulator Ready";
                                                break;
                                            case 1:
                                                msg_answer = ANSWER.Test_In_progress;
                                                msg_text = "Test Running";
                                                break;
                                            case 2:
                                                msg_answer = ANSWER.Test_Error;
                                                msg_text = "Test Error";
                                                break;
                                            default:
                                                msg_answer = ANSWER.Test_Error;
                                                msg_text = "Unknown State";
                                                break;
                                        }
                                        CommandResult = new SimulatorCommandResult() { Result = SimCommandResult.Success, Answer = msg_answer, Message = msg_text };
                                    }
                                    else
                                    {                                        
                                        msg_answer = ANSWER.Test_Completed;
                                        msg_text = "Test Completed";
                                        SimulatorCommandResult Tmp = new SimulatorCommandResult() { Result = SimCommandResult.Success, Answer = msg_answer, Message = msg_text };
                                        Tmp.DataResults = new byte[32];
                                        for (int i=0; i<32; i++)
                                        {
                                            Tmp.DataResults[i] = response[4 + i];
                                        }                                       
                                        CommandResult = Tmp;
                                    }                                    
                                    CommandRunning = false;
                                    return;
                                }
                            }
                            catch (Exception exc)
                            {
                                if (--tries == 0)
                                {
                                    CommandResult = new SimulatorCommandResult() { Result = SimCommandResult.Failed, Answer = ANSWER.StandBy, Message = exc.ToString() };
                                    CommandRunning = false;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (--tries == 0)
                            {
                                CommandResult = new SimulatorCommandResult() { Result = SimCommandResult.Failed, Answer = ANSWER.StandBy, Message = "No Response From Simulator" };
                                CommandRunning = false;
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                CommandRunning = false;
                CommandResult = new SimulatorCommandResult() { Result = SimCommandResult.Failed, Answer = ANSWER.StandBy, Message = exc.ToString() };
                return;
            }
        }

        #endregion

        #region Command Result

        public class SimulatorCommandResult
        {
            public SimCommandResult Result;
            public ANSWER Answer;
            public string Message;
            public byte[] DataResults;
         }

        private SimulatorCommandResult _commandResult;
        public SimulatorCommandResult CommandResult
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

        public event PropertyChangedEventHandler CommandCompleted;

        private void OnPropertyChanged(string name)
        {
            CommandCompleted?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }
}


