using MC_Suite.Services;

using Euromag.Utility.Endianness;
namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;

    public class WriteBslConfigFlags : StdCommand
    {
        public WriteBslConfigFlags()
        {
            
        }

        public WriteBslConfigFlags(commPortHandler handler)
            : base(handler)
        {
            
        }

        public WriteBslConfigFlags(String portname)
            : base (portname)
        {
            
        }

        public override string ToString()
        {
            return "Write bootloader configuration flags";
        }

        public BslConfigFlags Flags
        { get; set; }

        protected override CommunicationFrames.ICommunicationFrame CommandFrame()
        {
            if (completed)
                return null;

            StdPayload payload = new StdPayload();
            payload.Append((UInt16)Flags);

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
            
            Flags = (BslConfigFlags) (LEconverter.LEArrayToUInt16(payload.ToList().ToArray()));

            return new CommandResult();
        }

        protected override StdCommunicationFrame GetAnswerFrame()
        {
            return new StdCommunicationFrame(new StdHeader(), new StdPayload());
        }

        protected override void reset()
        {
            completed = false;
        }

        private const Byte commandFrameType = 0x69;
        private const Byte answerFrameType = 0x69;
        private bool completed;
    }
}
