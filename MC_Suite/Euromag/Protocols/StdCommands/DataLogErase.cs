using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;

    public class DataLogErase : StdCommand
    {
        public DataLogErase()
        {
            
        }

        public DataLogErase(commPortHandler handler)
            : base(handler)
        {
            
        }

        public DataLogErase(String portname)
            : base (portname)
        {
            
        }


        public override string ToString()
        {
            return "Data Log Erase Command";
        }


        protected override void reset()
        {
            completed = false;
        }

        protected override ICommunicationFrame CommandFrame()
        {
            if (completed)
            {
                return null;
            }

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;

            completed = true;

            return new StdCommunicationFrame(head, null);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType == errorFrameType)
                return new CommandResult(CommandResultOutcomes.CommandFailed, "Cannot erase log");

            if (head.FrameType == lockedFrameType)
                return new CommandResult(CommandResultOutcomes.CommandFailed, "Log is locked");

            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");
            
            return new CommandResult();
        }

        private const Byte commandFrameType = 0x46;
        private const Byte answerFrameType = 0x46;
        private const Byte lockedFrameType = 0x42;
        private const Byte errorFrameType = 0x47;
        private bool completed;
    }
}
