namespace DIVH_InventorySystem
{
    public class Axe : Item, IUseable
    {
        public override bool Use(Player player)
        {
            return true;
        }
    }
}
