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

            var itemPrefab = ItemDataManager.Instance.GetItemPrefab(itemSlot.ItemData.itemType.ToString());
            if (itemPrefab != null)
            {
                returnGameObject = MonoBehaviour.Instantiate(ItemDataManager.Instance.GetItemPrefab(itemSlot.ItemData.itemType.ToString()), parent);
                returnGameObject.GetComponent<Item>().SetData(itemSlot);
     
            }
           
            return returnGameObject.GetComponent<Item>();
        }

    }
}

