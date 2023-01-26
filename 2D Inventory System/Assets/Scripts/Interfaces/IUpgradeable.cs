using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeable 
{
    public int CurrentLevel { get;set; }
    public int MaxLevel { get; set; }   

    public void Upgrade();

    public bool IsMaxLevel();
    public bool CanUpgrade();

}
