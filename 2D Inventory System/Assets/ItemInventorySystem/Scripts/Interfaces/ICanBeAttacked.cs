namespace DIVH_InventorySystem
{

    /// <summary>
    /// Interface for objects that can be attacked.
    /// </summary>
    public interface ICanBeAttacked
    {
        /// <summary>
        /// The time between two instances of taking damage.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Called when the object is attacked.
        /// </summary>
        /// <param name="damage">Amount of damage to inflict on the object.</param>
        public void BeAttacked(int damaged);
    }
}

