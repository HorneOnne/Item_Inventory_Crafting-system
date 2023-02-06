using System.Collections.Generic;
using UnityEditor;

public static class GeneralItemTypeContainer
{
    public static List<ItemType> weapons = new List<ItemType>()
    {
        ItemType.Sword,
        ItemType.Bow,

    };

    public static List<ItemType> tools = new List<ItemType>()
    { 
        ItemType.Axe,
        ItemType.Hammer,

    };

    public static List<ItemType> projectiles = new List<ItemType>()
    {
        ItemType.Arrow,
        ItemType.Bullet,
    };


    public static List<ItemType> equipments = new List<ItemType>()
    {
        ItemType.Helm,
        ItemType.ChestArmor,
        ItemType.Shield,
    };


    public static List<ItemType> GetAllItemInType(GeneralItemType generalItemType)
    {
        switch(generalItemType)
        {
            case GeneralItemType.Weapons: return weapons;
            case GeneralItemType.Tools: return tools;
            case GeneralItemType.Projectiles: return projectiles;
            case GeneralItemType.Equipments: return equipments;
            default:
                return null;   
        }
    }

    public static GeneralItemType GetGeneralItemType(ItemType itemType)
    {
        if (weapons.Contains(itemType))
            return GeneralItemType.Weapons;
        if (tools.Contains(itemType))
            return GeneralItemType.Tools;
        if (projectiles.Contains(itemType))
            return GeneralItemType.Projectiles;
        if (equipments.Contains(itemType))
            return GeneralItemType.Equipments;


        throw new System.Exception();
    }



}