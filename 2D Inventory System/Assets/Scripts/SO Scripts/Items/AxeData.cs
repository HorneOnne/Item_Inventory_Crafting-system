using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Pickaxe Object", menuName = "ScriptableObject/Item/Tools/Pickaxe Object", order = 51)]
public class AxeData : ItemData
{
    [Header("Pickaxe Properties")]
    public byte axePower;


}
