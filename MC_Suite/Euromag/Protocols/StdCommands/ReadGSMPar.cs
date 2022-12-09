using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{

    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class ReadGSMPar : StdCommand, IHasVariable
    {
        public ReadGSMPar()
        {

        }

        public ReadGSMPar(commPortHandler handler)
            : base(handler)
        {

        }

        public ReadGSMPar(String portname)
            : base (portname)
        {
        }

        public override string ToString()
        {
            return "Read GSM Variable Command";
        }

        public ITargetVariable Variable
        {
            get
            {
                return variable;
            }
            set
            {
                IGSMvariable var = value as IGSMvariable;
                if (var == null)
                    throw new ArgumentException(this.ToString() + "parameters needs to implement GSMVarible interface");
                variable = var;
            }
        }

        public ICollection<ITargetVariable> AvailableVariables
        {
            get
            {
                if (varList == null)
                {
                    varList = new List<ITargetVariable>(AllGSMVarsLister.getList().Cast<ITargetVariable>());
                }
                return varList;
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
            
            if (head.FrameType == answerErrorFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Data type/values ");
            else if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            variable.Parse(payload.ToList());

            return new CommandResult();
        }

        protected override StdCommunicationFrame GetAnswerFrame()
        {
            return new StdCommunicationFrame(new StdHeader(), new StdPayload());
        }

        private const Byte commandFrameType = 0xD1;
        private const Byte answerFrameType  = 0xD2;
        private const Byte answerErrorFrameType = 0xE1;
        private List<ITargetVariable> varList;
       
        private bool completed;
        private IGSMvariable variable;

    }


}
