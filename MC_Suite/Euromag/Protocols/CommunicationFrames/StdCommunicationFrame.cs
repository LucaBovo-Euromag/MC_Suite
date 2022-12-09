namespace MC_Suite.Euromag.Protocols.CommunicationFrames
{
    public class StdCommunicationFrame : CommunicationFrame<StdHeader, StdPayload>
    {
        public StdCommunicationFrame()
        {

        }

        public StdCommunicationFrame(StdHeader anHeader, StdPayload aPayload)
            : base(anHeader, aPayload)
        {
        }
    }
}
