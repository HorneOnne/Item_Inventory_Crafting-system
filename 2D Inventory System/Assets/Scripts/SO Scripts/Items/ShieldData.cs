using UnityEngine;

[CreateAssetMenu(fileName = "New Sword Object", menuName = "ScriptableObject/Item/Equipment/Shield", order = 51)]
public class ShieldData : UpgradeableItemData
{
    [Header("Shield Data")]
    public int armor;
}