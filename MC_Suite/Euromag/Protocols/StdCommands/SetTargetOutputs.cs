using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class TargetOutputs
    {
        #region Fields

        private const UInt16 POSITIVE_PULSE_BIT = (UInt16)0x0001U;
        private const UInt16 NEGATIVE_PULSE_BIT = (UInt16)0x0002U;
        private const UInt16 RED_LED_BIT = (UInt16)0x0004U;
        private const UInt16 YELLOW_LED_BIT = (UInt16)0x0008U;

        #endregion

        #region Properties

        public Boolean PositivePulse
        {
            get
            {
                return (RawBits & POSITIVE_PULSE_BIT) == POSITIVE_PULSE_BIT;
            }
            set
            {
                if (value)
                    RawBits |= POSITIVE_PULSE_BIT;
                else
                    RawBits &= (POSITIVE_PULSE_BIT ^ 0xFFFF);
            }
        }

        public Boolean NegativePulse
        {
            get
            {
                return (RawBits & NEGATIVE_PULSE_BIT) == NEGATIVE_PULSE_BIT;
            }
            set
            {
                if (value)
                    RawBits |= NEGATIVE_PULSE_BIT;
                else
                    RawBits &= (NEGATIVE_PULSE_BIT ^ 0xFFFF);
            }
        }

        public Boolean RedLed
        {
            get
            {
                return (RawBits & RED_LED_BIT) == RED_LED_BIT;
            }
            set
            {
                if (value)
                    RawBits |= RED_LED_BIT;
                else
                    RawBits &= (RED_LED_BIT ^ 0xFFFF); 
            }
        }

        public Boolean YellowLed
        {
            get
            {
                return (RawBits & YELLOW_LED_BIT) == YELLOW_LED_BIT;
            }
            set
            {
                if (value)
                    RawBits |= YELLOW_LED_BIT;
                else
                    RawBits &= (YELLOW_LED_BIT ^ 0xFFFF);
            }
        }

        public UInt16 RawBits
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public void AppendTo(StdPayload data)
        {
            data.Append(RawBits);
        }

        #endregion
    }

    public class SetTargetOutputs : StdCommand
    {
        public SetTargetOutputs()
        {

        }

        public SetTargetOutputs(commPortHandler handler)
            : base(handler)
        {

        }

        public SetTargetOutputs(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Set Target's Outputs Command";
        }

        public TargetOutputs Outputs
        {
            get
            {
                if (_outputs == null)
                    _outputs = new TargetOutputs();
                return _outputs;
            }
            set
            {
                if (_outputs != value)
                    _outputs = value;
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

            StdPayload payload = new StdPayload();
            Outputs.AppendTo(payload);

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.PayloadLength = payload.Size;
            head.PayloadType = (Byte)TargetDataType.TYPE_DATA;
            completed = true;

            return new StdCommunicationFrame(head, payload);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            return new CommandResult();
        }

        private const Byte commandFrameType = 0xA0;
        private const Byte answerFrameType = 0xA1;
        private bool completed;

        private TargetOutputs _outputs;
    }
}
