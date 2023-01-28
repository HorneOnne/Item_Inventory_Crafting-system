using UnityEngine;

public class UpgradeableItemData : ItemData
{
    public int currentLevel;
    public int maxLevel;

    [Header("Upgrade References")]
    public UpgradeableItemData nextLvItemData;
}
