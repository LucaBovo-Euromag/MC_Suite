using MC_Suite.Services;
namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;

    public class EventLogErase : StdCommand
    {
        public EventLogErase()
        {
            
        }

        public EventLogErase(commPortHandler handler)
            : base(handler)
        {
            
        }

        public EventLogErase(String portname)
            : base(portname)
        {
            
        }


        public override string ToString()
        {
            return "Event Log Erase Command";
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
            //if (head.FrameType == errorFrameType)
            //    return new CommandResult(CommandResultOutcomes.CommandFailed, "Cannot erase log");

            if (head.FrameType == lockedFrameType)
                return new CommandResult(CommandResultOutcomes.CommandFailed, "Log is locked");

            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            return new CommandResult();
        }

        
        private const Byte commandFrameType = 0x5A;
        private const Byte answerFrameType = 0x5A;
        private const Byte lockedFrameType = 0x5B;
        //private const Byte errorFrameType = 0x5B;
        private bool completed;
        
    }
}
