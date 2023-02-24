using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DIVH_InventorySystem
{
    public static class Utilities
    {
        public static void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            EventTrigger.Entry eventTrigger = new EventTrigger.Entry();
            eventTrigger.eventID = type;
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }



        public static Item InstantiateItemObject(ItemSlot itemSlot, Transform parent = null)
        {
            GameObject returnGameObject = null;

            var itemPrefab = GameDataManager.Instance.GetItemPrefab($"IP_{itemSlot.ItemData.itemType}");
            if (itemPrefab != null)
            {
                returnGameObject = MonoBehaviour.Instantiate(itemPrefab, parent);
                returnGameObject.GetComponent<Item>().SetData(itemSlot);
  
            }
            else
            {
                throw new System.Exception($"Not found prefab name {itemSlot.ItemData.itemType} in GameDataManager.cs");
            }


            return returnGameObject.GetComponent<Item>();
        }


        public static void RotateObjectTowardMouse2D(Transform objectTransform, float offsetZAngle)
        {
            Vector3 spritePos = objectTransform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // Ensure the z-coordinate is 0 to keep the mouse and sprite on the same plane

            // Calculate the difference between the sprite's position and the mouse position
            float dx = mousePos.x - spritePos.x;
            float dy = mousePos.y - spritePos.y;

            // Calculate the angle between the sprite's position and the mouse position
            float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

            // Set the rotation of the sprite to the calculated angle
            objectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + offsetZAngle));
        }

    }
}

