using UnityEngine;

public interface IPlaceable
{
    public LayerMask PlacedLayer { get; set; }
    public bool IsAboveGround(Player player);
    public void Placed(Vector3 placedPosition, Player player = null, Transform parent = null);
}