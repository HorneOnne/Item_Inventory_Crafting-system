using UnityEngine;

namespace DIVH_InventorySystem
{
    public interface IDroppable
    {
        public void Drop(Player player, Vector2 position, Vector3 rotation, bool forceDestroyItemObject);
    }
}