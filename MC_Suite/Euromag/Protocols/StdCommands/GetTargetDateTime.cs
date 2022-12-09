using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class TargetDateTime
    {
        public void Decode(List<byte> Data)
        {
            int second = Data[0];
            int minute = Data[1];
            int hour = Data[2];
            int dayofweek = Data[3];
            int day = Data[4];
            int month = Data[5];
            int year = (Data[7]*256) + Data[6];

            try
            {
                Date = new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                Date = new DateTime();
            }
        }

        public DateTime Date
        {
            get;
            private set;
        }
   }

    public class GetTargetDateTime : StdCommand
    {
        public GetTargetDateTime()
        {

        }

        public GetTargetDateTime(commPortHandler handler)
            : base(handler)
        {

        }

        public GetTargetDateTime(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Get Target's Date Time";
        }

        public TargetDateTime Date_Time
        {
            get
            {
                if (_date_Time == null)
                    _date_Time = new TargetDateTime();
                return _date_Time;
            }
            set
            {
                if (_date_Time != value)
                    _date_Time = value;
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

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            completed = true;

            return new StdCommunicationFrame(head, null);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            Date_Time.Decode( payload.ToList() );            

            return new CommandResult();
        }

        private const Byte commandFrameType = 0x7A;
        private const Byte answerFrameType = 0x7B;
        private bool completed;

        private TargetDateTime _date_Time;
    }
}
