using System;
using System.Text;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string ItemName { get => AddSpacesBeforeUpperCase(this.name);}
    
    [Header("Item Properties")]
    public Sprite icon;
    public ItemType itemType;
    public ushort max_quantity;   
    [Multiline(5)]
    public string description;


    public override bool Equals(object other)
    {
        if (this.ItemName != ((ItemData)other).ItemName) return false;
        if (this.icon != ((ItemData)other).icon) return false;
        if (this.itemType != ((ItemData)other).itemType) return false;
        if (this.max_quantity != ((ItemData)other).max_quantity) return false;
        if (this.description != ((ItemData)other).description) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ItemName, icon, itemType, max_quantity, description);
    }

    public string AddSpacesBeforeUpperCase(string input)
    {
        StringBuilder output = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i != 0)
            {
                output.Append(" ");
            }
            output.Append(input[i]);
        }
        return output.ToString();
    }
}

public enum ItemType
{
    Null = 0,
    ChestArmor = 1,
    Helm = 2,
    Shield = 3,
    Weapon = 4,
    Shoes = 5,
    LightSource = 6,
    Currency = 7,
    Consumption = 8, 
    Tool = 9,
    Buff = 10, 
    Material = 11,
    Projectile = 12,
}
