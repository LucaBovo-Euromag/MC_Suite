using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MC_Suite.Properties;
using MC_Suite.Euromag.Protocols;
using MC_Suite.Euromag.Protocols.StdCommands;
using Euromag.Utility.CRC;
using Euromag.Utility.Endianness;

namespace MC_Suite.Euromag.Protocols.CommunicationFrames
{
    using System;
    using System.Collections.Generic;

    public delegate void dataSender(List<Byte> outBuff);
    public delegate List<Byte> dataReceiver(Int32 size);

    public interface IFrameable
    {
        Int32 Size { get; }
        List<Byte> ToList();
        CommandResult Extract(List<Byte> buff);
    }

    public interface IHeader
    {
        CommandResult receive(dataReceiver receiver);
    }

    public interface IPayload<H>
        where H : IHeader
    {
        CommandResult parse(List<Byte> buff, H header);
        CommandResult receive(dataReceiver receiver, H header);
    }

    public interface ICommunicationFrame
    {
        void send(dataSender sender);
        CommandResult receive(dataReceiver receiver);
    }

    public class CommunicationFrame<H, P> : ICommunicationFrame
        where H : IFrameable, IHeader, new()
        where P : IFrameable, IPayload<H>, new()
    {

        public delegate CommandResult CommandProcessor(H head, P Payload);

        public CommunicationFrame()
        {
            header = new H();
            payload = new P();
            isCrcAssigned = false;
            crc16Engine = new CRCengine(CRCengine.CRCCode.CRC_CCITT);
        }

        public CommunicationFrame(CommunicationFrame<H, P> rhs)
            : this(rhs.header, rhs.payload)
        {
            crc16 = rhs.crc16;
            isCrcAssigned = rhs.isCrcAssigned;
        }

        public CommunicationFrame(H anHeader, P aPayload)
            : this()
        {
            header = anHeader;
            payload = aPayload;
        }

        public CommunicationFrame(List<Byte> rhs)
            : this()
        {
            header.Extract(rhs);

            payload.parse(rhs, header);

            crc = rhs;
            isCrcAssigned = true;
        }

        private List<byte> SendFrameBuffer;
        public virtual void send(dataSender sender)
        {
            SendFrameBuffer = new List<byte>();
            SendFrameBuffer.AddRange(header.ToList());
            if (payload != null)
                SendFrameBuffer.AddRange(payload.ToList());
            SendFrameBuffer.AddRange(crc);
            sender(SendFrameBuffer);
        }

        public virtual CommandResult receive(dataReceiver receiver)
        {
            CommandResult valid = header.receive(receiver);

            if (valid.Outcome == CommandResultOutcomes.CommandSuccess)
            {
                if (payload != null)
                    valid = payload.receive(receiver, header);

                crc = receiver(sizeof(UInt16));

                ushort crc16calc = calcCRC16();

                if (crc16 != crc16calc)
                {
                    valid.Outcome = CommandResultOutcomes.CommunicationFails;
                    valid.Message = "CRC error";
                }
                else if (_processor != null)
                {
                    valid = _processor(header, payload);
                }
            }

            return valid;
        }

        public virtual CommandProcessor processor
        {
            set
            {
                _processor = value;
            }
        }

        #region class_CommFrame_protected_section

        //protected abstract Endianness frameEndian
        //{
        //    get;
        //}

        #endregion class_CommFrame_protected_section

        #region class_CommFrame_private_section

        private CRCengine crc16Engine;
        private H header;
        private P payload;
        private UInt16 crc16;
        private bool isCrcAssigned;
        private CommandProcessor _processor;

        private UInt16 calcCRC16()
        {
            Int32 count = header.Size;
            if (payload != null)
                count += payload.Size;
            List<Byte> all = new List<Byte>(count);
            all.AddRange(header.ToList());
            if (payload != null)
                all.AddRange(payload.ToList());
            return (UInt16)crc16Engine.crctable(all.ToArray());
        }

        private List<Byte> crc
        {
            get
            {
                if (!isCrcAssigned)
                {
                    crc16 = calcCRC16();
                    isCrcAssigned = true;
                }
                return new List<Byte>(LEconverter.toLEArray(crc16));
            }
            set
            {
                crc16 = LEconverter.LEArrayToUInt16(value.ToArray());
                isCrcAssigned = true;
            }
        }

        #endregion class_CommFrame_private_section
    }
}
