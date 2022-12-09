using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;

    public class SetVerificationMode : StdCommand
    {
        public SetVerificationMode()
        {

        }

        public SetVerificationMode(commPortHandler handler)
            : base(handler)
        {

        }

        public SetVerificationMode(String portname)
            : base(portname)
        {
        }

        public enum verif_mode
        {
            Disabled = 0,
            Enabled = 1
        };

        public verif_mode Mode
        {
            set
            {
                _mode = value;
            }
        }

        public override string ToString()
        {
            return "Set Verification Mode";
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
            System.Collections.Generic.List<byte> ValueList = new System.Collections.Generic.List<byte>();
            ValueList.Add((byte)_mode);
            payload.Append(ValueList);

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.PayloadLength = payload.Size;

            completed = true;

            return new StdCommunicationFrame(head, payload);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");
            return new CommandResult();
        }

        private verif_mode _mode;
        private const Byte commandFrameType = 0xC0;
        private const Byte answerFrameType = 0xC1;
        private bool completed;
    }
}
