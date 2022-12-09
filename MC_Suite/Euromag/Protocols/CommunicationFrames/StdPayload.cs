using Euromag.Utility.Endianness;

namespace MC_Suite.Euromag.Protocols.CommunicationFrames
{
    using System;
    using System.Collections.Generic;

    public class StdPayload : IFrameable, IPayload<StdHeader>
    {
        public StdPayload()
        {
            MAX_SIZE = 512;
            frame = new List<Byte>(MAX_SIZE);
        }

        public void Append(List<Byte> data)
        {
            frame.AddRange(data);
        }

        public void Append(Byte data)
        {
            frame.Add(data);
        }

        public void Append(UInt16 data)
        {
            frame.AddRange(LEconverter.toLEArray(data));
        }

        public Boolean Full
        {
            get
            {
                return (Size == MAX_SIZE);
            }
        }

        public void Clear()
        {
            frame.Clear();
        }

        #region IFrameable interface implementation

        public Int32 Size
        {
            get
            {
                if (frame != null)
                    return frame.Count;
                throw new ArgumentNullException("Payload is not initialized");
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
            frame.AddRange(buff);

            buff.Clear();

            return new CommandResult();
        }

        #endregion IFrameable interface implementation

        #region IPayload interface implementation

        public CommandResult parse(List<Byte> buff, StdHeader header)
        {
            if (buff == null)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Buffer cannot be null");

            if (buff.Count < header.PayloadLength)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Buffer is not long enough");

            frame.Clear();
            frame.AddRange(buff.GetRange(0, header.PayloadLength));

            buff.RemoveRange(0, header.PayloadLength);

            return new CommandResult();
        }

        public CommandResult receive(dataReceiver receiver, StdHeader header)
        {
            List<Byte> recvdframe = receiver(header.PayloadLength);

            if (recvdframe != null)
                frame = recvdframe;

            return new CommandResult();
        }

        #endregion IPayload interface implementation

        /// <summary>
        /// Protected accessor to allow derived class to access to frame
        /// without completely forego encapsulation
        /// </summary>
        protected List<Byte> Frame
        {
            get
            { return frame; }
            set
            { frame = value; }
        }

        protected readonly Int32 MAX_SIZE;

        private List<Byte> frame;
    }
}
