using Euromag.Utility.Endianness;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class GSMStatus
    {
        #region Fields
            private const byte GSM_ERROR_STATUS_POWER       = (byte)0x01;
            private const byte GSM_ERROR_STATUS_PIN         = (byte)0x02;
            private const byte GSM_ERROR_STATUS_SIM_MISS    = (byte)0x04;
            private const byte GSM_ERROR_STATUS_SIM_LOCK    = (byte)0x08;
        #endregion

        #region Properties

        public Boolean GSMErrorPower
        {
            get
            {
                return (ErrorBits & GSM_ERROR_STATUS_POWER) == GSM_ERROR_STATUS_POWER;
            }
        }

        public Boolean GSMErrorPin
        {
            get
            {
                return (ErrorBits & GSM_ERROR_STATUS_PIN) == GSM_ERROR_STATUS_PIN;
            }
        }

        public Boolean GSMErrorSimMiss
        {
            get
            {
                return (ErrorBits & GSM_ERROR_STATUS_SIM_MISS) == GSM_ERROR_STATUS_SIM_MISS;
            }
        }

        public Boolean GSM_ErrorSimLock
        {
            get
            {
                return (ErrorBits & GSM_ERROR_STATUS_SIM_LOCK) == GSM_ERROR_STATUS_SIM_LOCK;
            }
        }

        public byte ErrorBits
        {
            get;
            private set;
        }

        public byte Voltage
        {
            get;
            private set;
        }

        public byte SignalLevel
        {
            get;
            private set;
        }

        public byte UpdateParamsReq
        {
            get;
            private set;
        }

        public byte Busy
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public void Parse( List<byte> Data )
        {
            ErrorBits       = Data[0];
            Voltage         = Data[1];
            SignalLevel     = Data[2];
            UpdateParamsReq = Data[3];
            Busy            = Data[4];
        }

        #endregion
    }

    public class GetGSMStatus : StdCommand
    {
        public GetGSMStatus()
        {

        }

        public GetGSMStatus(commPortHandler handler)
            : base(handler)
        {

        }

        public GetGSMStatus(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Get GSM's Status Command";
        }

        public GSMStatus Status
        {
            get
            {
                if (_status == null)
                    _status = new GSMStatus();
                return _status;
            }
            set
            {
                if (_status != value)
                    _status = value;
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
            completed = true;

            return new StdCommunicationFrame(head, null);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            Status.Parse( payload.ToList() );

            return new CommandResult();
        }

        private const Byte commandFrameType = 0xDA;
        private const Byte answerFrameType = 0xDB;
        private bool completed;

        private GSMStatus _status;
    }
}
