using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Item, IUpgradeable
{
    public int CurrentLevel { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int MaxLevel { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    [Header("References")]
    private PlayerInventory playerInventory;

    [Header("Bow Properties")]
    private BowData bowData;
    private Arrow arrowInPlayerInventory;
    private int? arrowSlotIndex;
    GameObject arrowPrefab;



    protected override void Start()
    {
        base.Start();
        base.SetOffsetPosition();

        bowData = (BowData)this.ItemData;
        arrowPrefab = ItemContainerManager.Instance.GetItemPrefab("Arrow");
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
        if (arrowPrefab == null) return false;

        
        arrowInPlayerInventory = Instantiate(arrowPrefab, player.transform.position, player.HandHoldItem.rotation).GetComponent<Arrow>();
        arrowInPlayerInventory.SetData(playerInventory.inventory[(int)arrowSlotIndex]);
        
        arrowInPlayerInventory.Shoot(bowData, bowData.releaseSpeed);
        arrowInPlayerInventory.Consume(player);
        
        return true;
    }


    public void Upgrade()
    {
        //throw new System.NotImplementedException();
    }
}
