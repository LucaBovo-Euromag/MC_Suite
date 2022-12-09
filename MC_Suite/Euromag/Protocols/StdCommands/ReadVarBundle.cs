using MC_Suite.Services;

using Euromag.Utility.Endianness;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ReadVarBundle : StdCommand
    {
        public const UInt32 EEPROM_BUNDLE_BASE = 0x0U;
        public const UInt32 IO_PARAMS_BUNDLE_BASE = 0x4000U;
        public const UInt32 IO_VARIABLES_BUNDLE_BASE = 0x8000U;
        public const UInt32 RAM_BUNDLE_BASE = 0x10000U;
        public const UInt32 RAM_MEASURE_BUNDLE = RAM_BUNDLE_BASE + 0x0U;
        public const UInt32 RAM_OTHERS_BUNDLE = RAM_BUNDLE_BASE + 0x1U;
        public const UInt32 RAM_BUNDLE_T1_T2_PRESS = RAM_BUNDLE_BASE + 0x2U;
        public const UInt32 RAM_BUNDLE_BT_RS485 = RAM_BUNDLE_BASE + 0x3U;

        public ReadVarBundle()
        {

        }

        public ReadVarBundle(commPortHandler handler)
            : base(handler)
        {

        }

        public ReadVarBundle(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Read Variables Bundle Command";
        }

        public Dictionary<Type, ITargetVariable> BundleSet
        {
            get
            {
                if (_bundleSet == null)
                    _bundleSet = new Dictionary<Type, ITargetVariable>();
                return _bundleSet;
            }

            set
            {
                _bundleSet = value;
            }
        }

        public UInt32 BundleId
        { get; set; }

        protected override ICommunicationFrame CommandFrame()
        {
            if (completed)
                return null;

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.Address = BundleId;

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
                return new CommandResult(CommandResultOutcomes.CommandFailed, "Error in command processing");
            
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            List<Byte> recBytes = payload.ToList();

            while (recBytes.Count > 1)
            {
                ITargetVariable currentVar = null;
                Type currentType = null;

                if (BundleId >= IO_VARIABLES_BUNDLE_BASE)
                {
                    RAMAddresses varId = (RAMAddresses)LEconverter.LEArrayToUInt16(recBytes.ToArray());
                    recBytes = recBytes.Skip(sizeof(UInt16)).ToList();

                    currentVar = ramVarList.SingleOrDefault(var => (var as IRAMvariable).Address == varId);
                    currentType = currentVar.GetType();
                }
                else
                {
                    EEPROMAddresses varId = (EEPROMAddresses)LEconverter.LEArrayToUInt16(recBytes.ToArray());
                    recBytes = recBytes.Skip(sizeof(UInt16)).ToList();

                    currentVar = eepVarList.SingleOrDefault(var => (var as IEEPROMvariable).Address == varId);
                    currentType = currentVar.GetType();
                }

                if (!BundleSet.ContainsKey(currentType))
                {
                    BundleSet.Add(currentType, currentVar);
                }

                BundleSet[currentType].Parse(recBytes);
                recBytes = recBytes.Skip(BundleSet[currentType].Size).ToList();
            }
            
            return new CommandResult();
        }

        protected override StdCommunicationFrame GetAnswerFrame()
        {
            return new StdCommunicationFrame(new StdHeader(), new StdPayload());
        }

        private const Byte commandFrameType = 0x90;
        private const Byte answerFrameType = 0x91;
        private const Byte errorFrameType = 0x92;

        private ICollection<ITargetVariable> ramVarList = AllRAMVarsLister.getList();
        private ICollection<ITargetWritable> eepVarList = AllEEPROMVarsLister.getList();

        private Dictionary<Type, ITargetVariable> _bundleSet;

        private bool completed;
    }
}
