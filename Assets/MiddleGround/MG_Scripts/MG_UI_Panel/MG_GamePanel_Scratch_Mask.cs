using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiddleGround.UI
{
    public class MG_GamePanel_Scratch_Mask : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IDragHandler
    {
        MG_GamePanel_Scratch parent;
        private void Awake()
        {
            parent = transform.GetComponentInParent<MG_GamePanel_Scratch>();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            parent.StartBrush(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            parent.EndBrush(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            parent.SetBrushPos(eventData.position);
        }
    }
}
