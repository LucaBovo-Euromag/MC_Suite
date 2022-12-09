namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using MC_Suite.Services;

    public class TestMode420mA : StdCommand
    {
        public TestMode420mA()
        {

        }

        public TestMode420mA(commPortHandler handler)
            : base(handler)
        {

        }

        public TestMode420mA(String portname)
            : base(portname)
        {
        }

        public enum test_mode
        {
            Disabled    = 0,
            Enabled     = 1
        };

        public test_mode Mode
        {
            set
            {
                _mode = value;
            }
        }

        public override string ToString()
        {
            return "Test Mode enable-disable for 4-20 mA output";
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

            StdPayload payload = new StdPayload();
            System.Collections.Generic.List<byte> ValueList = new System.Collections.Generic.List<byte>();
            ValueList.Add((byte)_mode);
            payload.Append(ValueList);

            completed = true;

            return new StdCommunicationFrame(head, payload);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");
            return new CommandResult();
        }

        private test_mode _mode;
        private const Byte commandFrameType = 0xB2;
        private const Byte answerFrameType = 0xB3;
        private bool completed;
    }
}
