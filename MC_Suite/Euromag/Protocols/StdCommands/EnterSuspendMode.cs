using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;

    public class EnterSuspendMode : StdCommand
    {
        public EnterSuspendMode()
        {

        }

        public EnterSuspendMode(commPortHandler handler)
            : base(handler)
        {

        }

        public EnterSuspendMode(String portname)
            : base (portname)
        {
        }

        public override string ToString()
        {
            return "Enter Suspend Mode";
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

        private const Byte commandFrameType = 0xB0;
        private const Byte answerFrameType = 0xB1;
        private bool completed;
    }
}
