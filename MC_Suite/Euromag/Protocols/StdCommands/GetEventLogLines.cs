using Euromag.Utility.Endianness;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventLogLine : IFrameable, ILogRecord
    {
        public EventLogLine()
        {

        }

        public EventLogLine(List<Byte> buff)
        {
            Extract(buff);
        }

        public Int32 Size
        {
            get
            {
                return 0; //TODO: 
            }
        }

        public List<Byte> ToList()
        {
            //TODO: fix this
            throw new NotImplementedException();
        }

        public UInt32 Index
        {
            get;
            set;
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public EventType Event
        {
            get { return _type; }
        }

        public String AuxContent
        {
            get { return _auxContent; }
        }

        public CommandResult Extract(List<Byte> buff)
        {
            Byte[] inpFrame = buff.ToArray();
            buff.Clear();

            _timestamp = LogDateTime.Decode(inpFrame);
            _type = (EventType) (LEconverter.LEArrayToUInt16(inpFrame, 6));
            switch (_type)
            {
                default:
                case EventType.evtNone:
                case EventType.evtProcessLogErased:
                case EventType.evtUserWakeUp:
                case EventType.evtEmergencyOff:
                case EventType.evtSD24busy:
                case EventType.evtAccumulatorMemoryError:
                case EventType.evtAccumulatorMemoryFailure:
                case EventType.evtMeasureTOUT:
                case EventType.evtEPipeTOUT:
                case EventType.evtConvTempTOUT:
                case EventType.evtConvPressTOUT:
                    _auxContent = String.Empty;    
                break;
                case EventType.evtBoot:
                    resetCause reset = (resetCause)(LEconverter.LEArrayToUInt16(inpFrame, 8));
                    _auxContent = "Cause=" + reset.ToString();
                    break;
                case EventType.evtParameterChanged:
                    uint id = LEconverter.LEArrayToUInt16(inpFrame, 8);
                    ITargetWritable variable = AllEEPROMVarsLister.getList().SingleOrDefault(v => (v as IEEPROMvariable).ID == id);
                    if (variable != null)
                    {
                        _auxContent = "var=" + variable.ToString();
                        variable.Parse(inpFrame.ToList().GetRange(10, variable.Size));
                        _auxContent += ";oldVal=" + variable.ValAsString;
                        variable.Parse(inpFrame.ToList().GetRange(10 + variable.Size, variable.Size));
                        _auxContent += ";newVal=" + variable.ValAsString;
                    }
                    else
                        _auxContent = "var=" + id.ToString();
                    break;
                case EventType.evtBatteryExhausting:
                case EventType.evtBatteryFull:
                case EventType.evtBatteryReplaced:
                case EventType.evtBatteryLow:
                    _auxContent = "Voltage=" + (LEconverter.LEArrayToSingle(inpFrame, 8)).ToString() + "V;";
                    _auxContent += "Temperature=" + (LEconverter.LEArrayToUInt16(inpFrame, 12)).ToString() + "°C;";
                    _auxContent += "Energy=" + inpFrame[14].ToString() + "%";
                    break;
                case EventType.evtAlarmCleared:
                case EventType.evtAlarmRaised:
                    AlarmType alarm = (AlarmType)(LEconverter.LEArrayToUInt16(inpFrame, 8));
                    _auxContent = "Alarm: " + alarm.ToString();
                    break;
                case EventType.evtZeroCalibration:
                    _auxContent = "Offset=" + (LEconverter.LEArrayToSingle(inpFrame, 8)).ToString() + "pt";
                    break;
                case EventType.evtDateTimeChanged:
                    int seconds = inpFrame[8];
                    int minutes = inpFrame[9];
                    int hours = inpFrame[10];
                    DayOfWeek dowOld = (DayOfWeek)inpFrame[11];
                    int day = inpFrame[12];
                    int month = inpFrame[13];
                    int year = (int)(LEconverter.LEArrayToUInt16(inpFrame, 14));

                    DateTime oldDate;
                    try
                    {
                        oldDate = new DateTime(year, month, day, hours, minutes, seconds);
                    }
                    catch 
                    {
                        oldDate = new DateTime();
                    }

                    seconds = inpFrame[16];
                    minutes = inpFrame[17];
                    hours = inpFrame[18];
                    DayOfWeek dowNew = (DayOfWeek)inpFrame[19];
                    day = inpFrame[20];
                    month = inpFrame[21];
                    year = (int)(LEconverter.LEArrayToUInt16(inpFrame, 22));

                    DateTime newDate;
                    try
                    {
                        newDate = new DateTime(year, month, day, hours, minutes, seconds);
                    }
                    catch 
                    {
                        newDate = new DateTime();
                    }

                    _auxContent = "OldDate=" + dowOld.ToString() + ", " + oldDate.ToString();
                    _auxContent += ";NewDate=" + dowNew.ToString() + ", " + newDate.ToString();

                    break;
            }
            return new CommandResult();

        }

        public List<String> fieldNames
        {
            get
            {
                if (_fieldNames == null)
                {
                    _fieldNames = new List<String>();
                    _fieldNames.Add("Index");
                    _fieldNames.Add("Timestamp");
                    _fieldNames.Add("Event type");
                    _fieldNames.Add("Aux data");
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

                _fields.Add(Index.ToString());
                _fields.Add(_timestamp.ToShortDateString() + " " + _timestamp.ToShortTimeString());
                _fields.Add(_type.ToString());
                _fields.Add( _auxContent );
                
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

        private enum resetCause
        {
	        //! No reset occured since last clear
	        None                                      = 0x00,
	        //! Browout reset due to supply rail fail, it's a BOR
	        Brownout_BOR                              = 0x02,
	        //! Reset due to RST pin, it's a BOR
	        RST_NMI_BOR                               = 0x04,
	        //! Software reset explicitly invoked by aplication, it's a BOR
	        PMMSWBOR_BOR                              = 0x06,
	        //! Wake up from LPMx.5 power modes, it's a BOR
	        LPM_DOT5_WAKEUP_BOR                       = 0x08,
	        //! , it's a BOR
	        SecurityViolation_BOR                     = 0x0A,
	        LastBOR 								  = SecurityViolation_BOR,
	        SVSL_POR                                  = 0x0C,
	        SVSH_POR                                  = 0x0E,
	        SVML_OVP_POR                              = 0x10,
	        SVMH_OVP_POR                              = 0x12,
	        PMMSWPOR_POR                              = 0x14,
	        LastPOR 								  = PMMSWPOR_POR,
	        WDT_Timeout_PUC                           = 0x16,
	        WDT_PasswordViolation_PUC                 = 0x18,
	        FlashPasswordViolation_PUC                = 0x1A,
	        FLL_Unlock_PUC                            = 0x1C,
	        PERF_PeripheralConfigurationAreaFetch_PUC = 0x1E,
	        PMM_PasswordViolation_PUC                 = 0x20,
	        LastPUC									  = PMM_PasswordViolation_PUC,
	        Reserved_0x22                             = 0x22,
	        Reserved_0x24                             = 0x24,
	        Reserved_0x26                             = 0x26,
	        Reserved_0x28                             = 0x28,
	        Reserved_0x2A                             = 0x2A,
	        Reserved_0x2C                             = 0x2C,
	        Reserved_0x2E                             = 0x2E,
	        Reserved_0x30                             = 0x30,
	        Reserved_0x32                             = 0x32,
	        Reserved_0x34                             = 0x34,
	        Reserved_0x36                             = 0x36,
	        Reserved_0x38                             = 0x38,
	        Reserved_0x3A                             = 0x3A,
	        Reserved_0x3C                             = 0x3C,
	        Reserved_0x3E                             = 0x3E
        };

        public enum EventType
        {
            evtNone = 0,

            //! Simple events class
            evtProcessLogErased,
            evtUserWakeUp,
            evtEmergencyOff,
            evtSD24busy,
            evtAccumulatorMemoryError,
            evtAccumulatorMemoryFailure,

            //! Alarms events class
            evtAlarmRaised = 0x10,
            evtAlarmCleared,

            //! Battery/supply voltage class
            evtBatteryReplaced,
            evtBatteryLow,
            evtBatteryExhausting,
            evtBatteryFull,

            //! Boot events class
            evtBoot,

            //! Eeprom parameters class
            evtParameterChanged,

            //! Calibration class
            evtZeroCalibration,

            //! Date time class
            evtDateTimeChanged,

            //!Timeout supervisor
            evtMeasureTOUT,
            evtEPipeTOUT,
            evtConvTempTOUT,
            evtConvPressTOUT,

            _evtLastValidEvt
        };

        [Flags]
        private enum AlarmType
        {
            //salvati nel log
            ExcFailure = 0x0001,
            EmptyPipe = 0x0002,
            FlowMax = 0x0004,
            FlowMin = 0x0008,
            PulsesOverlap = 0x0010,
            ADCrange = 0x0020,
            InputStage = 0x0040,
            MeasElectrodeDry = 0x0080,

            //fuori dal log
            LowVoltage = 0x0100,
            HighTemp = 0x0200,
            LowTemp = 0x0400,
            FirmwareCRC32 = 0x0800,
            InputStage_OP1 = 0x1000,
            InputStage_OP2 = 0x2000,
            EepromCRC16	= 0x4000,
            PcbWetDetected = 0x8000,
        };

        private List<String> _fields;
        private List<String> _fieldNames;

        private DateTime _timestamp;
        private EventType _type;
        private String _auxContent;
    }

    public class GetEventLogLines : GetLogLines<EventLogLine>
    {
        public GetEventLogLines()
            : base()
        {

        }

        public GetEventLogLines(String portName)
            : base(portName)
        {

        }

        public GetEventLogLines(commPortHandler handler)
            : base(handler)
        {

        }

        public override string ToString()
        {
            return "Get Event Log Lines Command";
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            switch ( head.FrameType )
            {
                //case invalidRowFrameType:
                //    return new CommandResult(CommandResultOutcomes.CommandFailed, "Log Lines unavailable");
                case answerErrorFrameType:
                    return new CommandResult(CommandResultOutcomes.CommandFailed, "Read error");
                //case logLockedFrameType:
                //    return new CommandResult(CommandResultOutcomes.CommandFailed, "Log is locked");
                case answerFrameType:
                    StartLine = head.Address;

                    if (!(payload is LogLinesPayload<EventLogLine>))
                        return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong answer frame");

                    logLines = (payload as LogLinesPayload<EventLogLine>).GetLines();


                    uint idx = StartLine;
                    foreach (var line in logLines)
                        line.Index = idx++;

                    return new CommandResult();
 
                default:
                    return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");
            }
        }

        protected override Byte commandFrameType
        { 
            get
            {
                return 0x50;
            }
        }
        private const Byte answerFrameType = 0x50;
        //private const Byte invalidRowFrameType = 0x41;
        //private const Byte logLockedFrameType = 0x42;
        private const Byte answerErrorFrameType = 0x51;
    }

}
