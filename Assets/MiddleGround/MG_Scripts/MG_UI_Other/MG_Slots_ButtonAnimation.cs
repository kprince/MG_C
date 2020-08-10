using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiddleGround.UI.ButtonAnimation
{
    public class MG_Slots_ButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        Action downMethod;
        Action upMethod;
        public void Init(Action _onDown, Action _onUp)
        {
            downMethod = _onDown;
            upMethod = _onUp;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            downMethod();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            upMethod();
        }
    }
}
