using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sword : Item, IUpgradeable
{
    private int currentLevel = 1;
    private int maxLevel = 3;

    private GameObject swordProjectilePrefab;
    private GameObject swordProjectileObject;

    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public int MaxLevel { get => maxLevel; set => maxLevel= value; }

    public bool CanUpgrade()
    {
        return !IsMaxLevel();
    }

    public bool IsMaxLevel()
        => currentLevel <= maxLevel ? true : false;



    protected override void Start()
    {
        base.Start();
        swordProjectilePrefab = ItemContainerManager.Instance.GetItemPrefab("SwordProjectile_001");
    }

    public void Upgrade()
    {
        /*if(CanUpgrade())
        {
            currentLevel++;

            base.itemData = ((SwordData)base.itemData).upgradeSwordData;
            base.UpdateData();       
        }*/
        
    }


    public override bool Use(Player player)
    {       
        swordProjectilePrefab = ItemContainerManager.Instance.GetItemPrefab("SwordProjectile_001");
        swordProjectileObject = Instantiate(swordProjectilePrefab, transform.position, transform.rotation, player.transform);
        swordProjectileObject.transform.localScale = new Vector3(4,4,1);
        swordProjectileObject.SetActive(true);
        swordProjectileObject.GetComponent<Projectile>().SetData(this.ItemSlot.ItemData);
        swordProjectileObject.GetComponent<Projectile>().SetOffsetPosition();
        

        return true;
    }

    public void Update()
    {
        if (swordProjectilePrefab == null) return;
        
        swordProjectilePrefab.transform.Rotate(new Vector3(0, 0, -360) * Time.deltaTime);

        if(swordProjectilePrefab.transform.rotation == Quaternion.Euler(0,0,-80))
        {
            Destroy(swordProjectilePrefab);
        }
        Destroy(swordProjectilePrefab, 1f);

    }
}
