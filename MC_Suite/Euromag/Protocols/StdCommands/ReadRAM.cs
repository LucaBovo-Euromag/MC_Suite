using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class ReadRAM : StdCommand, IHasVariable
    {
        public ReadRAM()
        {

        }

        public ReadRAM(commPortHandler handler)
            : base(handler)
        {

        }

        public ReadRAM(String portname)
            : base (portname)
        {
        }

        public override string ToString()
        {
            return "Read RAM Variable Command";
        }

        public ITargetVariable Variable
        {
            get
            {
                return variable;
            }
            set
            {
                IRAMvariable var = value as IRAMvariable;
                if (var == null)
                    throw new ArgumentException(this.ToString() + "parameters needs to implement IRAMvariable interface");
                variable = var;
            }
        }

        public ICollection<ITargetVariable> AvailableVariables
        {
            get
            { return AllRAMVarsLister.getList(); }
        }

        protected override void reset()
        {
            completed = false;
        }

        protected override ICommunicationFrame CommandFrame()
        {
            if (completed)
                return null;

            if (variable == null)
                throw new NullReferenceException("Variable is not initilized in "+this.ToString());

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.Address = (uint)variable.Address;

            completed = true;

            return new StdCommunicationFrame(head, null);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            variable.Parse(payload.ToList());

            return new CommandResult();
        }

        protected override StdCommunicationFrame GetAnswerFrame()
        {
            return new StdCommunicationFrame(new StdHeader(), new StdPayload());
        }

        private const Byte commandFrameType = 0x30;
        private const Byte answerFrameType = 0x30;

        private bool completed;
        private IRAMvariable variable;

    }
}
