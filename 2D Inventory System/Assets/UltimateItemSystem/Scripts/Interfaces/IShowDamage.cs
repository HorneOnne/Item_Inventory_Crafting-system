namespace UltimateItemSystem
{
    /// <summary>
    /// Interface for showing a popup text object that represents damage caused by <see cref="ICanCauseDamage"/>.
    /// </summary>
    public interface IShowDamage
    {
        /// <summary>
        /// Shows a popup text object representing the specified amount of damage.
        /// </summary>
        /// <param name="damage">The amount of damage to display.</param>
        public void ShowDamage(int damaged);
    }
}