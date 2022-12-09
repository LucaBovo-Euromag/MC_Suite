using Euromag.Utility.Endianness;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class TargetInputs
    {
        #region Fields

        private const UInt16 BUTTON1_BIT = (UInt16)0x0001U;
        private const UInt16 BUTTON2_BIT = (UInt16)0x0002U;
        private const UInt16 BUTTON3_BIT = (UInt16)0x0004U;
        private const UInt16 BUTTON4_BIT = (UInt16)0x0008U;
        private const UInt16 MAGNETIC_REED_BIT = (UInt16)0x0010U;
        private const UInt16 GP_INPUT_BIT = (UInt16)0x0020U;

        #endregion
        
        #region Properties

        public Boolean Button1
        {
            get
            {
                return (RawBits & BUTTON1_BIT) == BUTTON1_BIT;
            }
        }

        public Boolean Button2
        {
            get
            {
                return (RawBits & BUTTON2_BIT) == BUTTON2_BIT;
            }
        }

        public Boolean Button3
        {
            get
            {
                return (RawBits & BUTTON3_BIT) == BUTTON3_BIT;
            }
        }

        public Boolean Button4
        {
            get
            {
                return (RawBits & BUTTON4_BIT) == BUTTON4_BIT;
            }
        }

        public Boolean MagneticReed
        {
            get
            {
                return (RawBits & MAGNETIC_REED_BIT) == MAGNETIC_REED_BIT;
            }
        }

        public Boolean GeneralPurposePin
        {
            get
            {
                return (RawBits & GP_INPUT_BIT) == GP_INPUT_BIT;
            }
        }

        public UInt16 RawBits
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public void Parse(StdPayload data)
        {
            RawBits = LEconverter.LEArrayToUInt16(data.ToList().ToArray());
        }

        #endregion
    }

    public class GetTargetInputs : StdCommand
    {
        public GetTargetInputs()
        {

        }

        public GetTargetInputs(commPortHandler handler)
            : base(handler)
        {

        }

        public GetTargetInputs(String portname)
            : base (portname)
        {
        }

        public override string ToString()
        {
            return "Get Target's Inputs Command";
        }

        public TargetInputs Inputs
        {
            get
            {
                if (_inputs == null)
                    _inputs = new TargetInputs();
                return _inputs;
            }
            set
            {
                if (_inputs != value)
                    _inputs = value;
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

            Inputs.Parse(payload);

            return new CommandResult();
        }

        private const Byte commandFrameType = 0xA2;
        private const Byte answerFrameType = 0xA3;
        private bool completed;

        private TargetInputs _inputs;
    }
}
