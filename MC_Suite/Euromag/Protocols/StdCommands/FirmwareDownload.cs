using Euromag.Memory;
using Euromag.Utility.Endianness;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FirmwareDownload : StdCommand
    {
        public FirmwareDownload()
        {
        }

        public FirmwareDownload(commPortHandler handler)
            : base(handler)
        {
        }

        public FirmwareDownload(String portname)
            : base (portname)
        {
        }

        public override string ToString()
        {
            return "Firmware Download Command";
        }

        public MemImage Image
        {
            set
            {
                imageToDownload = value;
            }
            get
            {
                return imageToDownload;
            }
        }

        #region Command class method override

        protected override void reset()
        {
            Tries = 40;
            if (imageToDownload != null)
            {
                recordToSend = imageToDownload.GetEnumerator();
                if (recordToSend.MoveNext() == true)
                {
                    _sendWorkflow = workflowSteps.firstRecord;
                }
                else
                {
                    _sendWorkflow = workflowSteps.completed;
                }
                bytesSent = 0;
            }
        }

        protected override int progress
        {
            get
            {
                UInt64 prog = (UInt64)bytesSent * 100;
                prog = prog / imageToDownload.length;
                return (int)prog;
            }
        }

        protected override String progressString
        {
            get 
            {
                if (bytesSent == 0)
                    return "Waiting for memory erasure";
                else
                    return "Downloading data";
            }
        }

        protected override ICommunicationFrame CommandFrame()
        {
            StdHeader head = new StdHeader();
            FirmwareDownloadPayload payload = new FirmwareDownloadPayload();
            _status = buildingStates.working;
            bool _completed = false;

            while (_status == buildingStates.working)
            {
                switch (_sendWorkflow)
                {
                    case workflowSteps.firstRecord:
                        //if (_readTimeOutBackup == 0)
                        //{
                        //    _readTimeOutBackup = PortReadTimeoutMs;
                        //    PortReadTimeoutMs = FW_DWNLD_READ_TO;
                        //}
                        head.FrameType = commandFrameType;
                        head.Address = recordToSend.Current.address;
                        payload.Clear();
                        payload.Append(recordToSend.Current.data);
                        if (recordToSend.MoveNext() == true)
                        {
                            _sendWorkflow = workflowSteps.nextRecord;
                        }
                        else
                        {
                            _sendWorkflow = workflowSteps.finalize;
                            _completed = true;
                        }
                        break;
                    case workflowSteps.nextRecord:
                        if (payload.recordCanFit(recordToSend.Current, head.Address))
                        {
                            payload.Append(recordToSend.Current.data);
                            if (payload.Full)
                            {
                                _sendWorkflow = workflowSteps.finalize;
                            }
                            if (recordToSend.MoveNext() == false)
                            {
                                _sendWorkflow = workflowSteps.finalize;
                                _completed = true;
                            }
                        }
                        else
                        {
                            _sendWorkflow = workflowSteps.finalize;
                        }
                        break;
                    case workflowSteps.finalize:
                        if (!payload.hasNullContent())
                        {
                            head.PayloadLength = payload.Size;
                            _status = buildingStates.frameReady;
                        }
                        else
                        {
                            bytesSent += (UInt32)payload.Size;
                        }
                        if (_completed)
                        {
                            _sendWorkflow = workflowSteps.completed;
                        }
                        else
                        {
                            _sendWorkflow = workflowSteps.firstRecord;
                        }
                        break;
                    case workflowSteps.completed:
                        //if (_readTimeOutBackup != 0)
                        //{
                        //    PortReadTimeoutMs = _readTimeOutBackup;
                        //    _readTimeOutBackup = 0;
                        //}
                        _status = buildingStates.done;
                        break;
                }

             }
            if (_status == buildingStates.frameReady)
            {
                return new StdCommunicationFrame(head, payload);
            }
            else
                return null;
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");

            FirmwareDownloadPayload downloadPayload = payload as FirmwareDownloadPayload;

            if (downloadPayload == null)
            {
                return new CommandResult(CommandResultOutcomes.CommandFailed, "Payload cannot be converted to FirmwareDownloadPayload");
            }

            if (downloadPayload.Result == FirmwareDownloadPayload.Results.FrameWritten)
            {
                bytesSent += (UInt32)downloadPayload.AnswerWrittenByte;
                return new CommandResult();
            }

            return new CommandResult(CommandResultOutcomes.CommandFailed, downloadPayload.Result.ToString());
        }

        protected override StdCommunicationFrame GetAnswerFrame()
        {
            return new StdCommunicationFrame(new StdHeader(), new FirmwareDownloadPayload());
        }

        #endregion Command class method override

        #region FirmwareDownload class private fields

        private enum buildingStates
        {
            working = 0,
            frameReady,
            done,
            error
        };

        private enum workflowSteps
        {
            firstRecord = 0,
            nextRecord,
            finalize,
            completed,
        };

        private const Byte commandFrameType = 0x62;
        private const Byte answerFrameType = 0x63;
        //private const int FW_DWNLD_READ_TO = 1000;
        private MemImage imageToDownload;
        private List<MemRecord>.Enumerator recordToSend;
        private workflowSteps _sendWorkflow;
        private buildingStates _status;
        private UInt32 bytesSent;
        //private int _readTimeOutBackup;

        #endregion FirmwareDownload class private fields

    }

    class FirmwareDownloadPayload : StdPayload
    {   
        public enum Results
        {
            FrameWritten = 0,
            FlashWriteError = 0x20,
            UnknownError = 0xff
        }

        public FirmwareDownloadPayload()
        {

        }

        public Boolean recordCanFit(MemRecord record, UInt32 startingAddress)
        {
            return (((startingAddress + Size) == record.address) &&
                ((Size + record.length) <= MAX_SIZE));
        }

        public Boolean hasNullContent()
        {
            return !Frame.Any((Byte b) => b != 0xff);
        }

        public UInt16 AnswerWrittenByte
        {
            get
            {
                return LEconverter.LEArrayToUInt16(Frame.ToArray(), 2);
            }
        }

        public Results Result
        {
            get
            {

                if (Frame[0] == 0)
                    return Results.FrameWritten;
                else if ((Frame[0] & (Byte)Results.FlashWriteError) != 0)
                    return Results.FlashWriteError;
                else
                    return Results.UnknownError;
            }
        }
    }
}
