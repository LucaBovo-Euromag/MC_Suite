using Euromag.Utility.Endianness;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class DataLogLineFields
    {
        public UInt32 RowNumber
        { get; set; }

        public DateTime Timestamp
        { get; set; }

        public DataLogLine.DataLogError Errors
        { get; set; }

        public int PCBtemperature
        { get; set; }

        public float BatteryVoltage
        { get; set; }

        public float Flow
        { get; set; }

        public float TotalPositive
        { get; set; }

        public float TotalNegative
        { get; set; }

        public int BatteryEnergy
        { get; set; }

        public float AuxFloat1
        { get; set; }

        public float AuxFloat2
        { get; set; }

        public DataLogLine.DataLogType LogType
        { get; set; }
    }

    public class DataLogLine : IFrameable, ILogRecord
    {
        public DataLogLine()
        {
        }

        public DataLogLine(List<Byte> buff)
        {
            Extract(buff);
        }

        /*public string Separator
        { get; set; }*/
        
        public Int32 Size
        {
            get
            {
                return SIZE;
            }
        }

        public UInt32 RowNumber
        { get; set; }

        public DateTime Timestamp
        { get; set; }

        public DataLogLine.DataLogError Errors
        { get; set; }

        public int PCBtemperature
        { get; set; }

        public float BatteryVoltage
        { get; set; }

        public float Flow
        { get; set; }

        public float TotalPositive
        { get; set; }

        public float TotalNegative
        { get; set; }

        public int BatteryEnergy
        { get; set; }

        public float AuxFloat1
        { get; set; }

        public float AuxFloat2
        { get; set; }

        public DataLogLine.DataLogType LogType
        { get; set; }

        public DataLogLineFields FieldsObj
        {
            get
            {
                DataLogLineFields fld = new DataLogLineFields();

                fld.Timestamp = _timestamp;
                fld.Flow = _flow;
                fld.TotalPositive = _totalPositive;
                fld.TotalNegative = _totalNegative;
                fld.AuxFloat1 = _auxFloat1;
                fld.AuxFloat2 = _auxFloat2;
                fld.LogType = _logType;
                fld.Errors = _errors;
                fld.BatteryEnergy = _batteryEnergy;
                fld.BatteryVoltage = ((float)_batteryVoltage) / 10;
                fld.PCBtemperature = _PCBtemperature;

                return fld;
            }
        }

        public List<Byte> ToList()
        {
            //TODO: fix this
            throw new NotImplementedException();
        }

        public CommandResult Extract(List<Byte> buff)
        {
            Byte[] inpFrame = new List<Byte>(buff.GetRange(0, SIZE)).ToArray();
            buff.RemoveRange(0, SIZE);

            _timestamp = LogDateTime.Decode(inpFrame);
            Timestamp = _timestamp;

            _flow = LEconverter.LEArrayToSingle(inpFrame, 5);
            Flow = _flow;

            _totalPositive =  LEconverter.LEArrayToSingle(inpFrame, 9);
            TotalPositive = _totalPositive;

            _totalNegative =  LEconverter.LEArrayToSingle(inpFrame, 13);
            TotalNegative = _totalNegative;

            _auxFloat1 = LEconverter.LEArrayToSingle(inpFrame, 17);
            AuxFloat1 = _auxFloat1;
            
            _auxFloat2 = LEconverter.LEArrayToSingle(inpFrame, 21);
            AuxFloat2 = _auxFloat2;
            
            _logType = (DataLogType)inpFrame[25];
            LogType = _logType;
            
            _errors = (DataLogError)inpFrame[26];
            Errors = _errors;
            
            _batteryEnergy = inpFrame[27];
            BatteryEnergy = _batteryEnergy;
            
            _batteryVoltage = ((float)inpFrame[28] / 10);
            BatteryVoltage = _batteryVoltage;
            
            _PCBtemperature = (int)(inpFrame[29]);
            PCBtemperature = _PCBtemperature;

            return new CommandResult();

        }

        public List<String> fieldNames
        {
            get
            {
                if (_fieldNames == null)
                {
                    _fieldNames = new List<String>();
                    _fieldNames.Add("Timestamp");
                    _fieldNames.Add("Errors");
                    _fieldNames.Add("Flow");
                    _fieldNames.Add("Total positive");
                    _fieldNames.Add("Total negative");
                    _fieldNames.Add("Log type");
                    _fieldNames.Add("Aux meas 1");
                    _fieldNames.Add("Aux meas 2");
                    _fieldNames.Add("Board Temperature");
                    _fieldNames.Add("Battery voltage [V]");
                    _fieldNames.Add("Battery Energy [%]");
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

                _fields.Add(_timestamp.ToShortDateString() + " " + _timestamp.ToShortTimeString());
                _fields.Add(_errors.ToString());
                _fields.Add(_flow.ToString());
                _fields.Add(_totalPositive.ToString());
                _fields.Add(_totalNegative.ToString());
                _fields.Add(_logType.ToString());
                _fields.Add(_auxFloat1.ToString());
                _fields.Add(_auxFloat2.ToString());
                _fields.Add(_PCBtemperature.ToString());
                _fields.Add(_batteryVoltage.ToString());
                _fields.Add(_batteryEnergy.ToString());
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

        [Flags]
        public  enum DataLogError
        {
            ExcFailure = 0x01,
            EmptyPipe = 0x02,
            FlowMax = 0x04,
            FlowMin = 0x08,
            PulsesOverlap = 0x10,
            ADCrange = 0x20,			//ADC absolute converted value is over the safety threshold (|ADC|>MaxAbsoluteValue)
            InputStage = 0x40,			//the input stage is not usable (over the working range)
            MeasElectrodeDry = 0x80	    //the measuring electrodes are not covered by water
        };

        public enum DataLogType
        {
            FlowOnly = 0,               //solo portata
            FlowAndPressure = 1,	    //portata e pressione
            FlowAndOneTemperature = 2,	//portata e 1 sonda di temperatura
            FlowAndOneTempAndPressure = 3,	//portata, 1 sonda di temperatura, pression
            FlowAndTwoTemperatures = 4,	//portata e 2 sonde di temperatura (contacalorie)
            FlowAndPressureAndDeltaTemp = 5	//portata, pressione e 2 sonde di temperatura (contacalorie)
        };

        private const Int32 SIZE = 32;

        private List<String> _fields;
        private static List<String> _fieldNames;
        private DateTime _timestamp;
        private DataLogError _errors;
        private int _PCBtemperature;
        private float _batteryVoltage;
        private float _flow;
        private float _totalPositive;
        private float _totalNegative;
        private int _batteryEnergy;
        private float _auxFloat1;
        private float _auxFloat2;
        private DataLogType _logType;
    }

    public class GetDataLogLines : GetLogLines<DataLogLine>
    {
        public GetDataLogLines()
            : base()
        {

        }

        public GetDataLogLines(String portName)
            : base(portName)
        {

        }

        public GetDataLogLines(commPortHandler handler)
            : base(handler)
        {

        }

        public override string ToString()
        {
            return "Get Data Log Lines Command";
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            switch ( head.FrameType )
            {
                case invalidRowFrameType:
                    return new CommandResult(CommandResultOutcomes.CommandFailed, "Log Lines unavailable");
                case answerErrorFrameType:
                    return new CommandResult(CommandResultOutcomes.CommandFailed, "Read error");
                case logLockedFrameType:
                    return new CommandResult(CommandResultOutcomes.CommandFailed, "Log is locked");
                case answerFrameType:
                    StartLine = head.Address;

                    if (!(payload is LogLinesPayload<DataLogLine>))
                        return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong answer frame");

                    logLines = (payload as LogLinesPayload<DataLogLine>).GetLines();

                    uint idx = StartLine;
                    foreach (var line in logLines)
                        line.RowNumber = idx++;
                    

                    return new CommandResult();
 
                default:
                    return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");
            }
        }

        protected override Byte commandFrameType
        { 
            get
            {
                return 0x40;
            }
        }
        private const Byte answerFrameType      = 0x40;
        private const Byte invalidRowFrameType  = 0x41;
        private const Byte logLockedFrameType   = 0x42;
        private const Byte answerErrorFrameType = 0x43;
    }

}
