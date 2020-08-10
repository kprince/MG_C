using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Tips : MonoBehaviour
    {
        public Text text_content;
        CanvasGroup canvasGroup;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        public void OnEnter()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            text_content.text = MG_Manager.Instance.str_Tips;
            StartCoroutine(AutoClose());
        }

        public void OnExit()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        IEnumerator AutoClose()
        {
            float time = MG_Manager.Instance.time_Tips;
            while (time>0)
            {
                yield return null;
                time -= Time.unscaledDeltaTime;
                text_content.text = MG_Manager.Instance.str_Tips;
            }
            OnExit();
        }
    }
}
