using Euromag.Utility.Endianness;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class TargetStatus
    {
        #region Fields

        private const UInt32 EE_DETECTED_BIT = (UInt32)0x00000001U;
        private const UInt32 SEE_DETECTED_BIT = (UInt32)0x00000002U;
        private const UInt32 SEE_SECURED_BIT = (UInt32)0x00000004U;
        private const UInt32 FLASH_DETECTED_BIT = (UInt32)0x00000008U;
        private const UInt32 TESTBENCH_DETECTED_BIT = (UInt32)0x00000010U;
        private const UInt32 AUX_PIN_TEST_PASSED_BIT = (UInt32)0x00000020U;
        // 10 bits gap
        private const UInt32 EXCITATION_FAILURE_BIT = (UInt32)0x00010000U;
        private const UInt32 EMPTY_PIPE_BIT = (UInt32)0x00020000U;
        private const UInt32 FLOW_MAX_BIT = (UInt32)0x00040000U;
        private const UInt32 FLOW_MIN_BIT = (UInt32)0x00080000U;
        private const UInt32 PULSES_OVERLAP_BIT = (UInt32)0x00100000U;
        private const UInt32 ADC_OVER_RANGE_BIT = (UInt32)0x00200000U;
        private const UInt32 INPUT_STAGE_BIT = (UInt32)0x00400000U;
        private const UInt32 ELECTRODE_DRY_BIT = (UInt32)0x00800000U;
        private const UInt32 LOW_VOLTAGE_BIT = (UInt32)0x01000000U;
        private const UInt32 HIGH_TEMP_BIT = (UInt32)0x02000000U;
        private const UInt32 LOW_TEMP_BIT = (UInt32)0x04000000U;
        private const UInt32 FIRMWARE_CRC32_BIT = (UInt32)0x08000000U;
        private const UInt32 INPUT_COMMON_SATURATED_BIT = (UInt32)0x10000000U;
        private const UInt32 INPUT_DIFFERENTIAL_SATURATED_BIT = (UInt32)0x20000000U;
        private const UInt32 EEPROM_CRC16_BIT = (UInt32)0x40000000U;
        private const UInt32 PCB_HUMID_BIT = (UInt32)0x80000000U;

        #endregion

        #region Properties

        public Boolean StdEepromDetected
        {
            get
            {
                return (RawBits & EE_DETECTED_BIT) == EE_DETECTED_BIT;
            }
        }

        public Boolean SafeEepromDetected
        {
            get
            {
                return (RawBits & SEE_DETECTED_BIT) == SEE_DETECTED_BIT;
            }
        }

        public Boolean SafeEepromSecured
        {
            get
            {
                return (RawBits & SEE_SECURED_BIT) == SEE_SECURED_BIT;
            }
        }

        public Boolean FlashDetected
        {
            get
            {
                return (RawBits & FLASH_DETECTED_BIT) == FLASH_DETECTED_BIT;
            }
        }

        public Boolean TestbenchDetected
        {
            get
            {
                return (RawBits & TESTBENCH_DETECTED_BIT) == TESTBENCH_DETECTED_BIT;
            }
        }

        public Boolean AuxPinsTestPassed
        {
            get
            {
                return (RawBits & AUX_PIN_TEST_PASSED_BIT) == AUX_PIN_TEST_PASSED_BIT;
            }
        }

        public Boolean ExcitationFailure
        {
            get
            {
                return (RawBits & EXCITATION_FAILURE_BIT) == EXCITATION_FAILURE_BIT;
            }
        }

        public Boolean EmptyPipe
        {
            get
            {
                return (RawBits & EMPTY_PIPE_BIT) == EMPTY_PIPE_BIT;
            }
        }

        public Boolean FlowMax
        {
            get
            {
                return (RawBits & FLOW_MAX_BIT) == FLOW_MAX_BIT;
            }
        }

        public Boolean FlowMin
        {
            get
            {
                return (RawBits & FLOW_MIN_BIT) == FLOW_MIN_BIT;
            }
        }

        public Boolean PulsesOverlap
        {
            get
            {
                return (RawBits & PULSES_OVERLAP_BIT) == PULSES_OVERLAP_BIT;
            }
        }

        public Boolean AdcOverRange
        {
            get
            {
                return (RawBits & ADC_OVER_RANGE_BIT) == ADC_OVER_RANGE_BIT;
            }
        }

        public Boolean InputStageError
        {
            get
            {
                return (RawBits & INPUT_STAGE_BIT) == INPUT_STAGE_BIT;
            }
        }

        public Boolean DryElectrodes
        {
            get
            {
                return (RawBits & ELECTRODE_DRY_BIT) == ELECTRODE_DRY_BIT;
            }
        }

        public Boolean LowVoltage
        {
            get
            {
                return (RawBits & LOW_VOLTAGE_BIT) == LOW_VOLTAGE_BIT;
            }
        }

        public Boolean HighTemperature
        {
            get
            {
                return (RawBits & HIGH_TEMP_BIT) == HIGH_TEMP_BIT;
            }
        }

        public Boolean LowTemperature
        {
            get
            {
                return (RawBits & LOW_TEMP_BIT) == LOW_TEMP_BIT;
            }
        }

        public Boolean FirmwareCRC32Error
        {
            get
            {
                return (RawBits & FIRMWARE_CRC32_BIT) == FIRMWARE_CRC32_BIT;
            }
        }

        public Boolean InputStageCommonSaturated
        {
            get
            {
                return (RawBits & INPUT_COMMON_SATURATED_BIT) == INPUT_COMMON_SATURATED_BIT;
            }
        }

        public Boolean InputStageDifferentialSaturated
        {
            get
            {
                return (RawBits & INPUT_DIFFERENTIAL_SATURATED_BIT) == INPUT_DIFFERENTIAL_SATURATED_BIT;
            }
        }

        public Boolean EepromCRC16Error
        {
            get
            {
                return (RawBits & EEPROM_CRC16_BIT) == EEPROM_CRC16_BIT;
            }
        }

        public Boolean PcbHumid
        {
            get
            {
                return (RawBits & PCB_HUMID_BIT) == PCB_HUMID_BIT;
            }
        }

        public UInt32 RawBits
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public void Parse(StdPayload data)
        {
            RawBits = LEconverter.LEArrayToUInt32(data.ToList().ToArray());
        }

        #endregion
    }

    public class GetTargetStatus : StdCommand
    {
        public GetTargetStatus()
        {

        }

        public GetTargetStatus(commPortHandler handler)
            : base(handler)
        {

        }

        public GetTargetStatus(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Get Target's Status Command";
        }

        public TargetStatus Status
        {
            get
            {
                if (_status == null)
                    _status = new TargetStatus();
                return _status;
            }
            set
            {
                if (_status != value)
                    _status = value;
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

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            completed = true;

            return new StdCommunicationFrame(head, null);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            Status.Parse(payload);

            return new CommandResult();
        }

        private const Byte commandFrameType = 0xA4;
        private const Byte answerFrameType = 0xA5;
        private bool completed;

        private TargetStatus _status;
    }
}
