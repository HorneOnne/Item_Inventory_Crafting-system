namespace DIVH_InventorySystem
{
    /// <summary>
    /// Interface for objects that can cause damage.
    /// </summary>
    public interface ICanCauseDamage
    {
        /// <summary>
        /// Gets the amount of damage caused by the object.
        /// </summary>
        /// <returns>The amount of damage caused by the object.</returns>
        public int GetDamage();
    }
}