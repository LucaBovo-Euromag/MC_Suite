namespace MC_Suite.Euromag.Protocols.CommunicationFrames
{
    using System;
    using System.Collections.Generic;

    public class StdHeader : IFrameable, IHeader
    {
        public StdHeader()
        {
            frame = new List<Byte>(SIZE);
            for (int i = 0; i < SIZE; i++)
                frame.Add(0);
            FrameStart = 0xA5;
            HeaderLen = SIZE;
        }

        #region StdHeader interface implementation

        private Byte FrameStart
        {
            get
            {
                return frame[0];
            }

            set
            {
                frame[0] = value;
            }
        }

        private Byte HeaderLen
        {
            get
            {
                return frame[1];
            }

            set
            {
                frame[1] = value;
            }
        }

        public Byte FrameType
        {
            get
            {
                return frame[2];
            }

            set
            {
                frame[2] = value;
            }
        }

        public Byte PayloadType
        {
            get
            {
                return frame[3];
            }

            set
            {
                frame[3] = value;
            }
        }

        public Int32 PayloadLength
        {
            get
            {
                return (Int32)BitConverter.ToUInt16(frame.ToArray(), 4);
            }

            set
            {
                UInt16 len = (UInt16)value;
                Byte[] buff = BitConverter.GetBytes(len);
                frame[4] = buff[0];
                frame[5] = buff[1];
            }
        }

        public UInt32 Address
        {
            get
            {
                return BitConverter.ToUInt32(frame.ToArray(), 6);
            }

            set
            {
                Byte[] buff = BitConverter.GetBytes(value);
                frame[6] = buff[0];
                frame[7] = buff[1];
                frame[8] = buff[2];
                frame[9] = buff[3];
            }
        }

        public List<Byte> UnknownBytes
        {
            get
            {
                Int32 len = Size - SIZE;
                return frame.GetRange(SIZE, len);
            }
        }

        #endregion StdHeader interface implementation

        #region IFrameable interface implementation

        public Int32 Size
        {
            get
            {
                if (frame != null)
                    return frame.Count;
                throw new ArgumentNullException("Header is not initialized");
            }
        }

        public List<Byte> ToList()
        {
            return frame;
        }

        public CommandResult Extract(List<Byte> buff)
        {
            if (buff == null)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Buffer cannot be null");

            frame.Clear();
            frame.AddRange(buff.GetRange(0, SIZE));

            if (FrameStart != 0xA5)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Frame Start Error");

            if (HeaderLen > SIZE)
                frame.AddRange(buff.GetRange(SIZE, HeaderLen - SIZE));

            buff.RemoveRange(0, HeaderLen);

            return new CommandResult();
        }

        #endregion IFrameable interface implementation

        #region IHeader interface implementation

        public CommandResult receive(dataReceiver receiver)
        {
            frame = receiver(SIZE);

            if (FrameStart != 0xA5)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Frame Start Error");

            if (HeaderLen > SIZE)
                frame.AddRange(receiver(HeaderLen - SIZE));

            return new CommandResult();
        }

        #endregion IHeader interface implementation

        private const Int32 SIZE = 10;
        private List<Byte> frame;
    }
}
