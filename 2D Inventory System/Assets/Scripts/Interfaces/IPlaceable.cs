using UnityEngine;

public interface IPlaceable
{
    public bool ShowRay { get; set; }
    /// <summary>
    /// Layer for placing item on it.
    /// </summary>
    public LayerMask PlacedLayer { get; set; }
    

    /// <summary>
    /// Check this item is above "PlacedLayer" layer.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="showRay"></param>
    /// <returns></returns>
    public bool IsAboveGround(Player player, bool showRay = false);
    
    
    /// <summary>
    /// Placed item at a specific position.
    /// </summary>
    /// <param name="placedPosition"></param>
    /// <param name="player"></param>
    /// <param name="parent"></param>
    public void Placed(Vector3 placedPosition, Player player = null, Transform parent = null);
}