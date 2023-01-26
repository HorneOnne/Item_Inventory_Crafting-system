using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ISlotHandler
{
    public void OnClick(BaseEventData baseEvent);
    public void OnBeginDrag(BaseEventData baseEvent);
    public void OnDrag(BaseEventData baseEvent);
    public void OnEndDrag(BaseEventData baseEvent);
}
