using Euromag.Utility.Endianness;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BslLogLine : IFrameable, ILogRecord
    {
        public BslLogLine()
        {
            if (_stepNames == null)
            {
                _stepNames = new List<String>();
                _stepNames.Add("Work Fw Check");
                _stepNames.Add("Listening to post");
                _stepNames.Add("Firmware update received");
                _stepNames.Add("Erasing download memory");
                _stepNames.Add("Downloading Update");
                _stepNames.Add("Downloaded Fw Check");
                _stepNames.Add("Backup working copy");
                _stepNames.Add("Update working copy");
                _stepNames.Add("Backup new working copy");
                _stepNames.Add("Update Ok");
                _stepNames.Add("Repair working copy");
                _stepNames.Add("Restore working copy");
            }
            _fields = null;
        }

        public BslLogLine(List<Byte> buff)
            : this()
        {
            Extract(buff);
        }

        public Int32 Size { 
            get
            {
                return SIZE;
            }
        }

        public DateTime Date
        { get; set; }

        public byte ID
        { get; set; }

        public string Step
        { get; set; }

        public string Success
        { get; set; }

        public string FW_Ver
        { get; set; }

        public string CRC32
        { get; set; }

        public List<Byte> ToList()
        {
            //TODO: fix this
            throw new NotImplementedException();
        }

        public CommandResult Extract(List<Byte> buff)
        {
            List<Byte> inpFrame = new List<Byte>(buff.GetRange(0, SIZE));
            buff.RemoveRange(0, SIZE);

            _crc32 = LEconverter.LEArrayToUInt32(inpFrame.ToArray(), 0);
            CRC32 = _crc32.ToString("X8");

            _success = (inpFrame.ToArray()[12] == 1);
            if (_success)
                Success = "Ok";
            else
                Success = "Fail";

            _step = inpFrame.ToArray()[13];
            Step = _stepNames[_step];

            _fwVer = inpFrame.ToArray()[4];
            _fwRev = inpFrame.ToArray()[5];
            FW_Ver = _fwVer.ToString() + "." + _fwRev.ToString();

            _id = inpFrame.ToArray()[6];
            ID = _id;

            List<Byte> newList = new List<byte>(inpFrame.SkipWhile((value, index) => index < 7));

            dt = LogDateTime.Decode(newList.ToArray());
            Date = dt;

            return new CommandResult();

        }
        
        public List<String> fieldNames
        {
            get
            {
                if (_fieldNames == null)
                {
                    _fieldNames = new List<String>();
                    _fieldNames.Add("Date");
                    _fieldNames.Add("Time");
                    _fieldNames.Add("ID");
                    _fieldNames.Add("Step");
                    _fieldNames.Add("Success");
                    _fieldNames.Add("Fw Rev.");
                    _fieldNames.Add("crc32");
                }
                return _fieldNames;
            }
        }

        public List<String> fields
        {
            get
            {
                if (_fields == null)
                    _fields = new List<String>();
                else
                    _fields.Clear();

                _fields.Add(dt.ToShortDateString());
                _fields.Add(dt.ToShortTimeString());
                _fields.Add(_id.ToString());
                if (_step < _stepNames.Count)
                    _fields.Add(_stepNames[_step]);
                else
                    _fields.Add(_step.ToString());
                _fields.Add(_success.ToString());
                _fields.Add(_fwVer.ToString("X2") + "." + _fwRev.ToString("X2"));
                _fields.Add(_crc32.ToString("X8"));
                
                return _fields;
            }
        }

        public override string ToString()
        {
            String str = String.Empty;

            foreach (String field in fields)
                str += field + ";";

            return str;
        }

        private const Int32 SIZE = 32;
        private UInt32 _crc32;
        private DateTime dt;
        private Byte _fwRev;
        private Byte _fwVer;
        private Byte _step;
        private Byte _id;
        private Boolean _success;
        private List<String> _fields;
        private static List<String> _stepNames;
        private static List<String> _fieldNames;
    }

    public class GetBslLogLines : GetLogLines<BslLogLine>
    {
        public GetBslLogLines()
            : base()
        {

        }

        public GetBslLogLines(String portName)
            : base(portName)
        {

        }

        public GetBslLogLines(commPortHandler handler)
            : base(handler)
        {

        }

        public override string ToString()
        {
            return "Get Bsl Log Lines Command";
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType == answerErrorFrameType)
                return new CommandResult(CommandResultOutcomes.CommandFailed, "Log Lines unavailable");

            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            StartLine = head.Address;

            if (!(payload is LogLinesPayload<BslLogLine>))
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong answer frame");

            logLines = (payload as LogLinesPayload<BslLogLine>).GetLines();

            return new CommandResult();
        }

        protected override Byte commandFrameType
        {
            get
            {
                return 0x48;
            }
        }
        private const Byte answerFrameType = 0x48;
        private const Byte answerErrorFrameType = 0x49;

    }
}
