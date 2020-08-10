using MiddleGround.Save;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Rateus : MG_UIBase
    {
        public Button btn_no;
        public Button btn_yes;
        protected override void Awake()
        {
            base.Awake();
            btn_no.onClick.AddListener(OnNoClick);
            btn_yes.onClick.AddListener(OnYesClick);
        }
        void OnNoClick()
        {
            MG_Manager.Play_ButtonClick();
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.Rateus);
        }
        void OnYesClick()
        {
            MG_Manager.Play_ButtonClick();
#if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=YourPackageName");
#elif UNITY_IOS
        var url = string.Format(
           "itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review",
           "");
        Application.OpenURL(url);
#endif
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.Rateus);
        }
        public override IEnumerator OnEnter()
        {
            MG_SaveManager.HasRateus = true;
            Transform transAll = transform.GetChild(1);
            transAll.localScale = new Vector3(0.8f, 0.8f, 1);
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = true;
            while (transAll.localScale.x < 1)
            {
                yield return null;
                float addValue = Time.unscaledDeltaTime * 2;
                transAll.localScale += new Vector3(addValue, addValue);
                canvasGroup.alpha += addValue;
            }
            transAll.localScale = Vector3.one;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
        }

        public override IEnumerator OnExit()
        {
            Transform transAll = transform.GetChild(1);
            canvasGroup.interactable = false;
            while (transAll.localScale.x > 0.8f)
            {
                yield return null;
                float addValue = Time.unscaledDeltaTime * 2;
                transAll.localScale -= new Vector3(addValue, addValue);
                canvasGroup.alpha -= addValue;
            }
            transAll.localScale = new Vector3(0.8f, 0.8f, 1);
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
    }
}
