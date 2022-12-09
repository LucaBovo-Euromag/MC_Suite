using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Euromag.Utility.Endianness;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;

    using System;
    using System.Collections.Generic;

    public class FilesInfo
    {
        public class FilesInfo_Type
        {
            public uint index;
            public string name;
            public byte exsist;
            public uint size;
            public string last_access_date;
        }

        public FilesInfo_Type RecievedFileData = new FilesInfo_Type();

        public void Decode(List<byte> Data)
        {
            int loc_index; 
            int loc_seconds, loc_minutes, loc_hours, loc_dow, loc_day, loc_month, loc_year;
            DateTime loc_datetime;

            loc_index = 0;

            RecievedFileData.index = Data[loc_index];
            loc_index += 1;

            RecievedFileData.name = "";
            for (int i = 0; i < 12; i++)
            {
                RecievedFileData.name = RecievedFileData.name + (char)Data[loc_index + i];
            }
            loc_index += 12;

            RecievedFileData.exsist = Data[loc_index];
            loc_index += 1;

            RecievedFileData.size = (uint)Data[loc_index] * 0x1000000;
            loc_index += 1;

            RecievedFileData.size = RecievedFileData.size + (uint)Data[loc_index] * 0x10000;
            loc_index += 1;

            RecievedFileData.size = RecievedFileData.size + (uint)Data[loc_index] * 0x100;
            loc_index += 1;

            RecievedFileData.size = RecievedFileData.size + Data[loc_index];
            loc_index += 1;

            loc_seconds = Data[loc_index];
            loc_index += 1;

            loc_minutes = Data[loc_index];
            loc_index += 1;

            loc_hours = Data[loc_index];
            loc_index += 1;

            loc_dow = Data[loc_index];
            loc_index += 1;

            loc_day = Data[loc_index];
            loc_index += 1;

            loc_month = Data[loc_index];
            loc_index += 1;

            loc_year = Data[loc_index] * 0x100;
            loc_index += 1;

            loc_year = loc_year + Data[loc_index];

            try
            {
                loc_datetime = new DateTime(loc_year, loc_month, loc_day, loc_hours, loc_minutes, loc_seconds);
                RecievedFileData.last_access_date = loc_datetime.ToString("MM/dd/yyyy HH:mm.ss");
            }
            catch
            {
                RecievedFileData.last_access_date = DateTime.Now.ToString("MM/dd/yyyy HH:mm.ss");
            }            
        }
    }

    public class GetFileData : StdCommand
    {
        public GetFileData()
        {

        }

        public GetFileData(commPortHandler handler)
            : base(handler)
        {

        }

        public GetFileData(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Get uSD Files Info";
        }

        public uint FileIndex
        {
            get
            {
                return FIndex;
            }

            set
            {
                FIndex = value;
            }
        }

        public FilesInfo File_Info
        {
            get
            {
                if (_file_info == null)
                    _file_info = new FilesInfo();
                return _file_info;
            }
            set
            {
                if (_file_info != value)
                    _file_info = value;
            }
        }

        protected override void reset()
        {
            completed = false;
        }

        protected override ICommunicationFrame CommandFrame()
        {
            if (completed)
                return null;

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.Address   = FIndex;
            completed      = true;

            return new StdCommunicationFrame(head, null);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            File_Info.Decode(payload.ToList());

            return new CommandResult();
        }

        private const Byte commandFrameType = 0xF6;
        private const Byte answerFrameType = 0xF5;
        private bool completed;
        private uint FIndex;

        private FilesInfo _file_info;
    }
}
