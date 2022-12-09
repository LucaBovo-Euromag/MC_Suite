using Euromag.Memory;
using Euromag.Utility.Endianness;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public class FirmwareUpdate : StdCommand
    {
        public FirmwareUpdate()
        {
        }

        public FirmwareUpdate(commPortHandler handler)
            : base(handler)
        {
        }

        public FirmwareUpdate(String portname)
            : base (portname)
        {
        }


        public override string ToString()
        {
            return "Firmware Update Command";
        }

        public MC406_fwImage fwImage
        {
            set
            {
                firmware = value;
            }
            get
            {
                return firmware;
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

            //TODO: modify StdPayload to manage this
            List<Byte> info = new List<Byte>();

            if (firmware != null)
            {
                info.Add(firmware.firmwareInfo.version);
                info.Add(firmware.firmwareInfo.revision);
                info.Add(firmware.firmwareInfo.boardId);
                info.AddRange(LEconverter.toLEArray(firmware.firmwareInfo.hwVersionMask));
                info.AddRange(LEconverter.toLEArray(firmware.firmwareInfo.crc32));
            }

            StdPayload payload = new StdPayload();
            payload.Append(info);

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.PayloadLength = payload.Size;

            completed = true;

            return new StdCommunicationFrame(head, payload);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            FwUpdatePayload updatePayload = payload as FwUpdatePayload;

            if (updatePayload == null)
            {
                return new CommandResult(CommandResultOutcomes.CommandFailed, "Payload cannot be converted to FwUpdatePayload");
            }
        
            String errorString = String.Empty;

            switch (updatePayload.Validity)
            {
                case FwUpdatePayload.FwValidity.FW_VALID:
                    return new CommandResult(CommandResultOutcomes.CommandSuccess);
                case FwUpdatePayload.FwValidity.FW_NO_CRC:
                    errorString = "No crc";
                    break;
                case FwUpdatePayload.FwValidity.FW_WRONG_CRC:
                    errorString = "Wrong crc";
                    break;
                case FwUpdatePayload.FwValidity.FW_WRONG_BOARD_ID:
                    errorString = "Board Id doesn't match";
                    break;
                case FwUpdatePayload.FwValidity.FW_WRONG_HW_VER:
                    errorString = "Hw version in not compatible";
                    break;
                case FwUpdatePayload.FwValidity.FW_LOG_FULL:
                    errorString = "Bootloader log is full";
                    break;
                case FwUpdatePayload.FwValidity.FW_UPDATE_DISABLED:
                    errorString = "Firmware update is disabled";
                    break;
                default:
                case FwUpdatePayload.FwValidity.FW_UNKNOW:
                    errorString = "Unknown validity state";
                    break;
            }

            return new CommandResult(CommandResultOutcomes.CommandFailed, errorString);
        }

        protected override StdCommunicationFrame GetAnswerFrame()
        {
            return new StdCommunicationFrame(new StdHeader(), new FwUpdatePayload());
        }

        private const Byte commandFrameType = 0x60;
        private const Byte answerFrameType = 0x61;
        private MC406_fwImage firmware;
        private bool completed;
    }

    class FwUpdatePayload : StdPayload
    {
        public enum FwValidity
        {
            FW_UNKNOW = 0,
            FW_VALID,
            FW_NO_CRC,
            FW_WRONG_CRC,
            FW_WRONG_BOARD_ID,
            FW_WRONG_HW_VER,
            FW_LOG_FULL,
            FW_UPDATE_DISABLED
        }
        
        public FwUpdatePayload()
        {

        }

        public FwValidity Validity
        {
            get 
            {
                FwValidity valid = (FwValidity)Enum.ToObject(typeof(FwValidity), (Byte)(Frame[4]));

                return valid;
            }
        }
    }
}
