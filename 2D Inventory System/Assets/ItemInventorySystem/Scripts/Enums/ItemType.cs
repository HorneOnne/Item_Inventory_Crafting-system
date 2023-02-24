namespace DIVH_InventorySystem
{
    /// <summary>
    /// Enumeration of different types of items that can exist in the game.
    /// </summary>
    public enum ItemType
    {
        // Armor
        ChestArmor,
        Helm,
        Shield,
        Shoes,

        // Weapons
        Bow,
        Sword,
        Axe,
        Hammer,
        Boomerang,
        MagicStaff,

        // Ammunition
        Arrow,
        Bullet,

        // Consumables
        Buff,
        Consumption,

        // Crafting materials
        Material,

        // Furniture
        Chest,
        Workbench,
        Anvil,

        // Miscellaneous
        CombatDummy,
        Currency,
        LightSource,
        Null
    }
}
