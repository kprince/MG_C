using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiddleGround.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MG_UIBase : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;
        protected virtual void Awake() 
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        public abstract IEnumerator OnEnter();
        public abstract void OnPause();
        public abstract void OnResume();
        public abstract IEnumerator OnExit();
    }
}
