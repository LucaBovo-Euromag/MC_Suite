using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class GetVerificationModeStatus : StdCommand
    {
        public GetVerificationModeStatus()
        {

        }

        public GetVerificationModeStatus(commPortHandler handler)
            : base(handler)
        {

        }

        public GetVerificationModeStatus(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Get Verification Mode Status";
        }

        public enum verif_mode
        {
            Disabled = 0,
            Enabled = 1
        };

        public verif_mode Status
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

            _status = (verif_mode)resp[0];

            return new CommandResult();
        }

        private const Byte commandFrameType = 0xC2;
        private const Byte answerFrameType = 0xC3;
        private bool completed;

        private verif_mode _status;
    }
}
