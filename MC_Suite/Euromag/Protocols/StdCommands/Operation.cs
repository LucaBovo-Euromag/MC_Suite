using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public enum BoardOperation
    {
        operationNone = 0,
        operationZeroFinder,
        operationResetPositiveAccumulator,
        operationResetNegativeAccumulator,
        operationSaveUserParameters,
        operationLoadUserParameters,
        operationLoadFactParameters,
        operationSaveFactParameters,
        operationInitParametersToDefault,
        operationResetPositiveTotalizer,
        operationResetNegativeTotalizer,
        operationEnterGSMConfigurationMode,
        operationExitGSMConfigurationMode,
    };

    public class Operation: StdCommand
    {
        public Operation()
        {
            init();
        }

        public Operation(commPortHandler handler)
            : base(handler)
        {
            init();
        }

        public Operation(String portname)
            : base (portname)
        {
            init();
        }

        public override string ToString()
        {
            return "Board operations Command";
        }

        public BoardOperation OperationCode
        { get; set; }

        protected override ICommunicationFrame CommandFrame()
        {
            if (completed)
                return null;
            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.Address = operationDict[OperationCode];

            completed = true;

            return new StdCommunicationFrame(head, null);
        }

        protected override void reset()
        {
            completed = false;
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType == errorFrameType)
                return new CommandResult(CommandResultOutcomes.CommandFailed, "Operation failed");
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");
            return new CommandResult();
        }

        private void init()
        {
            operationDict = new Dictionary<BoardOperation, UInt32>();
            operationDict.Add(BoardOperation.operationNone, 0x00);
            operationDict.Add(BoardOperation.operationZeroFinder, 0x01);
            operationDict.Add(BoardOperation.operationResetPositiveAccumulator, 0x02);
            operationDict.Add(BoardOperation.operationResetNegativeAccumulator, 0x03);
            operationDict.Add(BoardOperation.operationSaveUserParameters, 0x04);
            operationDict.Add(BoardOperation.operationLoadUserParameters, 0x05);
            operationDict.Add(BoardOperation.operationLoadFactParameters, 0x06);
            operationDict.Add(BoardOperation.operationSaveFactParameters, 0xAD);
            operationDict.Add(BoardOperation.operationInitParametersToDefault, 0xC5);
            operationDict.Add(BoardOperation.operationResetPositiveTotalizer, 0xF2);
            operationDict.Add(BoardOperation.operationResetNegativeTotalizer, 0xF3);
            operationDict.Add(BoardOperation.operationEnterGSMConfigurationMode, 0xDA);
            operationDict.Add(BoardOperation.operationExitGSMConfigurationMode, 0xDB);

        }

        private const Byte commandFrameType = 0x80;
        private const Byte answerFrameType = 0x81;
        private const Byte errorFrameType = 0x82;
        private bool completed;
        private Dictionary<BoardOperation, UInt32> operationDict;
    }
}
