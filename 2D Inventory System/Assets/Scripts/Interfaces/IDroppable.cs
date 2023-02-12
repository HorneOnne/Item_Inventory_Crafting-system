using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDroppable 
{
    public void Drop(Player player, Vector2 position, Vector3 rotation);
}
