namespace MC_Suite.Euromag.Protocols
{
    using System;

    public enum CommandResultOutcomes
    {
        CommunicationFails = 0,
        CommandFailed,
        CommandSuccess
    }

    public class CommandResult
    {
        public CommandResult(CommandResultOutcomes outcome = CommandResultOutcomes.CommandSuccess)
        {
            _outcome = outcome;
            _msg = _outcome.ToString();
        }

        public CommandResult(CommandResultOutcomes outcome, String message)
        {
            _outcome = outcome;
            _msg = message;
        }

        public CommandResultOutcomes Outcome
        {
            get
            {
                return _outcome;
            }
            set
            {
                _outcome = value;
            }
        }

        public String Message
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
            }
        }

        private CommandResultOutcomes _outcome;
        private String _msg;
    }
}
