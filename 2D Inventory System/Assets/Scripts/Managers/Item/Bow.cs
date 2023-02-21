using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bow : Item, IUpgradeable, IConsumability
{
    public int CurrentLevel { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int MaxLevel { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool Consumability { get => consumeArrow; set => consumeArrow = value; }

    [Header("References")]
    private PlayerInventory playerInventory;

    [Header("Bow Properties")]
    private BowData bowData;
    [SerializeField] private bool consumeArrow;


    private ArrowData arrowData;
    private int? arrowSlotIndex;
    private ItemSlot arrowSlotInPlayerInventory;
    private GameObject arrowProjectilePrefab;
    private ArrowProjectile_001 arrowProjectileObject;

    // Cached

    protected override void Start()
    {
        base.Start();
        base.SetOffsetPosition();

        bowData = (BowData)this.ItemData;
        arrowProjectilePrefab = ItemContainerManager.Instance.GetItemPrefab("ArrowProjectile_001");
        
    }

   

    public bool CanUpgrade()
    {
        //throw new System.NotImplementedException();
        return false;
    }

    public bool IsMaxLevel()
    {
        //throw new System.NotImplementedException();
        return false;
    }


    public override bool Use(Player player)
    {
        playerInventory = player.PlayerInventory;
        arrowSlotIndex = playerInventory.FindArrowSlotIndex();

        if (arrowSlotIndex == null) return false;
        if (arrowProjectilePrefab == null) return false;


        arrowProjectileObject = ArrowSpawner.Instance.Pool.Get().GetComponent<ArrowProjectile_001>();
        arrowProjectileObject.transform.position = transform.position;
        arrowProjectileObject.transform.rotation = transform.rotation;
      
        arrowSlotInPlayerInventory = playerInventory.inventory[(int)arrowSlotIndex];      
        arrowData = (ArrowData)arrowSlotInPlayerInventory.ItemData;
        arrowProjectileObject.SetData(arrowData);    
        arrowProjectileObject.Shoot(bowData, arrowData);

        Consume(player);
        return true;
    }



    public void Upgrade()
    {
        //throw new System.NotImplementedException();
    }

    public void Consume(Player fromPlayer)
    {
        if (Consumability)
        {
            arrowSlotInPlayerInventory.RemoveItem();
            UIPlayerInventory.Instance.UpdateInventoryUIAt((int)arrowSlotIndex);
        }
    }
}
