using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public class ReadEEPROM : StdCommand, IHasVariable
    {
        public ReadEEPROM()
        {

        }

        public ReadEEPROM(commPortHandler handler)
            : base(handler)
        {

        }

        public ReadEEPROM(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Read EEPROM Variable Command";
        }

        public ITargetVariable Variable
        {
            get
            {
                return variable;
            }
            set
            {
                IEEPROMvariable var = value as IEEPROMvariable;
                if (var == null)
                    throw new ArgumentException(this.ToString() + "parameters needs to implement IEEPROMvariable interface");
                variable = var;
            }
        }

        public ICollection<ITargetVariable> AvailableVariables
        {
            get
            {
                if (varList == null)
                {
                    varList = new List<ITargetVariable>(AllEEPROMVarsLister.getList().Cast<ITargetVariable>());
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
                throw new NullReferenceException("Variable is not initilized in " + this.ToString());

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.Address = (uint)variable.Address;

            completed = true;

            return new StdCommunicationFrame(head, null);
        }
        string exceptions;
        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            List<byte> VarList = new List<byte>();

            VarList = payload.ToList();

            try
            { 
                variable.Parse(VarList);
            }
            catch(Exception ex)
            {                
                exceptions = ex.ToString();
            }
            return new CommandResult();
        }

        protected override StdCommunicationFrame GetAnswerFrame()
        {
            return new StdCommunicationFrame(new StdHeader(), new StdPayload());
        }

        private const Byte commandFrameType = 0x10;
        private const Byte answerFrameType = 0x10;
        private List<ITargetVariable> varList;

        private bool completed;
        private IEEPROMvariable variable;

    }
}
