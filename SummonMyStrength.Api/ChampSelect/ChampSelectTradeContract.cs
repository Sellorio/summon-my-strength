namespace SummonMyStrength.Api.ChampSelect
{
    public class ChampSelectTradeContract
    {
        public long CellId { get; set; }
        public long Id { get; set; }
        public ChampSelectTradeState State { get; set; }
    }
}
