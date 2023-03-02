using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UltimateItemSystem
{
    public static class Utilities
    {
        /// <summary>
        /// Adds an event listener to a game object's EventTrigger component.
        /// </summary>
        /// <param name="obj">The game object to add the event listener to.</param>
        /// <param name="type">The type of event trigger to listen for.</param>
        /// <param name="action">The action to be executed when the event is triggered.</param>
        public static void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            EventTrigger.Entry eventTrigger = new EventTrigger.Entry();
            eventTrigger.eventID = type;
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }


        /// <summary>
        /// Instantiates a new item object based on the data in an item slot.
        /// </summary>
        /// <param name="itemSlot">The item slot containing the data for the item.</param>
        /// <param name="parent">The parent transform to assign to the instantiated item object.</param>
        /// <returns>The newly instantiated item object.</returns>
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


        /// <summary>
        /// Rotates a 2D object towards the mouse position on the screen.
        /// </summary>
        /// <param name="objectTransform">The transform of the object to be rotated.</param>
        /// <param name="offsetZAngle">The additional offset angle to add to the rotation.</param>
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

