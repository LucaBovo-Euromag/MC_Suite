using System;
using System.Linq;
using System.Text;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using MC_Suite.Services;
    using System;
    using Euromag.Protocols.CommunicationFrames;
    using System.Collections.Generic;

    public class GetTestMode420mAStatus : StdCommand
    {
        public GetTestMode420mAStatus()
        {

        }

        public GetTestMode420mAStatus(commPortHandler handler)
            : base(handler)
        {

        }

        public GetTestMode420mAStatus(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Get Test Mode 4-20mA Status";
        }

        public enum test_mode
        {
            Disabled = 0,
            Enabled = 1
        };

        public test_mode Status
        {
            get
            {
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

            List<byte> resp;
            resp = payload.ToList();

            _status = (test_mode)resp[0];

            return new CommandResult();
        }

        private const Byte commandFrameType = 0xB4;
        private const Byte answerFrameType = 0xB5;
        private bool completed;

        private test_mode _status;
    }
}
