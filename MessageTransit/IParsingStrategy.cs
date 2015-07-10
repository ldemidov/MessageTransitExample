namespace MessageTransit
{
    public interface IParsingStrategy<out T>
    {

        T ParseData(byte[] data, int packetLength);
    

    }
}