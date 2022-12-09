using System.Collections.Generic;
using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;

    public class FileDwnld
    {
        public void Decode(List<byte> Data, Int32 Lenght)
        {
 
        }
    }

    public class FileDownload : StdCommand
    {
        public FileDownload()
        {

        }

        public FileDownload(commPortHandler handler)
            : base(handler)
        {

        }

        public FileDownload(String portname)
            : base(portname)
        {
        }

        public override string ToString()
        {
            return "Download Files Data";
        }

        public uint FileIndex
        {
            get
            {
                return FIndex;
            }

            set
            {
                FIndex = value;
            }
        }

        public int DataLenght
        {
            get
            {
                return DataLen;
            }

            set
            {
                DataLen = value;
            }
        }

        public byte AnswerFrame
        {
            get
            {
                return AnswFrame;
            }

            set
            {
                AnswFrame = value;
            }
        }

        public byte FrameAsk
        {
            get
            {
                return commandFrameAsk;
            }
        }

        public byte FrameOk
        {
            get
            {
                return answerFrameOk;
            }
        }

        public byte FrameStart
        {
            get
            {
                return answerFrameStart;
            }
        }

        public byte FrameEnd
        {
            get
            {
                return answerFrameEnd;
            }
        }

        public byte FrameWait
        {
            get
            {
                return answerFrameWait;
            }
        }

        public List<byte> FileData
        {
            get
            {
                return FileBuff;
            }

            set
            {
                FileBuff = value;
            }
        }

        public FileDwnld File_Dwnld
        {
            get
            {
                if (_file_dwnld == null)
                    _file_dwnld = new FileDwnld();
                return _file_dwnld;
            }
            set
            {
                if (_file_dwnld != value)
                    _file_dwnld = value;
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

            StdHeader head  = new StdHeader();
            head.FrameType  = commandFrameAsk;
            head.Address    = FIndex;
            completed       = true;

            return new StdCommunicationFrame(head, null);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            AnswFrame = head.FrameType;
            DataLen   = head.PayloadLength;
            FileBuff  = payload.ToList();

            if ( (head.FrameType != commandFrameAsk) &&
                 (head.FrameType != answerFrameOk   ) &&
                 (head.FrameType != answerFrameStart) &&
                 (head.FrameType != answerFrameEnd  ) &&
                 (head.FrameType != answerFrameWait ) )
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");
           
            return new CommandResult();
        }

        private const Byte commandFrameAsk = 0xFD;
        private const Byte answerFrameOk    = 0xFB;
        private const Byte answerFrameStart = 0xFA;
        private const Byte answerFrameEnd   = 0xFC;
        private const Byte answerFrameWait  = 0xFE;
        private bool completed;
        private uint FIndex;
        private byte AnswFrame;
        private int  DataLen;
        private List<byte> FileBuff;

        private FileDwnld _file_dwnld;
    }
}
