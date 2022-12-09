using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class StdCommandLister
    {
        public static ICollection<StdCommand> GetList()
        {
            List<StdCommand> list = new List<StdCommand>();
            list.Add(new TargetReset());
            list.Add(new FirmwareDownload());
            list.Add(new FirmwareUpdate());
            list.Add(new GetBslLogLines());
            list.Add(new GetDataLogLines());
            list.Add(new ReadRAM());
            list.Add(new ReadEEPROM());
            list.Add(new WriteEEPROM());
            list.Add(new SetDateTime());
            list.Add(new DataLogErase());
            list.Add(new ReadEEPROMpage());
            list.Add(new WriteEEPROMpage());
            list.Add(new EventLogErase());
            list.Add(new GetEventLogLines());
            list.Add(new ReadBslConfigFlags());
            list.Add(new WriteBslConfigFlags());
            list.Add(new ReadVarBundle());
            list.Add(new Operation());
            list.Add(new SetDisplay());
            list.Add(new GetTargetInputs());
            list.Add(new SetTargetOutputs());
            list.Add(new GetTargetStatus());
            list.Add(new GetTargetDateTime());
            return list.AsReadOnly();
        }
    }

    public abstract class StdCommand : Command
    {
        protected StdCommand()
        {

        }

        protected StdCommand(commPortHandler handler)
            : base(handler)
        {

        }

        protected StdCommand(String portname)
            : base(portname)
        {
        }

        protected override CommandResult receiveAnswer(dataReceiver receiver)
        {
            StdCommunicationFrame answer = GetAnswerFrame();
            answer.processor = commonProcessAnswer;
            return answer.receive(receiver);
        }

        protected override int progress
        {
            get
            {
                return 100;
            }
        }

        protected virtual StdCommunicationFrame GetAnswerFrame()
        {
            return new StdCommunicationFrame();
        }

        protected override ICommunicationFrame getCommandFrame()
        {
            switch (protocolAnswerReceived)
            {
                case 0x01:  // WAIT
                    TimeSpan span = TimeSpan.FromMilliseconds(100);

                    DateTime inAwhile = DateTime.Now + span;

                    while (DateTime.Now < inAwhile) ;

                    StdHeader head = new StdHeader();
                    head.FrameType = 0x08;  //POKE
                    head.Address = protocolAnswerId;
                    return new StdCommunicationFrame(head, null);
                case 0x02:  // REFUSED
                case 0x03:  // UNKNOWN
                case 0x04:  // EXPIRED
                    break;
            }

            protocolAnswerReceived = 0;

            return CommandFrame();
        }

        private CommandResult commonProcessAnswer(StdHeader head, StdPayload payload)
        {
            // Checks for protocol messages as WAIT, REFUSED, UNKNOWN
            switch (head.FrameType)
            {
                case 0x01:  // WAIT
                    protocolAnswerReceived = head.FrameType;
                    protocolAnswerId = head.Address;
                    return new CommandResult();
                case 0x02:  // REFUSED
                    protocolAnswerReceived = 0;
                    return new CommandResult(CommandResultOutcomes.CommandFailed, "Command refused by target");
                case 0x03:  // UNKNOWN
                    protocolAnswerReceived = 0;
                    return new CommandResult(CommandResultOutcomes.CommandFailed, "Command is unknown to target");
                case 0x04:  // EXPIRED
                    protocolAnswerReceived = 0;
                    return new CommandResult(CommandResultOutcomes.CommandFailed, "Command is expired, completion unknown");
            }

            protocolAnswerReceived = 0;
            protocolAnswerId = 0;

            return processAnswer(head, payload);
        }

        protected virtual CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            return new CommandResult();
        }

        protected abstract ICommunicationFrame CommandFrame();

        private Byte protocolAnswerReceived = 0;
        private UInt32 protocolAnswerId = 0;
    }
}
