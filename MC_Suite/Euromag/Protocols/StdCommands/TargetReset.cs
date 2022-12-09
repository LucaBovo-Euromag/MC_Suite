using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;

    public class TargetReset : StdCommand
    {
        public TargetReset()
        {

        }

        public TargetReset(commPortHandler handler)
            : base(handler)
        {

        }

        public TargetReset(String portname)
            : base (portname)
        {
        }

        public override string ToString()
        {
            return "Target Reset Command";
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
            return new CommandResult();
        }

        private const Byte commandFrameType = 0x70;
        private const Byte answerFrameType = 0x71;
        private bool completed;
    }
}
