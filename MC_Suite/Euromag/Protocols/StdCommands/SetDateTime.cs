using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;

    public class SetDateTime: StdCommand
    {
        public SetDateTime()
        {
            _init();
        }

        public SetDateTime(commPortHandler handler)
            : base(handler)
        {
            _init();
        }

        public SetDateTime(String portname)
            : base (portname)
        {
            _init();
        }

        public override string ToString()
        {
            return "Set Date Time Command";
        }

        public DateTime DateTimeToSet
        {
            set
            {
                _timestamp = value;
                settingNow = false;
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


            if (settingNow)
            {
                _timestamp = DateTime.Now;
            }

            //TODO: move these responsibilities to DateTimePayload:StdPayload class
            StdPayload payload = new StdPayload();
            payload.Append((Byte)_timestamp.Second);
            payload.Append((Byte)_timestamp.Minute);
            payload.Append((Byte)_timestamp.Hour);
            payload.Append((Byte)_timestamp.DayOfWeek);
            payload.Append((Byte)_timestamp.Day);
            payload.Append((Byte)_timestamp.Month);
            payload.Append((UInt16)_timestamp.Year);
            
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

        private void _init()
        {
            settingNow = true;
        }

        private bool settingNow;
        private DateTime _timestamp;
        private const Byte commandFrameType = 0x78;
        private const Byte answerFrameType = 0x79;
        private bool completed;
    }
}
