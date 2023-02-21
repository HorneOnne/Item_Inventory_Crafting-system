namespace DIVH_InventorySystem
{
    public interface ICanBeAttacked
    {
        public float Cooldown { get; set; } // The time between two instances of taking damage
        public void BeAttacked(int damaged);
    }
}

