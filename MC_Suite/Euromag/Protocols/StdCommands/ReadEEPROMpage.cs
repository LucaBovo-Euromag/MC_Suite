using Euromag.Memory;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class ReadEEPROMpage : StdCommand
    {
        public ReadEEPROMpage()
        {

        }

        public ReadEEPROMpage(commPortHandler handler)
            : base(handler)
        {

        }

        public ReadEEPROMpage(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Read EEPROM Page Command";
        }

        public uint PageNumber
        { get; set; }

        public MemImage Page
        {
            get
            {
                return new MemImage(image);
            }
        }

        protected override CommunicationFrames.ICommunicationFrame CommandFrame()
        {
            if (completed)
                return null;

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.Address = PageNumber;
            
            completed = true;

            return new StdCommunicationFrame(head, null);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            image = new MemImage();

            List<Byte> data = payload.ToList();

            uint addr = PageNumber * (uint)data.Count;

            while (data.Count > 0)
            {
                MemRecord rec = new MemRecord();
                rec.address = addr;
                addr += RECORD_LEN;
                rec.data = data.GetRange(0, RECORD_LEN);
                data.RemoveRange(0, RECORD_LEN);
                image.Add(rec);
            }

            return new CommandResult();
            
        }

        protected override void reset()
        {
            completed = false;
        }

        private const Byte commandFrameType = 0x18;
        private const Byte answerFrameType = 0x18;

        private const int RECORD_LEN = 16;

        private bool completed;
        private MemImage image;
    }
}
