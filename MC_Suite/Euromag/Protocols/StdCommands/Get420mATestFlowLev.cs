using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using MC_Suite.Services;
    using System;
    using Euromag.Protocols.CommunicationFrames;

    public class Get420mATestFlowLev : StdCommand
    {
        public Get420mATestFlowLev()
        {

        }

        public Get420mATestFlowLev(commPortHandler handler)
            : base(handler)
        {

        }

        public Get420mATestFlowLev(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Get Test Mode 4-20mA Status";
        }

        public float FlowLevel
        {
            get
            {
                return _flowlevel;
            }
            set
            {
                if (_flowlevel != value)
                    _flowlevel = value;
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

            byte[] resp = payload.ToList().ToArray();

            _flowlevel = System.BitConverter.ToSingle(resp, 0);

            return new CommandResult();
        }

        private const Byte commandFrameType = 0xB6;
        private const Byte answerFrameType = 0xB7;
        private bool completed;

        private float _flowlevel;
    }
}
