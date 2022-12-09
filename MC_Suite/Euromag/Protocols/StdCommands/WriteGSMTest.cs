using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class WriteGSMTest : StdCommand, IHasWritableVariable
    {
        public WriteGSMTest()
        {

        }

        public WriteGSMTest(commPortHandler handler)
            : base(handler)
        {

        }

        public WriteGSMTest(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Write WriteGSMTest Variable Command";
        }

        ITargetVariable IHasVariable.Variable
        {
            get
            {
                return variable;
            }
            set
            {
                ITargetWritable var = value as ITargetWritable;
                if (var == null)
                    throw new ArgumentException(this.ToString() + "parameters needs to implement ITargetWritable interface");
                this.Variable = var;
            }
        }

        ICollection<ITargetVariable> IHasVariable.AvailableVariables
        {
            get
            { 
                return this.AvailableVariables as ICollection<ITargetVariable>;
            }
        }

        public ITargetWritable Variable
        {
            get
            {
                return variable;
            }
            set
            {
                IGSMvariable var = value as IGSMvariable;
                if (var == null)
                    throw new ArgumentException(this.ToString() + "parameters needs to implement IGSMvariable interface");
                variable = var;
            }
        }

        public ICollection<ITargetWritable> AvailableVariables
        {
            get
            { return AllGSMTestVarsLister.getList(); }
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
                throw new NullReferenceException("Variable is not initilized in " + this.ToString());

            StdPayload payload = new StdPayload();
            payload.Append(variable.ToList());

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.Address = (uint)variable.Address;
            head.PayloadType = (Byte)variable.DataType;
            head.PayloadLength = variable.Size;

            completed = true;

            return new StdCommunicationFrame(head, payload);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            CommandResultOutcomes res = CommandResultOutcomes.CommandSuccess;
            String outString = CommandResultOutcomes.CommandSuccess.ToString();
            
            if (head.FrameType == answerWrongDataFrameType)
                return new CommandResult(CommandResultOutcomes.CommandFailed, "Data Type/Length Mismatch");
            else if (head.FrameType == answerTruncatedFrameType)
            {
                res = CommandResultOutcomes.CommandFailed;
                outString = "Data out of bound. Value truncated to fit";
            }
            else if (head.FrameType == answerErrorFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "ERROR 0xE5: ");
            else if (head.FrameType != answerOkFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            variable.Parse(payload.ToList());

            return new CommandResult(res, outString);
        }

        protected override StdCommunicationFrame GetAnswerFrame()
        {
            return new StdCommunicationFrame(new StdHeader(), new StdPayload());
        }

        private const Byte commandFrameType     = 0xD9;
        private const Byte answerOkFrameType    = 0xD0;
        private const Byte answerErrorFrameType = 0xE5;
        private const Byte answerUnkownErrorFrameType = 0xE0; 

        private const Byte answerTruncatedFrameType = 0x22;
        private const Byte answerWrongDataFrameType = 0x23;

        private bool completed;
        private IGSMvariable variable;

    }
}
