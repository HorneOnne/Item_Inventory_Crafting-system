using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerEquipment : MonoBehaviour
{
    private Player player;

    public ItemSlot helmSlot;
    public ItemSlot chestSlot;
    public ItemSlot shieldSlot;


    private void Awake()
    {
        player = GetComponent<Player>(); 
    }

    public bool TryEquipHelm(ItemSlot equipItemSlot)
    {
        bool canEquip = false;

        if(equipItemSlot.itemObject.itemType == ItemType.Helm)
        {
            canEquip = true;
            helmSlot = new ItemSlot(equipItemSlot);
        }

        return canEquip;
    }

    public bool TryEquipChest(ItemSlot equipItemSlot)
    {
        bool canEquip = false;

        if (equipItemSlot.itemObject.itemType == ItemType.ChestArmor)
        {
            canEquip = true;
            chestSlot = new ItemSlot(equipItemSlot);
        }

        return canEquip;
    }

    public bool TryEquipShield(ItemSlot equipItemSlot)
    {
        bool canEquip = false;

        if (equipItemSlot.itemObject.itemType == ItemType.Shield)
        {
            canEquip = true;
            shieldSlot = new ItemSlot(equipItemSlot);
        }

        return canEquip;
    }


    public bool Equip(ItemType itemType, ItemSlot equipItemSlot)
    {
        switch (itemType)
        {
            case ItemType.Helm:
                return TryEquipHelm(equipItemSlot);
            case ItemType.ChestArmor:
                return TryEquipChest(equipItemSlot);
            case ItemType.Shield:
                return TryEquipShield(equipItemSlot);
            default:
                throw new System.Exception();
        }
    }

    public ItemSlot GetEquipmentSlot(ItemType equipmentType)
    {
        switch (equipmentType)
        {
            case ItemType.Helm:
                return helmSlot;
            case ItemType.ChestArmor:
                return chestSlot;
            case ItemType.Shield:
                return shieldSlot;
            default:
                return null;
        }
    }

}
