using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerEquipment : MonoBehaviour
{
    [Header("References")]
    private Player player;
    [SerializeField] private SpriteRenderer helmSr;
    [SerializeField] private SpriteRenderer chestSr;


    #region Properties
    [field: SerializeField] public ItemSlot Helm {get; private set;}
    [field: SerializeField] public ItemSlot Chest { get; private set; }
    [field: SerializeField] public ItemSlot Shield { get; private set; }
    #endregion


    private void Awake()
    {
        player = GetComponent<Player>(); 
    }


    private void OnEnable()
    {
        EventManager.OnPlayerEquipmentChanged += UpdatePlayerEquipSkin;
    }

    private void OnDisable()
    {
        EventManager.OnPlayerEquipmentChanged -= UpdatePlayerEquipSkin;
    }


    private bool TryEquipHelm(ItemSlot equipItemSlot)
    {
        bool canEquip = false;

        if(equipItemSlot.ItemData.itemType == ItemType.Helm)
        {
            canEquip = true;
            Helm = new ItemSlot(equipItemSlot);
        }

        return canEquip;
    }

    private bool TryEquipChest(ItemSlot equipItemSlot)
    {
        bool canEquip = false;

        if (equipItemSlot.ItemData.itemType == ItemType.ChestArmor)
        {
            canEquip = true;
            Chest = new ItemSlot(equipItemSlot);
        }

        return canEquip;
    }

    private bool TryEquipShield(ItemSlot equipItemSlot)
    {
        bool canEquip = false;

        if (equipItemSlot.ItemData.itemType == ItemType.Shield)
        {
            canEquip = true;
            Shield = new ItemSlot(equipItemSlot);
        }

        return canEquip;
    }


    public bool Equip(ItemType itemType, ItemSlot equipItemSlot)
    {
        bool canEquip;
        switch (itemType)
        {
            case ItemType.Helm:
                canEquip = TryEquipHelm(equipItemSlot);
                break;
            case ItemType.ChestArmor:
                canEquip = TryEquipChest(equipItemSlot);
                break;
            case ItemType.Shield:
                canEquip = TryEquipShield(equipItemSlot);
                break;
            default:
                canEquip = false;
                throw new System.Exception();
        }

        return canEquip;
    }

    public ItemSlot GetEquipmentSlot(ItemType equipmentType)
    {
        switch (equipmentType)
        {
            case ItemType.Helm:
                return Helm;
            case ItemType.ChestArmor:
                return Chest;
            case ItemType.Shield:
                return Shield;
            default:
                return null;
        }
    }

    private void UpdatePlayerEquipSkin()
    {
        if (helmSr == null || chestSr == null)
            Debug.LogError("Missing player armor skin references.");

        if (Helm.HasItem())
            helmSr.sprite = Helm.GetItemIcon();
        else
            helmSr.sprite = null;

        if (Chest.HasItem())
            chestSr.sprite = Chest.GetItemIcon();
        else
            chestSr.sprite = null;
    }

}
