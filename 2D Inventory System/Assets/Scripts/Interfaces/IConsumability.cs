public interface IConsumability
{
    public bool Consumability { get; set; }
    public void Consume(Player fromPlayer);
}
