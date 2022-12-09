using MC_Suite.Services;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using Euromag.Protocols.CommunicationFrames;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Segment
    {
        #region Properties

        /// <summary>
        /// Gets or sets the <b>Segments</b>'s the zero based common pin (column) index
        /// </summary>
        public int CommonPin { get; set; }

        /// <summary>
        /// Gets or sets the <b>Segments</b>'s the zero based segment pin (row) index
        /// </summary>
        public int SegmentPin { get; set; }

        /// <summary>
        /// Gets or sets the <b>Segments</b>'s status, on or off
        /// </summary>
        public Boolean IsOn { get; set; }

        #endregion

        #region ctors

        /// <summary>
        /// Creates a <b>Segment</b> with <b>CommonPin</b> and <b>SegmentPin</b> set to zero
        /// </summary>
        public Segment()
        { }

        /// <summary>
        /// Creates a <b>Segment</b> with the given <b>CommonPin</b> and <b>SegmentPin</b>
        /// </summary>
        /// <param name="segmentPin">Value for <b>SegmentPin</b>, the zero based segment pin (row) index</param>
        /// <param name="commonPin">Value for <b>CommonPin</b>, the zero based common pin (column) index</param>
        /// <param name="on">On/Off status for the <b>Segment</b></param>
        public Segment(int segmentPin, int commonPin, Boolean on = true)
        {
            SegmentPin = segmentPin;
            CommonPin = commonPin;
            IsOn = on;
        }

        #endregion
    }

    public class DisplaySegments
    {
        #region Fields

        private const int COMMON_PINS = 8;

        private readonly int _segmentPins;
        private Byte[] matrix;
        
        #endregion

        #region Properties
        
        public int SegmentPins
        {
            get { return _segmentPins; }
        }

        public int CommonPins
        {
            get { return COMMON_PINS; }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Builds the segment matrix, initially all segments are off
        /// </summary>
        /// <param name="segmentPins">Value for <b>SegmentPins</b></param>
        public DisplaySegments(int segmentPins)
        {
            _segmentPins = segmentPins;
            matrix = new Byte[SegmentPins];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets on/off a single segment
        /// </summary>
        /// <param name="on">True to set segment on, False to set it off</param>
        /// <param name="segmentPin">Zero based segment pin (row) index</param>
        /// <param name="commonPin">Zero based common pin (column) index</param>
        public void SetSegment(int segmentPin, int commonPin, Boolean on = true)
        {
            if ((segmentPin >= SegmentPins) ||
                (commonPin >= CommonPins))
                return;

            Byte mask = (Byte)(((Byte)0x80) >> commonPin);
            
            if (on)
                matrix[segmentPin] |= mask;
            else
                matrix[segmentPin] &= (Byte)~mask;
        }

        /// <summary>
        /// Sets on/off a single segment
        /// </summary>
        /// <param name="segment">Segment to be set</param>
        public void SetSegment(Segment segment)
        {
            SetSegment(segment.SegmentPin, segment.CommonPin, segment.IsOn);
        }

        /// <summary>
        /// Sets on/off the given collection of <b>Segment</b>
        /// </summary>
        /// <param name="collection">The <b>Segment</b>'s collection</param>
        public void SetSegments(IEnumerable<Segment> collection)
        {
            foreach (Segment seg in collection)
                SetSegment(seg);
        }

        /// <summary>
        /// Sets all segments on
        /// </summary>
        public void AllOn()
        {
            for (int index = 0; index < SegmentPins; index++)
                matrix[index] = (Byte)0xFF;
        }

        /// <summary>
        /// Sets all segments off
        /// </summary>
        public void AllOff()
        {
            for (int index = 0; index < SegmentPins; index++)
                matrix[index] = 0;
        }

        public List<Byte> ToList()
        {
            return matrix.ToList();
        }

        #endregion
    }

    public class SetDisplay : StdCommand
    {
        public SetDisplay()
        {

        }

        public SetDisplay(commPortHandler handler)
            : base(handler)
        {

        }

        public SetDisplay(String portname)
            : base (portname)
        {
        }

        public override string ToString()
        {
            return "Set Display Command";
        }

        public DisplaySegments SegmentsStatus
        {
            get
            {
                if (_segmentsStatus == null)
                    _segmentsStatus = new DisplaySegments(DEFAULT_LCD_SEGMENTS_PINS);
                return _segmentsStatus;
            }
            set
            {
                if (_segmentsStatus != value)
                    _segmentsStatus = value;
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

            StdPayload payload = new StdPayload();
            payload.Append(SegmentsStatus.ToList());

            StdHeader head = new StdHeader();
            head.FrameType = commandFrameType;
            head.PayloadType = (Byte)TargetDataType.TYPE_DATA;
            head.PayloadLength = payload.Size;

            completed = true;

            return new StdCommunicationFrame(head, payload);
        }

        protected override CommandResult processAnswer(StdHeader head, StdPayload payload)
        {
            if (head.FrameType != answerFrameType)
                return new CommandResult(CommandResultOutcomes.CommunicationFails, "Wrong Answer Frame Type");
            return new CommandResult();
        }

        private const Byte commandFrameType = 0xA6;
        private const Byte answerFrameType = 0xA7;
        private bool completed;
        private DisplaySegments _segmentsStatus;
        private const int DEFAULT_LCD_SEGMENTS_PINS = 18;
    }
}
