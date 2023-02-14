using UnityEngine;
using System;

public static class EventManager 
{
    public static event Action OnInventoryUpdate;
    public static event Action OnChestInventoryUpdate;

    public static event Action OnOpenChest;
    public static event Action OnCloseChest;

    public static event Action OnItemInHandChanged;

    public static event Action OnPlayerEquipmentChanged;


    public static event Action OnGridChanged;
    public static event Action OnGetOutputItem;

    public static void PlayerInventoryUpdate() => OnInventoryUpdate?.Invoke();
    public static void ChestInventoryUpdate() => OnChestInventoryUpdate?.Invoke();


    public static void OpenChest() => OnOpenChest?.Invoke();
    public static void CloseChest() => OnCloseChest?.Invoke();

    public static void ItemInHandChanged() => OnItemInHandChanged?.Invoke();

    public static void PlayerEquipmentChanged() => OnPlayerEquipmentChanged?.Invoke();

    public static void GridChanged() => OnGridChanged?.Invoke();
    public static void GetOutputItem() => OnGetOutputItem?.Invoke();
}
