public class Packet
{
    public Packet(GameClient client, string correlationId, string command)
    {
        Client = client;
        CorrelationId = correlationId;
        Command = command;
    }

    public GameClient Client { get; }
    public string CorrelationId { get; }
    public string Command { get; }
}