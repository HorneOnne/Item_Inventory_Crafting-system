using UnityEngine;

[CreateAssetMenu(fileName = "Arrow", menuName = "ScriptableObject/Item/Projectiles/Arrow", order = 51)]
public class ArrowData : ItemData
{
    [Header("Bow Properties")]
    public int damage;
}
