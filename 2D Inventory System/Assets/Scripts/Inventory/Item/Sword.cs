using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Item, IUpgradeable
{
    private int currentLevel = 1;
    private int maxLevel = 3;

   

    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public int MaxLevel { get => maxLevel; set => maxLevel= value; }

    public bool CanUpgrade()
    {
        return !IsMaxLevel();
    }

    public bool IsMaxLevel()
        => currentLevel <= maxLevel ? true : false;

  
    public void Upgrade()
    {
        /*if(CanUpgrade())
        {
            currentLevel++;

            base.itemData = ((SwordData)base.itemData).upgradeSwordData;
            base.UpdateData();       
        }*/
        
    }
}
