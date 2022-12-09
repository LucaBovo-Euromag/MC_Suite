namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using MC_Suite.Services;

    public class Set420mATestFlowLev : StdCommand
    {
        public Set420mATestFlowLev()
        {

        }

        public Set420mATestFlowLev(commPortHandler handler)
            : base(handler)
        {

        }

        public Set420mATestFlowLev(String portname)
            : base(portname)
        {

        }

        public override string ToString()
        {
            return "Set Flow Level for 4-20mA Test";
        }

        public float FlowLevel
        {
            set
            {
                if (_flowlevel != value)
                    _flowlevel = value;
            }
            get
            {
                return _flowlevel;
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

            //TODO: move these responsibilities to DateTimePayload:StdPayload class
            StdPayload payload = new StdPayload();

            byte[] value = BitConverter.GetBytes(_flowlevel);
            System.Collections.Generic.List<byte> ValueList = new System.Collections.Generic.List<byte>(); 
            for(int i=0; i < sizeof(float); i++)
            {
                ValueList.Add(value[i]);
            }           
            payload.Append(ValueList);

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

        private float _flowlevel;
        private const Byte commandFrameType = 0xB8;
        private const Byte answerFrameType = 0xB9;
        private bool completed;
    }
}
