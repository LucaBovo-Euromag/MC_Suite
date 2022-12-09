using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;

    public interface ILogRecord
    {
        List<String> fieldNames
        {
            get;
        }

        List<String> fields
        {
            get;
        }
    }
    
    
    public abstract class GetLogLines<T> : StdCommand
        where T : IFrameable, ILogRecord, new()
    {
        public GetLogLines()
        {

        }

        public GetLogLines(commPortHandler handler)
            : base(handler)
        {

        }

        public GetLogLines(String portname)
            : base (portname)
        {
        }

        public override string ToString()
        {
            return "Get Log Lines Command";
        }

        public UInt32 StartLine
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
            }
        }

        public virtual Byte Lines
        {
            get
            {
                return linesNum;
            }
            set
            {
                linesNum = (value <= MaxLines)? value : MaxLines;
            }
        }

        public virtual Byte MaxLines
        {
            get
            {
                //TODO: fix this using info from T and StdPayload (or a suitable derived class)
                return (512 / 32);
            }
        }

        public List<T> logLines
        {
            get
            {
                return new List<T>(lines);
            }

            protected set
            {
                lines = value;
            }
        }

        protected override void reset()
        {
            completed = false;
            if (lines != null)
                lines.Clear();
        }

        protected override ICommunicationFrame CommandFrame()
        {
            if (completed)
                return null;

            //TODO: modify StdPayload to manage this
            StdPayload payload = new StdPayload();
            payload.Append(linesNum);

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.Address = address;
            head.PayloadLength = payload.Size;

            completed = true;

            return new StdCommunicationFrame(head, payload);
        }

        protected override StdCommunicationFrame GetAnswerFrame()
        {
            return new StdCommunicationFrame(new StdHeader(), new LogLinesPayload<T>());
        }

        protected abstract Byte commandFrameType
        { get; }

        private bool completed;
        private Byte linesNum;
        private UInt32 address;
        private List<T> lines;
    
    }

    class LogDateTime
    {
        public static DateTime Decode(Byte[] input)
        {
            int hour = input[0];
            int minute = input[1];
            int day = input[2];
            int month = input[3];
            int year = 2000 + input[4];

            DateTime result;
            try
            {
                result = new DateTime(year, month, day, hour, minute, 0);
            }
            catch
            {
                result = new DateTime();
            }
            return result;
        }
    }

    class LogLinesPayload<T> : StdPayload
        where T : IFrameable, ILogRecord, new()
    {
        public List<T> GetLines()
        {
            List<T>  lines = new List<T>();

            while (Frame.Count > 0)
            {
                T line = new T();
                line.Extract(Frame);
                lines.Add(line);
            }

            return lines;
        }
    }
}
