using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static MC_Suite.Services.IrCOMPortManager;
using Windows.UI.Popups;
using MC_Suite.Services;
using MC_Suite.Properties;
using MC_Suite.Euromag.Protocols;
using static MC_Suite.Euromag.Protocols.StdCommands.FirmwareDownloadPayload;
using Windows.ApplicationModel.VoiceCommands;
using Euromag.Utility.CRC;
using Windows.UI.Xaml.Controls;

namespace MC_Suite.Services
{
    public class SlaveCOMPortManager : INotifyPropertyChanged
    {
        private static SlaveCOMPortManager _instance;
        public static SlaveCOMPortManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SlaveCOMPortManager();
                return _instance;
            }
        }

        public async void Open(Settings.COMPortItem Port)
        {
            if ((Port == null) || (Port.ID == String.Empty))
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "COM Port not selected",
                    Content = "Please select COM port: OPTIONS->Settings->Connection",
                    CloseButtonText = "OK",
                };
                await dialog.ShowAsync();
            }
            else
            {
                portHandler = new commPortHandler(Port.ID, 19200, 0, 8, 0, TimeSpan.FromMilliseconds(500));
                if (await portHandler.open())
                {

                }
                else
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "COM Port Error",
                        Content = "Error Opening " + Port.Name + " Port",
                        CloseButtonText = "OK",
                    };
                    await dialog.ShowAsync();
                }
            }
        }

        public class SlaveCmd
        {
            public byte key { get; set; }
            public short value { get; set; }
        }

        public async Task<SlaveCmd> ReceiveCommand()
        {
            if (await portHandler.receiveData(SerialPort.ReadMode.SlaveMode))
            {
                try
                {
                    byte[] Command = portHandler.GetReadBuffer();

                    SlaveCmd slaveCmd = new SlaveCmd();

                    slaveCmd.key = Command[0];
                    slaveCmd.value = BitConverter.ToInt16(Command, 10);

                    return slaveCmd;
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
            return null;
        }

        private CRCengine crc16Engine = new CRCengine(CRCengine.CRCCode.CRC_CCITT);
        public void SendResponse(SlaveCmd value)
        {
            UInt16 crc16;
            List<Byte> Response = new List<Byte>();
            List<Byte> Frame = new List<Byte>();
            Response.Add(value.key);
            Response.Add(0x0A);
            Response.Add(0xE9);
            Response.Add(0x01);
            Response.Add(0x02);
            Response.Add(0x00);
            Response.Add(0x00);
            Response.Add(0x00);
            Response.Add(0x00);
            Response.Add(0x00);
            Byte[] Data = BitConverter.GetBytes(value.value);
            Response.Add(Data[0]);
            Response.Add(Data[1]);
            
            crc16 = (UInt16) crc16Engine.crctable(Response.ToArray());
            Byte[] Crc = BitConverter.GetBytes(crc16);

            Frame.AddRange(Response);
            Frame.AddRange(Crc);

            SendData(Frame);
        }

        public async void SendData(List<byte> Data)
        {
            portHandler.sendData(Data);            
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

        #region ObservableObject

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
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
