using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MyGame.Ultilities
{
    public static class Utilities
    {    
        public static Vector2 GetMousPosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }


      

        public static void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            EventTrigger.Entry eventTrigger = new EventTrigger.Entry();
            eventTrigger.eventID = type;
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }


        /// <summary>
        /// Call callback method after an interval of time.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="timeInterval"></param>
        /// <param name="timeCount"></param>
        public static void InvokeMethodByInterval(System.Action callback, float timeInterval, ref float timeCount)
        {
            timeCount += Time.deltaTime;
            if (timeCount > timeInterval)
            {
                callback();
                timeCount = 0.0f;
            }
        }
    }
}

