using MC_Suite.Services;

using Euromag.Memory;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;

    public class WriteEEPROMpage : StdCommand
    {
        public WriteEEPROMpage()
        {

        }

        public WriteEEPROMpage(commPortHandler handler)
            : base(handler)
        {

        }

        public WriteEEPROMpage(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Write EEPROM Page Command";
        }

        public uint PageNumber
        { get; set; }

        public MemImage Page
        {
            get
            {
                if (image == null)
                {
                    image = new MemImage();
                }
                return image;
            }

            set
            {
                image = new MemImage(value);
            }
        }

        protected override CommunicationFrames.ICommunicationFrame CommandFrame()
        {
            if (completed)
                return null;

            if (image == null)
                return null;

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.Address = PageNumber;
            head.PayloadType = (Byte) TargetDataType.TYPE_DATA;
            head.PayloadLength = EE_PAGE_SIZE;

            StdPayload payload = new StdPayload();
            foreach ( MemRecord rec in image )
                payload.Append( rec.data );

            completed = true;

            return new StdCommunicationFrame(head, payload);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType == errorFrameType)
                return new CommandResult(CommandResultOutcomes.CommandFailed, "Wrong data length or type");

            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            return new CommandResult();

        }

        protected override void reset()
        {
            completed = false;
        }

        private const Byte commandFrameType = 0x28;
        private const Byte answerFrameType = 0x28;
        private const Byte errorFrameType = 0x29;

        private const int EE_PAGE_SIZE = 128;

        private bool completed;
        private MemImage image;
    }
}
