using MC_Suite.Services;

using Euromag.Utility.Endianness;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;

    [Flags]
    public enum BslConfigFlags
    {
        BSL_CONFIG_NONE             = 0x00,
        BSL_CONFIG_UPDATE_DISABLED  = 0x01,
    }

    public class ReadBslConfigFlags : StdCommand
    {
        public ReadBslConfigFlags()
        {
            
        }

        public ReadBslConfigFlags(commPortHandler handler)
            : base(handler)
        {
            
        }

        public ReadBslConfigFlags(String portname)
            : base (portname)
        {
            
        }

        public override string ToString()
        {
            return "Read bootloader configuration flags";
        }

        public BslConfigFlags Flags
        { get; set; }

        protected override CommunicationFrames.ICommunicationFrame CommandFrame()
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

        private const Byte commandFrameType = 0x68;
        private const Byte answerFrameType = 0x68;
        private bool completed;
    }

}
